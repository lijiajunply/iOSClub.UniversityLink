using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LinkController(LinkContext context) : ControllerBase
{
    // GET: api/<LinkController>
    [HttpGet]
    public async Task<ActionResult<List<CategoryModel>>> Get()
    {
        return await context.Categories
            .Include(x => x.Links)
            .ToListAsync();
    }
}