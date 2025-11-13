using System.Threading;
using System.Threading.Tasks;

namespace UniversityLink.DataModels.Repositories;

public interface ICategoryRepository
{
    // 获取所有分类
    public Task<IEnumerable<CategoryModel>> GetAllAsync(bool includeLinks = false, CancellationToken cancellationToken = default);
    
    // 根据Key获取分类
    public Task<CategoryModel?> GetByKeyAsync(string key, bool includeLinks = false, CancellationToken cancellationToken = default);
    
    // 根据名称获取分类
    public Task<CategoryModel?> GetByNameAsync(string name, bool includeLinks = false, CancellationToken cancellationToken = default);
    
    // 创建分类
    public Task<CategoryModel> CreateAsync(CategoryModel category, CancellationToken cancellationToken = default);
    
    // 更新分类
    public Task<CategoryModel> UpdateAsync(CategoryModel category, CancellationToken cancellationToken = default);
    
    // 删除分类
    public Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
    
    // 检查分类是否存在
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    
    // 获取分类总数
    public Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    // 批量更新分类
    public Task<int> BulkUpdateAsync(IEnumerable<CategoryModel> categories, CancellationToken cancellationToken = default);
}