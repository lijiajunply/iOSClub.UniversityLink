using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace iOSClub.UniversityLink.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly SymmetricSecurityKey _key;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public AuthService(IUserService userService)
    {
        _userService = userService;

        // 这里应该从配置中读取密钥，暂时使用硬编码密钥
        // 在实际项目中，应该使用环境变量或配置文件
        var keyString = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ??
                        "your-secret-key-here-change-in-production";
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    // 验证用户凭据
    public async Task<bool> ValidateCredentialsAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.LoginAsync(username, password, cancellationToken);
        return user != null;
    }

    // 生成JWT令牌
    public async Task<string> GenerateTokenAsync(string username, CancellationToken cancellationToken = default)
    {
        // 获取用户信息
        var user = await _userService.GetUserByUsernameAsync(username, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"用户名 '{username}' 不存在");
        }

        // 创建声明
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Identity),
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
        };

        // 创建令牌描述符
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24), // 令牌有效期24小时
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Issuer = "UniversityLink",
            Audience = "UniversityLink-Clients"
        };

        // 生成令牌
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    // 验证JWT令牌
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = "UniversityLink",
                ValidateAudience = true,
                ValidAudience = "UniversityLink-Clients",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // 获取令牌中的用户名
    public string? GetUsernameFromToken(string token)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = "UniversityLink",
                ValidateAudience = true,
                ValidAudience = "UniversityLink-Clients",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal.FindFirstValue(ClaimTypes.Name);
        }
        catch
        {
            return null;
        }
    }

    // 检查用户权限
    public async Task<bool> HasPermissionAsync(string username, string requiredPermission,
        CancellationToken cancellationToken = default)
    {
        // 获取用户信息
        var user = await _userService.GetUserByUsernameAsync(username, cancellationToken);
        if (user == null)
        {
            return false;
        }

        // 管理员拥有所有权限
        if (user.Identity == "Admin")
        {
            return true;
        }

        // 根据角色检查权限
        switch (requiredPermission.ToLower())
        {
            case "read":
                // 所有用户都有读取权限
                return true;

            case "write":
            case "edit":
            case "update":
                // 编辑权限需要User或更高角色
                return user.Identity == "User" || user.Identity == "Admin";

            case "delete":
                // 删除权限需要Admin角色
                return user.Identity == "Admin";

            case "admin":
                // 管理员操作需要Admin角色
                return user.Identity == "Admin";

            default:
                return false;
        }
    }
}