using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Services;

public interface IUserService
{
    // 获取所有用户
    Task<List<UserModel>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    
    // 按ID获取用户
    Task<UserModel?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    
    // 按用户名获取用户
    Task<UserModel?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    
    // 创建用户
    Task<UserModel> CreateUserAsync(UserModel user, string password, CancellationToken cancellationToken = default);
    
    // 更新用户
    Task UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default);
    
    // 更新密码
    Task UpdatePasswordAsync(string userId, string newPassword, CancellationToken cancellationToken = default);
    
    // 删除用户
    Task DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    
    // 用户登录
    Task<UserModel?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}