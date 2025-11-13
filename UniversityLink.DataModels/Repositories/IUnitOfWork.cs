using System.Threading;
using System.Threading.Tasks;

namespace UniversityLink.DataModels.Repositories;

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