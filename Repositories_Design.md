# 仓储层(Repositories)设计方案

## 1. 设计原则
- 仓储层负责所有数据访问操作
- 提供面向集合的领域对象访问接口
- 封装数据持久化细节
- 支持事务操作
- 实现查询组合和过滤

## 2. 仓储列表

### 2.1 ICategoryRepository
处理分类实体的数据访问

### 2.2 ILinkRepository
处理链接实体的数据访问

### 2.3 IUserRepository
处理用户实体的数据访问

### 2.4 IUnitOfWork
提供工作单元模式，管理多个仓储的事务

## 3. 接口设计

### 3.1 ICategoryRepository 接口
```csharp
public interface ICategoryRepository
{
    // 获取所有分类
    Task<IEnumerable<CategoryModel>> GetAllAsync(bool includeLinks = false, CancellationToken cancellationToken = default);
    
    // 根据Key获取分类
    Task<CategoryModel?> GetByKeyAsync(string key, bool includeLinks = false, CancellationToken cancellationToken = default);
    
    // 根据名称获取分类
    Task<CategoryModel?> GetByNameAsync(string name, bool includeLinks = false, CancellationToken cancellationToken = default);
    
    // 创建分类
    Task<CategoryModel> CreateAsync(CategoryModel category, CancellationToken cancellationToken = default);
    
    // 更新分类
    Task<CategoryModel> UpdateAsync(CategoryModel category, CancellationToken cancellationToken = default);
    
    // 删除分类
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
    
    // 检查分类是否存在
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    
    // 获取分类总数
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    // 批量更新分类
    Task<int> BulkUpdateAsync(IEnumerable<CategoryModel> categories, CancellationToken cancellationToken = default);
}
```

### 3.2 ILinkRepository 接口
```csharp
public interface ILinkRepository
{
    // 获取所有链接
    Task<IEnumerable<LinkModel>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // 根据Key获取链接
    Task<LinkModel?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    
    // 根据分类获取链接
    Task<IEnumerable<LinkModel>> GetByCategoryAsync(string categoryKey, CancellationToken cancellationToken = default);
    
    // 创建链接
    Task<LinkModel> CreateAsync(LinkModel link, CancellationToken cancellationToken = default);
    
    // 更新链接
    Task<LinkModel> UpdateAsync(LinkModel link, CancellationToken cancellationToken = default);
    
    // 删除链接
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
    
    // 检查链接是否存在
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    
    // 获取链接总数
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    // 获取分类下的链接数量
    Task<int> CountByCategoryAsync(string categoryKey, CancellationToken cancellationToken = default);
    
    // 批量更新链接
    Task<int> BulkUpdateAsync(IEnumerable<LinkModel> links, CancellationToken cancellationToken = default);
    
    // 批量删除链接
    Task<int> BulkDeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}
```

### 3.3 IUserRepository 接口
```csharp
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
```

### 3.4 IUnitOfWork 接口
```csharp
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    // 分类仓储
    ICategoryRepository Categories { get; }
    
    // 链接仓储
    ILinkRepository Links { get; }
    
    // 用户仓储
    IUserRepository Users { get; }
    
    // 保存更改
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    // 开始事务
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    // 提交事务
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    // 回滚事务
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

## 4. 实现类设计

### 4.1 CategoryRepository 实现
```csharp
public class CategoryRepository : ICategoryRepository
{
    private readonly LinkContext _context;
    
    public CategoryRepository(LinkContext context)
    {
        _context = context;
    }
    
    // 实现接口方法...
    
    public async Task<IEnumerable<CategoryModel>> GetAllAsync(bool includeLinks = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsQueryable();
        
        if (includeLinks)
        {
            query = query.Include(c => c.Links.OrderBy(l => l.Index));
        }
        
        return await query.OrderBy(c => c.Index).ToListAsync(cancellationToken);
    }
    
    // 其他方法实现...
}
```

### 4.2 LinkRepository 实现
```csharp
public class LinkRepository : ILinkRepository
{
    private readonly LinkContext _context;
    
    public LinkRepository(LinkContext context)
    {
        _context = context;
    }
    
    // 实现接口方法...
    
    public async Task<IEnumerable<LinkModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Links.ToListAsync(cancellationToken);
    }
    
    // 其他方法实现...
}
```

### 4.3 UserRepository 实现
```csharp
public class UserRepository : IUserRepository
{
    private readonly LinkContext _context;
    
    public UserRepository(LinkContext context)
    {
        _context = context;
    }
    
    // 实现接口方法...
    
    public async Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }
    
    // 其他方法实现...
}
```

### 4.4 UnitOfWork 实现
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly LinkContext _context;
    private IDbContextTransaction? _transaction;
    
    public UnitOfWork(LinkContext context)
    {
        _context = context;
        Categories = new CategoryRepository(context);
        Links = new LinkRepository(context);
        Users = new UserRepository(context);
    }
    
    // 仓储属性
    public ICategoryRepository Categories { get; }
    public ILinkRepository Links { get; }
    public IUserRepository Users { get; }
    
    // 事务管理
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }
    
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) throw new InvalidOperationException("No transaction started");
        
        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) throw new InvalidOperationException("No transaction started");
        
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    // 保存更改
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    
    // 释放资源
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }
    
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        
        await _context.DisposeAsync();
    }
}
```

## 5. 数据访问实现细节

### 5.1 查询优化
- 使用适当的Include加载相关实体
- 使用Select投影减少返回的数据量
- 使用AsNoTracking提高读取性能
- 实现分页查询减少内存消耗

### 5.2 批量操作
- 实现批量插入、更新和删除操作
- 使用EF Core的批量操作功能或第三方库提高性能

### 5.3 事务管理
- 提供显式事务支持
- 确保复杂操作的数据一致性

### 5.4 并发控制
- 实现乐观并发控制
- 处理并发冲突

## 6. 异常处理
仓储层应该捕获并转换数据库异常为应用程序异常：
- DbUpdateException：数据更新异常
- DbUpdateConcurrencyException：并发冲突异常
- InvalidOperationException：操作无效异常
- NotSupportedException：不支持的操作异常

这些异常将被服务层捕获并进行相应处理。

## 7. 性能优化策略
- 使用索引优化查询性能
- 实现缓存机制减少数据库访问
- 优化批量操作
- 实现异步操作提高响应性能
- 使用存储过程处理复杂查询