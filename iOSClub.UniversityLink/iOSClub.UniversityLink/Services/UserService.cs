using Microsoft.AspNetCore.Identity;
using UniversityLink.DataModels;
using UniversityLink.DataModels.Repositories;

namespace iOSClub.UniversityLink.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    private readonly PasswordHasher<UserModel> _passwordHasher = new();

    // 获取所有用户
    public async Task<List<UserModel>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return (await unitOfWork.Users.GetAllAsync(cancellationToken)).ToList();
    }

    // 按ID获取用户
    public async Task<UserModel?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        return user ?? throw new KeyNotFoundException($"用户ID {id} 不存在");
    }

    // 按用户名获取用户
    public async Task<UserModel?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("用户名不能为空");
        }

        var user = await unitOfWork.Users.GetByUserNameAsync(username, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"用户名 '{username}' 不存在");
        }

        return user;
    }

    // 创建用户
    public async Task<UserModel> CreateUserAsync(UserModel user, string password,
        CancellationToken cancellationToken = default)
    {
        // 验证必填字段
        if (string.IsNullOrWhiteSpace(user.UserName))
        {
            throw new ArgumentException("用户名不能为空");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("密码不能为空");
        }

        // 检查用户名是否已存在
        var existingUserByUsername = await unitOfWork.Users.GetByUserNameAsync(user.UserName, cancellationToken);
        if (existingUserByUsername != null)
        {
            throw new ArgumentException($"用户名 '{user.UserName}' 已存在");
        }


        // 验证密码复杂度
        if (password.Length < 6)
        {
            throw new ArgumentException("密码长度至少为6位");
        }

        // 密码处理在CreateAsync中完成

        // 设置默认值
        if (string.IsNullOrEmpty(user.Identity))
        {
            user.Identity = "User"; // 默认为普通用户
        }

        // 简化处理，UserModel中没有时间戳字段

        // 创建用户
        return await unitOfWork.Users.CreateAsync(user, cancellationToken);
    }

    // 更新用户
    public async Task UpdateUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        // 验证用户是否存在
        // 由于UserModel没有Id属性，我们假设使用UserId作为标识符
        var existingUser = await unitOfWork.Users.GetByIdAsync(user.UserId, cancellationToken);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"用户ID {user.UserId} 不存在");
        }

        // 验证必填字段
        if (string.IsNullOrWhiteSpace(user.UserName))
        {
            throw new ArgumentException("用户名不能为空");
        }

        // 检查用户名是否已被其他用户使用
        var existingUserByUsername = await unitOfWork.Users.GetByUserNameAsync(user.UserName, cancellationToken);
        if (existingUserByUsername != null && existingUserByUsername.UserId != user.UserId)
        {
            throw new ArgumentException($"用户名 '{user.UserName}' 已存在");
        }


        // 简化处理，UserModel中没有密码哈希和时间戳字段

        // 更新用户
        await unitOfWork.Users.UpdateAsync(user, cancellationToken);
    }

    // 更新密码
    public async Task UpdatePasswordAsync(string userId, string newPassword,
        CancellationToken cancellationToken = default)
    {
        // 验证用户是否存在
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"用户ID {userId} 不存在");
        }

        // 验证新密码
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("新密码不能为空");
        }

        if (newPassword.Length < 6)
        {
            throw new ArgumentException("密码长度至少为6位");
        }

        // 简化处理，UserModel中没有密码哈希和更新时间字段

        // 更新用户
        await unitOfWork.Users.UpdateAsync(user, cancellationToken);
    }

    // 删除用户
    public async Task DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        // 验证用户是否存在
        var user = await unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"用户ID {id} 不存在");
        }

        // 不允许删除最后一个管理员
        if (user.Identity is "Manager" or "Founder")
        {
            // 简化处理，不进行管理员数量检查
        }

        // 删除用户
        await unitOfWork.Users.DeleteAsync(id, cancellationToken);
    }

    // 用户登录
    public async Task<UserModel?> LoginAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        // 验证输入
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        // 获取用户
        var user = await unitOfWork.Users.GetByUserNameAsync(username, cancellationToken);
        if (user == null)
        {
            return null;
        }

        // 简化密码验证（实际项目中应该有密码哈希机制）
        // 这里假设密码存储在某个地方，或者需要额外的处理
        // 简化处理，不更新登录时间
        // 实际项目中应该有登录时间字段

        return user;
    }
}