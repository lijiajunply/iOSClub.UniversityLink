using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class LinkController(LinkContext context) : ControllerBase
{
    // GET: api/<LinkController>
    [HttpGet]
    public async Task<ActionResult<List<CategoryModel>>> GetCategory()
    {
        return await context.Categories
            .Include(x => x.Links)
            .OrderBy(x => x.Index)
            .ToListAsync();
    }

    [HttpGet]
    public async Task<ActionResult<List<LinkModel>>> GetLink()
    {
        return await context.Links.ToListAsync();
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<CategoryModel>> GetCategory(string name)
    {
        var a = await context.Categories
            .Include(x => x.Links)
            .FirstOrDefaultAsync(x => x.Name == name);
        if (a == null) return NotFound();
        return a;
    }
}