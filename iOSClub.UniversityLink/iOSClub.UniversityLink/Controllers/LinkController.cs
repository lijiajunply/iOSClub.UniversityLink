using System.Text;
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
            .ToListAsync();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<LinkModel>>> GetLink()
    {
        return await context.Links.ToListAsync();
    }
    
    [HttpGet]
    public async Task<ActionResult<string>> GetMd()
    {
        var c = await context.Categories
            .Include(x => x.Links.OrderBy(y => y.Index))
            .OrderBy(x => x.Index)
            .ToListAsync();
        
        var builder = new StringBuilder();
        foreach (var model in c)
        {
            builder.AppendLine($"## {model.Name}");
            builder.AppendLine("");
            builder.AppendLine(model.Description);
            builder.AppendLine("");
            foreach (var link in model.Links)
            {
                builder.AppendLine($"- [{link.Name}]({link.Url} \"{link.Description}\")");
            }
            builder.AppendLine("");
        }
        
        return builder.ToString();
    }
}