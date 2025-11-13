using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Services;

public interface ICategoryService
{
    // 获取所有分类
    Task<List<CategoryModel>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    
    // 按ID获取分类
    Task<CategoryModel?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
    
    // 创建分类
    Task<CategoryModel> CreateCategoryAsync(CategoryModel category, CancellationToken cancellationToken = default);
    
    // 更新分类
    Task UpdateCategoryAsync(CategoryModel category, CancellationToken cancellationToken = default);
    
    // 删除分类
    Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    
    // 更新分类排序
    Task UpdateCategorySortAsync(List<int> categoryIds, CancellationToken cancellationToken = default);
    
    // 搜索分类
    Task<List<CategoryModel>> SearchCategoriesAsync(string keyword, CancellationToken cancellationToken = default);
}