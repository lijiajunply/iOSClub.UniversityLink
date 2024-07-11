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
    public async Task<ActionResult<List<LinkModel>>> Get()
    {
        return await context.Links.ToListAsync();
    }
}