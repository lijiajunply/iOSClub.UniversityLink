using System.Threading;
using System.Threading.Tasks;

namespace UniversityLink.DataModels.Repositories;

public interface IUserRepository
{
    // 获取所有用户
    Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // 根据ID获取用户
    Task<UserModel?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    
    // 根据用户名获取用户
    Task<UserModel?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    
    // 创建用户
    Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken = default);
    
    // 更新用户
    Task<UserModel> UpdateAsync(UserModel user, CancellationToken cancellationToken = default);
    
    // 删除用户
    Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default);
    
    // 检查用户是否存在
    Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default);
    
    // 获取用户总数
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    // 根据身份获取用户
    Task<IEnumerable<UserModel>> GetByIdentityAsync(string identity, CancellationToken cancellationToken = default);
}