using UniversityLink.DataModels;
using UniversityLink.DataModels.Repositories;

namespace iOSClub.UniversityLink.Services;

public class LinkService(IUnitOfWork unitOfWork) : ILinkService
{
    // 获取所有链接
    public async Task<List<LinkModel>> GetAllLinksAsync(CancellationToken cancellationToken = default)
    {
        return (await unitOfWork.Links.GetAllAsync(cancellationToken)).ToList();
    }

    // 按ID获取链接
    public async Task<LinkModel?> GetLinkByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Links.GetByKeyAsync(id, cancellationToken);
    }

    // 按Key获取链接
    public async Task<LinkModel?> GetLinkByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("链接Key不能为空");
        }

        var link = await unitOfWork.Links.GetByKeyAsync(key, cancellationToken);
        if (link == null)
        {
            throw new KeyNotFoundException($"链接Key '{key}' 不存在");
        }

        return link;
    }

    // 按分类获取链接
    public async Task<List<LinkModel>> GetLinksByCategoryAsync(string categoryId,
        CancellationToken cancellationToken = default)
    {
        var c = await unitOfWork.Categories.GetByKeyAsync(categoryId, true, cancellationToken);
        return c?.Links ?? [];
    }

    // 创建链接
    public async Task<LinkModel> CreateLinkAsync(LinkModel link, CancellationToken cancellationToken = default)
    {
        // 验证必填字段
        if (string.IsNullOrWhiteSpace(link.Name))
        {
            throw new ArgumentException("链接名称不能为空");
        }

        if (string.IsNullOrWhiteSpace(link.Url))
        {
            throw new ArgumentException("链接URL不能为空");
        }

        // 验证URL格式
        if (!Uri.IsWellFormedUriString(link.Url, UriKind.Absolute))
        {
            throw new ArgumentException("URL格式不正确");
        }

        // 设置默认值
        if (string.IsNullOrEmpty(link.Icon))
        {
            link.Icon = "link";
        }

        // LinkModel中没有IsExternal属性，移除相关设置

        // 创建链接
        return await unitOfWork.Links.CreateAsync(link, cancellationToken);
    }

    // 更新链接
    public async Task UpdateLinkAsync(LinkModel link, CancellationToken cancellationToken = default)
    {
        // 验证链接是否存在
        if (!await unitOfWork.Links.ExistsAsync(link.Key, cancellationToken))
        {
            throw new KeyNotFoundException($"链接Key '{link.Key}' 不存在");
        }

        // 验证必填字段
        if (string.IsNullOrWhiteSpace(link.Name))
        {
            throw new ArgumentException("链接名称不能为空");
        }

        if (string.IsNullOrWhiteSpace(link.Url))
        {
            throw new ArgumentException("链接URL不能为空");
        }

        // 验证URL格式
        if (!Uri.IsWellFormedUriString(link.Url, UriKind.Absolute))
        {
            throw new ArgumentException("URL格式不正确");
        }

        // 更新链接
        await unitOfWork.Links.UpdateAsync(link, cancellationToken);
    }

    // 删除链接
    public async Task DeleteLinkAsync(string id, CancellationToken cancellationToken = default)
    {
        // 检查链接是否存在
        if (!await unitOfWork.Links.ExistsAsync(id, cancellationToken))
        {
            throw new KeyNotFoundException($"链接Key '{id}' 不存在");
        }
        
        // 删除链接
        var result = await unitOfWork.Links.DeleteAsync(id, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"无法删除链接Key '{id}'");
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // 更新链接排序
    public async Task UpdateLinkSortAsync(string categoryId, List<string> linkIds,
        CancellationToken cancellationToken = default)
    {
        // 获取分类下的所有链接
        var links = (await unitOfWork.Links.GetByCategoryAsync(categoryId, cancellationToken)).ToList();
        
        // 创建一个字典来快速查找顺序
        var orderDict = linkIds.Select((id, index) => new { id, index })
                               .ToDictionary(x => x.id, x => x.index);
        
        // 更新链接的索引
        foreach (var link in links)
        {
            if (orderDict.TryGetValue(link.Key, out var index))
            {
                link.Index = index;
            }
        }
        
        // 批量更新链接
        await unitOfWork.Links.BulkUpdateAsync(links, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // 搜索链接
    public async Task<List<LinkModel>> SearchLinksAsync(string keyword, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllLinksAsync(cancellationToken);
        }

        // 在当前实现中，简单返回所有链接
        // 由于仓储没有Search方法，这里返回所有链接
        return await GetAllLinksAsync(cancellationToken);
    }
}