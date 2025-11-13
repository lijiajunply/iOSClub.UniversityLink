# 服务层(Services)设计方案

## 1. 设计原则
- 服务层负责封装业务逻辑
- 服务层依赖于仓储层接口，不直接操作数据库
- 实现业务规则验证和数据转换
- 处理事务管理
- 提供清晰的错误处理机制

## 2. 服务列表

### 2.1 ICategoryService
处理分类相关的业务逻辑

### 2.2 ILinkService
处理链接相关的业务逻辑

### 2.3 IUserService
处理用户相关的业务逻辑

### 2.4 IAuthService
处理认证和授权相关的业务逻辑

## 3. 接口设计

### 3.1 ICategoryService 接口
```csharp
public interface ICategoryService
{
    // 获取所有分类
    Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync(bool includeLinks = false);
    
    // 根据Key获取分类
    Task<CategoryModel> GetCategoryByKeyAsync(string key, bool includeLinks = false);
    
    // 创建分类
    Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
    
    // 更新分类
    Task<CategoryModel> UpdateCategoryAsync(string key, CategoryModel category);
    
    // 删除分类
    Task<bool> DeleteCategoryAsync(string key);
    
    // 重新排序分类
    Task<bool> ReorderCategoryAsync(string key, int newIndex);
    
    // 获取分类下的所有链接
    Task<IEnumerable<LinkModel>> GetCategoryLinksAsync(string categoryKey);
}
```

### 3.2 ILinkService 接口
```csharp
public interface ILinkService
{
    // 获取所有链接
    Task<IEnumerable<LinkModel>> GetAllLinksAsync();
    
    // 根据Key获取链接
    Task<LinkModel> GetLinkByKeyAsync(string key);
    
    // 创建链接
    Task<LinkModel> CreateLinkAsync(LinkModel link, string categoryKey);
    
    // 更新链接
    Task<LinkModel> UpdateLinkAsync(string key, LinkModel link);
    
    // 删除链接
    Task<bool> DeleteLinkAsync(string key);
    
    // 移动链接到其他分类
    Task<bool> MoveLinkAsync(string linkKey, string targetCategoryKey);
    
    // 重新排序链接
    Task<bool> ReorderLinkAsync(string linkKey, int newIndex);
    
    // 批量更新链接
    Task<bool> BulkUpdateLinksAsync(IEnumerable<LinkModel> links);
}
```

### 3.3 IUserService 接口
```csharp
public interface IUserService
{
    // 获取所有用户
    Task<IEnumerable<UserModel>> GetAllUsersAsync();
    
    // 根据ID获取用户
    Task<UserModel> GetUserByIdAsync(string userId);
    
    // 创建用户
    Task<UserModel> CreateUserAsync(UserModel user);
    
    // 更新用户
    Task<UserModel> UpdateUserAsync(string userId, UserModel user);
    
    // 删除用户
    Task<bool> DeleteUserAsync(string userId);
    
    // 更新用户身份
    Task<bool> UpdateUserIdentityAsync(string userId, string identity);
    
    // 检查用户是否存在
    Task<bool> UserExistsAsync(string userId);
}
```

### 3.4 IAuthService 接口
```csharp
public interface IAuthService
{
    // 用户登录
    Task<string> LoginAsync(string userName, string userId);
    
    // 验证用户身份
    Task<bool> ValidateUserAsync(string userName, string userId);
    
    // 生成JWT令牌
    string GenerateJwtToken(UserModel user);
    
    // 解析JWT令牌
    UserModel? ParseJwtToken(string token);
    
    // 验证用户权限
    bool HasPermission(UserModel user, string requiredPermission);
    
    // 获取当前登录用户
    UserModel? GetCurrentUser();
}
```

## 4. 实现类设计

### 4.1 CategoryService 实现
```csharp
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILinkRepository _linkRepository;
    
    public CategoryService(ICategoryRepository categoryRepository, ILinkRepository linkRepository)
    {
        _categoryRepository = categoryRepository;
        _linkRepository = linkRepository;
    }
    
    // 实现接口方法...
}
```

### 4.2 LinkService 实现
```csharp
public class LinkService : ILinkService
{
    private readonly ILinkRepository _linkRepository;
    private readonly ICategoryRepository _categoryRepository;
    
    public LinkService(ILinkRepository linkRepository, ICategoryRepository categoryRepository)
    {
        _linkRepository = linkRepository;
        _categoryRepository = categoryRepository;
    }
    
    // 实现接口方法...
}
```

### 4.3 UserService 实现
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    // 实现接口方法...
}
```

### 4.4 AuthService 实现
```csharp
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _jwtSecretKey;
    
    public AuthService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _jwtSecretKey = configuration["Jwt:SecretKey"]!;
    }
    
    // 实现接口方法...
}
```

## 5. 业务规则实现

### 5.1 权限检查
在服务层实现基于用户角色的权限控制：
- Founder：创始人，可以执行所有操作
- Manager：管理员，可以执行大部分操作
- Member：普通成员，只能查看内容

### 5.2 数据验证
- 验证必填字段
- 验证数据格式
- 验证数据唯一性
- 验证操作权限

### 5.3 事务管理
在需要的服务方法中使用事务确保数据一致性，特别是在涉及多个实体修改的场景：
- 移动链接时更新原分类和目标分类
- 重新排序时更新多个项目的索引

## 6. 错误处理
服务层应该抛出具体的业务异常，包括：
- EntityNotFoundException：实体不存在
- ValidationException：数据验证失败
- AuthorizationException：权限不足
- DuplicateEntityException：实体重复

这些异常将在控制器层被捕获并转换为适当的HTTP响应。

## 7. 缓存策略
对于频繁访问的数据，可以在服务层实现缓存策略：
- 缓存分类列表
- 缓存热门链接
- 缓存用户信息

使用内存缓存或分布式缓存提高性能。