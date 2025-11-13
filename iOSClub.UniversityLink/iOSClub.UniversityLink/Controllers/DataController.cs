using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using iOSClub.UniversityLink.Models;
using UniversityLink.DataModels;
using Newtonsoft.Json;

namespace iOSClub.UniversityLink.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController(IDbContextFactory<LinkContext> dbFactory, IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    private readonly IDbContextFactory<LinkContext> _dbFactory = dbFactory;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    // GET: api/data/download
    [HttpGet("download")]
    [Authorize]
    public async Task<IActionResult> JsonDownload(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _dbFactory.CreateDbContextAsync(cancellationToken);
            
            // 获取当前用户身份（从JWT token或其他身份验证机制中）
            var identity = User.FindFirst("role")?.Value ?? "Member";
            
            if (identity == "Member")
            {
                // 普通成员只下载Markdown格式的链接数据
                var categories = await context.Categories
                    .Include(x => x.Links.OrderBy(y => y.Index))
                    .OrderBy(x => x.Index)
                    .ToListAsync(cancellationToken);

                var builder = new StringBuilder();
                foreach (var category in categories)
                {
                    builder.AppendLine($"## {category.Name}");
                    builder.AppendLine("");
                    builder.AppendLine(category.Description);
                    builder.AppendLine("");
                    foreach (var link in category.Links)
                    {
                        builder.AppendLine($"- [{link.Name}]({link.Url} \"{link.Description}\")");
                    }
                    builder.AppendLine("");
                }

                var markdownContent = builder.ToString();
                var markdownBytes = Encoding.UTF8.GetBytes(markdownContent);
                
                return File(markdownBytes, "text/markdown", "links.md");
            }
            else
            {
                // 管理员或其他角色下载完整的JSON数据
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };

                var allData = new AllDataModel
                {
                    Users = await context.Users.ToListAsync(cancellationToken),
                    Categories = await context.Categories
                        .Include(x => x.Links)
                        .ToListAsync(cancellationToken)
                };
                
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(allData, options);
                var jsonBytes = Encoding.UTF8.GetBytes(jsonContent);
                
                return File(jsonBytes, "application/json", "allData.json");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "下载数据失败", error = ex.Message });
        }
    }

    // POST: api/data/upload
    [HttpPost("upload")]
    [Authorize(Roles = "Founder,Manager")]
    public async Task<IActionResult> UploadFiles([FromForm] IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "请选择要上传的文件" });
            }

            // 读取文件内容
            using var streamReader = new StreamReader(file.OpenReadStream());
            var content = await streamReader.ReadToEndAsync(cancellationToken);
            
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest(new { message = "上传的文件内容为空" });
            }

            await using var context = await _dbFactory.CreateDbContextAsync(cancellationToken);

            try
            {
                // 反序列化数据
                var allData = JsonConvert.DeserializeObject<AllDataModel>(content);
                
                if (allData == null)
                {
                    return BadRequest(new { message = "无法解析JSON数据" });
                }

                // 导入用户数据
                foreach (var user in allData.Users)
                {
                    if (!await context.Users.AnyAsync(x => x.UserId == user.UserId, cancellationToken))
                    {
                        await context.Users.AddAsync(user, cancellationToken);
                    }
                }
                
                await context.SaveChangesAsync(cancellationToken);

                // 导入分类和链接数据
                foreach (var category in allData.Categories)
                {
                    var existingCategory = await context.Categories
                        .Include(x => x.Links)
                        .FirstOrDefaultAsync(x => x.Name == category.Name, cancellationToken);
                    
                    if (existingCategory != null)
                    {
                        foreach (var link in category.Links)
                        {
                            var existingLink = await context.Links
                                .FirstOrDefaultAsync(x => x.Key == link.Key, cancellationToken);
                            
                            if (existingLink != null)
                            {
                                // 更新现有链接
                                existingLink.Description = link.Description;
                                existingLink.Url = link.Url;
                                existingLink.Name = link.Name;
                                existingLink.Icon = link.Icon;
                                existingLink.Index = link.Index;
                            }
                            else
                            {
                                // 添加新链接
                                existingCategory.Links.Add(link);
                            }
                        }
                    }
                    else
                    {
                        // 添加新分类
                        await context.Categories.AddAsync(category, cancellationToken);
                    }
                    
                    await context.SaveChangesAsync(cancellationToken);
                }

                return Ok(new { message = "数据导入成功" });
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return BadRequest(new { message = "JSON格式错误" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "上传数据失败", error = ex.Message });
        }
    }
}