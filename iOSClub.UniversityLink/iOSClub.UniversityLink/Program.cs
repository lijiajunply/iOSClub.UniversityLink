using System.Text;
using iOSClub.UniversityLink;
using iOSClub.UniversityLink.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UniversityLink.DataModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<AuthenticationStateProvider, Provider>();

builder.Services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false, //是否验证Issuer
            ValidateAudience = false, //是否验证Audience
            ValidateIssuerSigningKey = true, //是否验证SecurityKey
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)), //SecurityKey
            ValidateLifetime = false, //是否验证失效时间
        };
    });

builder.Services.AddAuthorizationCore();

var sql = Environment.GetEnvironmentVariable("SQL", EnvironmentVariableTarget.Process);
if (string.IsNullOrEmpty(sql))
{
    builder.Services.AddDbContextFactory<LinkContext>(opt =>
        opt.UseSqlite("Data Source=Data.db",
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
}
else
{
    builder.Services.AddDbContextFactory<LinkContext>(opt =>
        opt.UseNpgsql(sql,
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LinkContext>();
    
    try
    {
        context.Database.Migrate();
    }
    catch (Exception e)
    {
        context.Database.EnsureCreated();
        Console.WriteLine(e.Message);
    }
    
    // if (!context.Categories.Any())
    // {
    //     var xauat = new CategoryModel() { Name = "建大生活", Description = "西建大校园网站", Icon = "fangyuan" };
    //     var ios = new CategoryModel()
    //     {
    //         Name = "社团出品", Description = "iOS Club 出品的App",
    //         Icon = "pingguo"
    //     };
    //     var other = new CategoryModel() { Name = "其他", Description = "其他资源", Icon = "gengduo1" };
    //     var recommend = new CategoryModel() { Name = "校友推荐", Description = "推荐资源", Icon = "shoucang2" };
    //     var test = new CategoryModel() { Name = "考试学习", Description = "考试和学习资料", Icon = "wodexuexi" };
    //     var neighborhood = new CategoryModel() { Name = "校园周边", Description = "建大周边", Icon = "jiejiarifanxiao" };
    //     var tool = new CategoryModel { Name = "在线工具", Description = "在线工具", Icon = "gongju" };
    //     xauat.Key = xauat.ToString();
    //     ios.Key = ios.ToString();
    //     other.Key = other.ToString();
    //     recommend.Key = recommend.ToString();
    //     neighborhood.Key = neighborhood.ToString();
    //     tool.Key = tool.ToString();
    //     test.Key = test.ToString();
    //     var list = new List<CategoryModel>()
    //         { xauat, test, neighborhood, recommend, ios, tool, other };
    //     for (var i = 0; i < list.Count; i++)
    //     {
    //         var model = list[i];
    //         model.Index = i;
    //         await context.Categories.AddAsync(model);
    //         await context.SaveChangesAsync();
    //     }
    // }

    // if(context.Users.Any())
    // {
    //     var list = await context.Users.Where(x => x.Identity == "Department").ToListAsync();
    //     foreach (var item in list)
    //     {
    //         item.Identity = "Member";
    //         await context.SaveChangesAsync();
    //     }
    // }

    // if (context.Users.Any())
    // {
    //     var sqlConversion = Environment.GetEnvironmentVariable("CONVERSION", EnvironmentVariableTarget.Process);
    //     if (!string.IsNullOrEmpty(sqlConversion))
    //     {
    //         var newContext = DesignTimeDbContextFactory.Create(sqlConversion);
    //         try
    //         {
    //             newContext.Database.Migrate();
    //         }
    //         catch (Exception e)
    //         {
    //             newContext.Database.EnsureCreated();
    //             Console.WriteLine(e.Message);
    //         }
    //
    //         if (!newContext.Users.Any())
    //         {
    //             await newContext.Users.AddRangeAsync(context.Users);
    //
    //             await newContext.SaveChangesAsync();
    //
    //             foreach (var category in context.Categories.Include(categoryModel => categoryModel.Links))
    //             {
    //                 await newContext.Categories.AddAsync(category);
    //                 await newContext.SaveChangesAsync();
    //             }
    //         }
    //     }
    //
    //     
    // }
    await context.SaveChangesAsync();
    await context.DisposeAsync();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthorization();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();