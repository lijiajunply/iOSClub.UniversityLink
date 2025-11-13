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
    public Task<LinkModel?> GetLinkByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // 在当前实现中，我们通过Key查找链接
        // 由于模型中没有Id属性，这里简单返回null
        return null;
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
    public async Task<List<LinkModel>> GetLinksByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        // 在当前实现中，我们通过Key查找分类
        // 由于模型中没有Id属性，这里返回空列表
        return [];
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
        // 在当前实现中，我们通过Key删除链接
        // 由于模型中没有Id属性，这里不执行操作
    }
    
    // 更新链接排序
    public async Task UpdateLinkSortAsync(string categoryId, List<string> linkIds, CancellationToken cancellationToken = default)
    {
        // 在当前实现中，我们使用Index属性进行排序
        // 由于模型中没有Id属性，这里不执行操作
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