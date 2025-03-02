using System.Text;
using iOSClub.UniversityLink;
using iOSClub.UniversityLink.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            ).ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)) // <--- This line ✨
            .EnableDetailedErrors());
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

    if (context.Database.GetPendingMigrations().Any())
    {
        await context.Database.MigrateAsync();
    }


    await context.SaveChangesAsync();
    await context.DisposeAsync();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthorization();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(o => o.ContentSecurityFrameAncestorsPolicy = "'none'");

app.Run();