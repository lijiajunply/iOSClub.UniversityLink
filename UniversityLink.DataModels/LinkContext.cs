using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace UniversityLink.DataModels;

public sealed class LinkContext : DbContext
{
    public DbSet<LinkModel> Links { get; init; }
    public DbSet<CategoryModel> Categories { get; init; }
    public DbSet<UserModel> Users { get; init; }

    public LinkContext(DbContextOptions<LinkContext> options) : base(options)
    {
        if (!Database.GetPendingMigrations().Any()) return;
        try
        {
            Database.MigrateAsync();
        }
        catch
        {
            Database.EnsureCreatedAsync();
        }
    }
}

[Serializable]
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LinkContext>
{
    public LinkContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LinkContext>();
        optionsBuilder.UseSqlite("Data Source=Data.db");
        return new LinkContext(optionsBuilder.Options);
    }

    public static LinkContext Create(string s)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LinkContext>();
        optionsBuilder.UseNpgsql(s);
        return new LinkContext(optionsBuilder.Options);
    }
}

public static class DataTool
{
    public static string StringToHash(string s)
    {
        var data = Encoding.UTF8.GetBytes(s);
        var hash = MD5.HashData(data);
        var hashStringBuilder = new StringBuilder();
        foreach (var t in hash)
            hashStringBuilder.Append(t.ToString("x2"));
        return hashStringBuilder.ToString();
    }

    public static string GetProperties<T>(T t)
    {
        var builder = new StringBuilder();
        if (t == null) return builder.ToString();

        var properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        if (properties.Length <= 0) return builder.ToString();

        foreach (var item in properties)
        {
            var name = item.Name;
            var value = item.GetValue(t, null);
            if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
            {
                builder.Append($"{name}:{value ?? "null"},");
            }
        }

        return builder.ToString();
    }
}

public abstract class DataModel
{
    public override string ToString()
        => DataTool.StringToHash($"{GetType()} : {DataTool.GetProperties(this)}");
}