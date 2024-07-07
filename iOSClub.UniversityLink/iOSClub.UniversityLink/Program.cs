using System.Text;
using iOSClub.UniversityLink.Components;
using iOSClub.UniversityLink.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UniversityLink.DataModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, Provider>();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

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

builder.Services.AddSingleton(new JwtHelper(builder.Configuration));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<TokenActionFilter>();
builder.Services.AddOptions();
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
    if (!context.Users.Any())
    {
        var user = Environment.GetEnvironmentVariable("USER", EnvironmentVariableTarget.Process);
        var model = new UserModel() { UserName = "root", UserId = "0000000000", Identity = "Founder" };
        var users = user?.Split(',');
        if (!string.IsNullOrEmpty(user) && users != null)
        {
            if (users.Length > 0)
                model.UserName = users[0];
            if (users.Length > 1)
                model.UserId = users[1];
        }

        context.Users.Add(model);
    }

    if (!context.Categories.Any())
    {
        
    }

    context.SaveChanges();
    context.Dispose();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthorization();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(iOSClub.UniversityLink.Client._Imports).Assembly);

app.Run();