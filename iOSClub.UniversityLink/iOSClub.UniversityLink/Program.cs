using System.Text;
using iOSClub.UniversityLink;
using iOSClub.UniversityLink.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using NpgsqlDataProtection;
using UniversityLink.DataModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(o =>
    {
        o.MaxBufferedUnacknowledgedRenderBatches = 1024 * 1024 * 1024;
        o.DisconnectedCircuitMaxRetained = 1024;
    });

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
            //
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(o =>
    {
        o.DisableWebSocketCompression = true;
        o.ContentSecurityFrameAncestorsPolicy = "'none'";
    });

app.Run();