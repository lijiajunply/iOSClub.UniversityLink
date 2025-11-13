using Microsoft.EntityFrameworkCore;
using UniversityLink.DataModels;
using UniversityLink.DataModels.Repositories;

namespace iOSClub.UniversityLink.Services.Repositories;

public class UserRepository(LinkContext context) : IUserRepository
{
    // 获取所有用户
    public async Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users
            .OrderBy(u => u.UserId)
            .ToListAsync(cancellationToken);
    }
    
    // 根据ID获取用户
    public async Task<UserModel?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }
    
    // 根据用户名获取用户
    public async Task<UserModel?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
    
    // 创建用户
    public async Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return user;
    }
    
    // 更新用户
    public async Task<UserModel> UpdateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return user;
    }
    
    // 删除用户
    public async Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user == null)
        {
            return false;
        }
        
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    // 检查用户是否存在
    public async Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(u => u.UserId == userId, cancellationToken);
    }
    
    // 获取用户总数
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users.CountAsync(cancellationToken);
    }
    
    // 根据身份获取用户
    public async Task<IEnumerable<UserModel>> GetByIdentityAsync(string identity, CancellationToken cancellationToken = default)
    {
        // 根据UserModel定义，使用Name属性进行搜索
        return await context.Users
            .Where(u => u.Identity == identity)
            .OrderBy(u => u.UserId)
            .ToListAsync(cancellationToken);
    }
}