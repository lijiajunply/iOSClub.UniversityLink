using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Services;

public interface ILinkService
{
    // 获取所有链接
    Task<List<LinkModel>> GetAllLinksAsync(CancellationToken cancellationToken = default);
    
    // 按ID获取链接
    Task<LinkModel?> GetLinkByIdAsync(int id, CancellationToken cancellationToken = default);
    
    // 按Key获取链接
    Task<LinkModel?> GetLinkByKeyAsync(string key, CancellationToken cancellationToken = default);
    
    // 按分类获取链接
    Task<List<LinkModel>> GetLinksByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    
    // 创建链接
    Task<LinkModel> CreateLinkAsync(LinkModel link, CancellationToken cancellationToken = default);
    
    // 更新链接
    Task UpdateLinkAsync(LinkModel link, CancellationToken cancellationToken = default);
    
    // 删除链接
    Task DeleteLinkAsync(int id, CancellationToken cancellationToken = default);
    
    // 更新链接排序
    Task UpdateLinkSortAsync(int categoryId, List<int> linkIds, CancellationToken cancellationToken = default);
    
    // 搜索链接
    Task<List<LinkModel>> SearchLinksAsync(string keyword, CancellationToken cancellationToken = default);
}