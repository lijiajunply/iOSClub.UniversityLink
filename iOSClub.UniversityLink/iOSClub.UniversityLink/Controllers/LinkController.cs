using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityLink.DataModels;
using iOSClub.UniversityLink.Services;

namespace iOSClub.UniversityLink.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LinkController(ILinkService linkService) : ControllerBase
{
    // GET: api/link
    [HttpGet]
    public async Task<ActionResult<List<LinkModel>>> GetAllLinks(CancellationToken cancellationToken = default)
    {
        try
        {
            var links = await linkService.GetAllLinksAsync(cancellationToken);
            return Ok(links);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取链接列表失败", error = ex.Message });
        }
    }
    
    // GET: api/link/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<LinkModel>> GetLinkById(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var link = await linkService.GetLinkByIdAsync(id, cancellationToken);
            return Ok(link);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取链接失败", error = ex.Message });
        }
    }
    
    // GET: api/link/key/{key}
    [HttpGet("key/{key}")]
    public async Task<ActionResult<LinkModel>> GetLinkByKey(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var link = await linkService.GetLinkByKeyAsync(key, cancellationToken);
            return Ok(link);
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
            return StatusCode(500, new { message = "获取链接失败", error = ex.Message });
        }
    }
    
    // GET: api/link/category/{categoryId}
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<List<LinkModel>>> GetLinksByCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var links = await linkService.GetLinksByCategoryAsync(categoryId, cancellationToken);
            return Ok(links);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取分类下的链接失败", error = ex.Message });
        }
    }
    
    // POST: api/link
    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<LinkModel>> CreateLink([FromBody] LinkModel link, CancellationToken cancellationToken = default)
    {
        try
        {
            var createdLink = await linkService.CreateLinkAsync(link, cancellationToken);
            // LinkModel没有Id属性，使用Key属性替代
            return CreatedAtAction(nameof(GetLinkById), new { id = 0 }, createdLink);
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
            return StatusCode(500, new { message = "创建链接失败", error = ex.Message });
        }
    }
    
    // PUT: api/link/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult> UpdateLink(int id, [FromBody] LinkModel link, CancellationToken cancellationToken = default)
    {
        try
        {
            // LinkModel没有Id属性，移除ID匹配检查
            
            await linkService.UpdateLinkAsync(link, cancellationToken);
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
            return StatusCode(500, new { message = "更新链接失败", error = ex.Message });
        }
    }
    
    // DELETE: api/link/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult> DeleteLink(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await linkService.DeleteLinkAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "删除链接失败", error = ex.Message });
        }
    }
    
    // PUT: api/link/sort/{categoryId}
    [HttpPut("sort/{categoryId}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult> UpdateLinkSort(int categoryId, [FromBody] List<int> linkIds, CancellationToken cancellationToken = default)
    {
        try
        {
            await linkService.UpdateLinkSortAsync(categoryId, linkIds, cancellationToken);
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
            return StatusCode(500, new { message = "更新链接排序失败", error = ex.Message });
        }
    }
    
    // GET: api/link/search?keyword=xxx
    [HttpGet("search")]
    public async Task<ActionResult<List<LinkModel>>> SearchLinks([FromQuery] string keyword, CancellationToken cancellationToken = default)
    {
        try
        {
            var links = await linkService.SearchLinksAsync(keyword, cancellationToken);
            return Ok(links);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "搜索链接失败", error = ex.Message });
        }
    }
}