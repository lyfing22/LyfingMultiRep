using Dapper;
using DataMigrate.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DataMigrate.Upload;

/// <summary>
/// HTTP 上传 + ZTemp 错误缓冲。
/// 单个 UploadAsync 立即发起 HTTP 请求，结果先存入内存缓冲 _buffer，
/// 攒够 _batchSize 条（或结束时）批量 MERGE 到 SQL Server ZTemp_MigrateError 表。
/// 分离 HTTP 和 SQL 的目的是：不阻塞上传管道等待 SQL 写入。
/// </summary>
public class ArchiveUploader
{
    private readonly HttpClient _http;
    private readonly JwtTokenManager _jwt;
    private readonly string _destConn;
    private readonly string _dbFlag;
    private readonly int _batchSize;
    private readonly ILogger<ArchiveUploader> _logger;
    // 内存缓冲：每条记录的上传结果，由 FlushAsync 批量刷入 SQL Server
    private readonly List<(string AccNum, bool Success, string? Error)> _buffer = new();

    public ArchiveUploader(
        HttpClient http,
        JwtTokenManager jwt,
        string destConn,
        string dbFlag,
        int batchSize,
        ILogger<ArchiveUploader> logger)
    {
        _http = http;
        _jwt = jwt;
        _destConn = destConn;
        _dbFlag = dbFlag;
        _batchSize = batchSize;
        _logger = logger;
    }

    /// <summary>上传单条 MigrateArchive，返回成功/失败，结果写入缓冲</summary>
    public async Task<ArchiveUploadResult> UploadAsync(MigrateArchive archive, CancellationToken ct)
    {
        try
        {
            var token = _jwt.GetToken();
            using var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(archive, ArchiveJson.Serialize),
                System.Text.Encoding.UTF8,
                "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Content = content
            };
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _http.SendAsync(request, ct);
            if (response.IsSuccessStatusCode)
            {
                _buffer.Add((archive.Order.AccessionNumber, true, null));
                await FlushIfNeededAsync();
                return new ArchiveUploadResult { Success = true };
            }

            var body = await response.Content.ReadAsStringAsync(ct);
            var msg = $"HTTP {(int)response.StatusCode}: {body}";
            _buffer.Add((archive.Order.AccessionNumber, false, msg));
            await FlushIfNeededAsync();
            return new ArchiveUploadResult { Success = false, ErrorMessage = msg };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            var msg = ex.Message;
            _buffer.Add((archive.Order.AccessionNumber, false, msg));
            await FlushIfNeededAsync();
            return new ArchiveUploadResult { Success = false, ErrorMessage = msg };
        }        
    }

    /// <summary>将缓冲中的结果批量写入 ZTemp_MigrateError 表（MERGE 幂等写入）</summary>
    public async Task FlushAsync()
    {
        if (_buffer.Count == 0) return;
        List<(string AccNum, bool Success, string? Error)> batch;
        lock (_buffer) { batch = new List<(string, bool, string?)>(_buffer); _buffer.Clear(); }

        try
        {
            using var conn = new SqlConnection(_destConn);
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            // 第一步：删除本次成功记录的错误行（之前失败但现在已成功的，清除旧错误）
            var successes = batch.Where(b => b.Success).Select(b => b.AccNum).Distinct().ToList();
            if (successes.Count > 0)
            {
                var pars = successes.Select((_, i) => $"@a{i}").ToList();
                var dp = new DynamicParameters();
                dp.Add("DbFlag", _dbFlag);
                for (int i = 0; i < successes.Count; i++)
                    dp.Add($"a{i}", successes[i]);
                await conn.ExecuteAsync(
                    $"DELETE FROM ZTemp_MigrateError WHERE DbFlag=@DbFlag AND AccessionNumber IN ({string.Join(",", pars)})",
                    dp, tx);
            }

            // 第二步：插入/更新失败记录（MERGE 幂等，同一条检查号只保留最后一次错误）
            foreach (var f in batch.Where(b => !b.Success))
            {
                await conn.ExecuteAsync(@"
MERGE ZTemp_MigrateError AS t
USING (SELECT @DbFlag AS DbFlag, @Acc AS AccNum) AS s
ON t.DbFlag=s.DbFlag AND t.AccessionNumber=s.AccNum
WHEN MATCHED THEN UPDATE SET ErrorMessage=@Msg, LastOccurredAt=GETDATE()
WHEN NOT MATCHED THEN INSERT (DbFlag, AccessionNumber, ErrorMessage, LastOccurredAt)
    VALUES (@DbFlag, @Acc, @Msg, GETDATE());",
                    new { DbFlag = _dbFlag, Acc = f.AccNum, Msg = f.Error }, tx);
            }

            tx.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ZTemp 批量写入失败, 丢失 {Count} 条", batch.Count);
        }
    }

    private async Task FlushIfNeededAsync()
    {
        bool shouldFlush;
        lock (_buffer) { shouldFlush = _buffer.Count >= _batchSize; }
        if (shouldFlush) await FlushAsync();
    }

    /// <summary>迁移结束时调用，确保所有缓冲都被刷入</summary>
    public async Task FlushRemainingAsync()
    {
        await FlushAsync();
        lock (_buffer)
        {
            if (_buffer.Count > 0)
            {
                var remaining = new List<(string, bool, string?)>(_buffer);
                _buffer.Clear();
                _logger.LogWarning("还有 {Count} 条记录未能写入 ZTemp", remaining.Count);
            }
        }
    }
}

public class ArchiveUploadResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
