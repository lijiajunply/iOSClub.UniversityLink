using Microsoft.EntityFrameworkCore;
using UniversityLink.DataModels;
using UniversityLink.DataModels.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iOSClub.UniversityLink.Services.Repositories;

public class CategoryRepository(LinkContext context) : ICategoryRepository
{
    // 获取所有分类
    public async Task<IEnumerable<CategoryModel>> GetAllAsync(bool includeLinks = false, CancellationToken cancellationToken = default)
    {
        var query = context.Categories.AsQueryable();
        
        if (includeLinks)
        {
            query = query.Include(c => c.Links.OrderBy(l => l.Index));
        }
        
        return await query.OrderBy(c => c.Index).ToListAsync(cancellationToken);
    }
    
    // 根据Key获取分类
    public async Task<CategoryModel?> GetByKeyAsync(string key, bool includeLinks = false, CancellationToken cancellationToken = default)
    {
        var query = context.Categories.AsQueryable();
        
        if (includeLinks)
        {
            query = query.Include(c => c.Links.OrderBy(l => l.Index));
        }
        
        return await query.FirstOrDefaultAsync(c => c.Key == key, cancellationToken);
    }
    
    // 根据名称获取分类
    public async Task<CategoryModel?> GetByNameAsync(string name, bool includeLinks = false, CancellationToken cancellationToken = default)
    {
        var query = context.Categories.AsQueryable();
        
        if (includeLinks)
        {
            query = query.Include(c => c.Links.OrderBy(l => l.Index));
        }
        
        return await query.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }
    
    // 创建分类
    public async Task<CategoryModel> CreateAsync(CategoryModel category, CancellationToken cancellationToken = default)
    {
        // 设置排序值为当前最大排序值+1
        var maxSort = await context.Categories.MaxAsync(c => (int?)c.Index, cancellationToken) ?? 0;
        category.Index = maxSort + 1;

        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return category;
    }
    
    // 更新分类
    public async Task<CategoryModel> UpdateAsync(CategoryModel category, CancellationToken cancellationToken = default)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }
    
    // 删除分类
    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var category = await GetByKeyAsync(key, true, cancellationToken);
        if (category == null)
            return false;

        // 删除分类下的所有链接
        if (category.Links != null)
        {
            context.Links.RemoveRange(category.Links);
        }

        // 删除分类
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    // 检查分类是否存在
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await context.Categories.AnyAsync(c => c.Key == key, cancellationToken);
    }
    
    // 获取分类总数
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories.CountAsync(cancellationToken);
    }
    
    // 批量更新分类
    public async Task<int> BulkUpdateAsync(IEnumerable<CategoryModel> categories, CancellationToken cancellationToken = default)
    {
        var categoryList = categories.ToList();
        context.Categories.UpdateRange(categoryList);
        return await context.SaveChangesAsync(cancellationToken);
    }
}