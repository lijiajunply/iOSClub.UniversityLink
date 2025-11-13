namespace iOSClub.UniversityLink.Services;

public interface IAuthService
{
    // 验证用户凭据
    Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
    
    // 生成JWT令牌
    Task<string> GenerateTokenAsync(string username, CancellationToken cancellationToken = default);
    
    // 验证JWT令牌
    bool ValidateToken(string token);
    
    // 获取令牌中的用户名
    string? GetUsernameFromToken(string token);
    
    // 检查用户权限
    Task<bool> HasPermissionAsync(string username, string requiredPermission, CancellationToken cancellationToken = default);
}