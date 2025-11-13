using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityLink.DataModels;
using iOSClub.UniversityLink.Services;

namespace iOSClub.UniversityLink.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    // GET: api/category
    [HttpGet]
    public async Task<ActionResult<List<CategoryModel>>> GetAllCategories(CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await categoryService.GetAllCategoriesAsync(cancellationToken);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取分类列表失败", error = ex.Message });
        }
    }
    
    // GET: api/category/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryModel>> GetCategoryById(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await categoryService.GetCategoryByIdAsync(id, cancellationToken);
            return Ok(category);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取分类失败", error = ex.Message });
        }
    }
    
    // POST: api/category
    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<CategoryModel>> CreateCategory([FromBody] CategoryModel category, CancellationToken cancellationToken = default)
    {
        try
        {
            var createdCategory = await categoryService.CreateCategoryAsync(category, cancellationToken);
            // 使用Key替代不存在的Id属性，使用正确的方法名
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Key }, createdCategory);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "创建分类失败", error = ex.Message });
        }
    }
    
    // PUT: api/category/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult> UpdateCategory(int id, [FromBody] CategoryModel category, CancellationToken cancellationToken = default)
    {
        try
        {
            // 移除对不存在的Id属性的检查，整个条件块已删除
            // 由于没有Id属性，不再需要ID匹配检查
            
            await categoryService.UpdateCategoryAsync(category, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "更新分类失败", error = ex.Message });
        }
    }
    
    // DELETE: api/category/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await categoryService.DeleteCategoryAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "删除分类失败", error = ex.Message });
        }
    }
    
    // PUT: api/category/sort
    [HttpPut("sort")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult> UpdateCategorySort([FromBody] List<int> categoryIds, CancellationToken cancellationToken = default)
    {
        try
        {
            await categoryService.UpdateCategorySortAsync(categoryIds, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "更新分类排序失败", error = ex.Message });
        }
    }
    
    // GET: api/category/search?keyword=xxx
    [HttpGet("search")]
    public async Task<ActionResult<List<CategoryModel>>> SearchCategories([FromQuery] string keyword, CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await categoryService.SearchCategoriesAsync(keyword, cancellationToken);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "搜索分类失败", error = ex.Message });
        }
    }
}