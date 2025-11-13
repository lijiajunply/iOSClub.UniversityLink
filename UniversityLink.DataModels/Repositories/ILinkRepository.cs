using System.Threading;
using System.Threading.Tasks;

namespace UniversityLink.DataModels.Repositories;

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