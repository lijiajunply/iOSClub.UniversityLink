using System.Text;
using iOSClub.UniversityLink;
using iOSClub.UniversityLink.Models;
using iOSClub.UniversityLink.Services.Repositories;
using iOSClub.UniversityLink.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NpgsqlDataProtection;
using UniversityLink.DataModels;
using UniversityLink.DataModels.Repositories;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<AuthenticationStateProvider, Provider>();

// 注册仓储层实现
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 注册服务层实现
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 注册密码哈希器
builder.Services.AddSingleton<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();

// 配置JWT认证
builder.Services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false, //是否验证Issuer
            ValidateAudience = false, //是否验证Audience
            ValidateIssuerSigningKey = true, //是否验证SecurityKey
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ??
                                           "UniversityLinkSecretKey")), //SecurityKey
            ValidateLifetime = true, //启用过期时间验证
            ClockSkew = TimeSpan.Zero //不允许时间偏差
        };

        // 配置JWT事件
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }

                return Task.CompletedTask;
            }
        };
    });

// 配置授权策略
builder.Services.AddAuthorizationBuilder()
    // 配置授权策略
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build())
    // 配置授权策略
    .AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"))
    // 配置授权策略
    .AddPolicy("UserPolicy", policy =>
        policy.RequireRole("Admin", "User"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                origin.EndsWith(".zeabur.app") || // 支持所有 zeabur.app 子域名
                origin.EndsWith(".xauat.site") || // 支持所有 xauat.site 子域名
                origin.StartsWith("http://localhost")) // 支持本地开发环境
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // 如果需要发送凭据（如cookies、认证头等）
    });
});

var sql = Environment.GetEnvironmentVariable("SQL", EnvironmentVariableTarget.Process);
if (string.IsNullOrEmpty(sql))
{
    builder.Services.AddDbContextFactory<LinkContext>(opt =>
        opt.UseSqlite("Data Source=Data.db",
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo("./keys"));
}
else
{
    builder.Services.AddDbContextFactory<LinkContext>(opt =>
        opt.UseNpgsql(sql,
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        ));

    builder.Services.AddDataProtection()
        .PersistKeysToPostgres(sql, true);
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
    //app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LinkContext>();

    if (context.Database.GetPendingMigrations().Any())
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch
        {
            await context.Database.EnsureCreatedAsync();
        }
    }

    await context.SaveChangesAsync();
    await context.DisposeAsync();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthorization();
app.MapControllers();
app.UseCors();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();