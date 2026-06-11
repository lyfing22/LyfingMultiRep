using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace DataMigrate.Upload;

/// <summary>
/// JWT 令牌管理器，Timer 定期自动刷新 token。
/// 刷新时机 = ExpiryMinutes - 5 分钟，避免 token 过期导致上传 401。
/// </summary>
public class JwtTokenManager : IDisposable
{
    private readonly string _appId;
    private readonly string _serverNode;
    private readonly int _expiryMinutes;
    private readonly string _secret;
    private readonly Timer _timer;
    private string _currentToken = "";
    private readonly object _lock = new();

    private static readonly IJwtAlgorithm Algorithm = new HMACSHA256Algorithm();
    private static readonly IJsonSerializer Serializer = new JsonNetSerializer();
    private static readonly IBase64UrlEncoder UrlEncoder = new JwtBase64UrlEncoder();
    private static readonly IJwtEncoder Encoder = new JwtEncoder(Algorithm, Serializer, UrlEncoder);

    public JwtTokenManager(string appId, string serverNode, int expiryMinutes, string secret)
    {
        _appId = appId;
        _serverNode = serverNode;
        _expiryMinutes = expiryMinutes;
        _secret = secret;
        RefreshToken();

        // Timer 定期自动刷新：到期前 5 分钟触发
        var ms = TimeSpan.FromMinutes(expiryMinutes - 5);
        _timer = new Timer(_ => RefreshToken(), null, ms, ms);
    }

    public string GetToken()
    {
        lock (_lock)
        {
            if (string.IsNullOrEmpty(_currentToken))
                RefreshToken();
            return _currentToken;
        }
    }

    private void RefreshToken()
    {
        // JWT payload：jti=应用ID, username=服务节点, exp=Unix时间戳
        var exp = (DateTime.UtcNow.AddMinutes(_expiryMinutes) - new DateTime(1970, 1, 1)).TotalSeconds;
        var payload = new Dictionary<string, object>
        {
            ["jti"] = _appId,
            ["username"] = _serverNode,
            ["exp"] = exp,
            ["ValidTo"] = DateTime.Now.AddMinutes(_expiryMinutes)
        };
        var token = Encoder.Encode(payload, _secret);

        lock (_lock)
        {
            _currentToken = token;
        }
    }

    public void Dispose() => _timer.Dispose();
}
