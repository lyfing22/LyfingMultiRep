namespace DataMigrate.Core;

/// <summary>线程安全的迁移统计，由多个消费者协程并发更新</summary>
public class MigrationStatistics
{
    public int TotalRecords { get; set; }
    public int Succeeded { get; private set; }
    public int Failed { get; private set; }
    public int Processed => Succeeded + Failed;
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public TimeSpan Elapsed => EndTime.HasValue
        ? EndTime.Value - StartTime
        : DateTime.Now - StartTime;
    public double RecordsPerSecond => Elapsed.TotalSeconds > 0
        ? Processed / Elapsed.TotalSeconds : 0;

    private readonly object _lock = new();

    public void Start() => StartTime = DateTime.Now;
    public void Stop() => EndTime = DateTime.Now;

    // 并发安全：lock 保护 int 递增（Interlocked.Increment 也可，但此处统一用 lock）
    public void IncrementSucceeded() { lock (_lock) { Succeeded++; } }
    public void IncrementFailed() { lock (_lock) { Failed++; } }

    public void PrintSummary()
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 50));
        Console.WriteLine("  迁移完成");
        Console.WriteLine($"  总记录:     {TotalRecords,10:N0}");
        Console.WriteLine($"  成功:       {Succeeded,10:N0}");
        Console.WriteLine($"  失败:       {Failed,10:N0}");
        Console.WriteLine($"  耗时:       {Elapsed:hh\\:mm\\:ss}");
        Console.WriteLine($"  速度:       {RecordsPerSecond,10:F1} 条/秒");
        Console.WriteLine(new string('=', 50));
    }
}
