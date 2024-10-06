using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityLink.DataModels;

[Index(nameof(Index))]
public class LinkModel : DataModel
{
    [Key]
    [Column(TypeName = "varchar(32)")]
    public string Key { get; set; } = "";

    [Column(TypeName = "varchar(32)")] public string Name { get; set; } = "";
    [Column(TypeName = "varchar(32)")] public string? Icon { get; set; } = "";
    [Column(TypeName = "varchar(64)")] public string Url { get; set; } = "";
    [Column(TypeName = "varchar(32)")] public string? Description { get; set; } = "";
    public int Index { get; set; }
}

[Index(nameof(Index))]
public class CategoryModel : DataModel
{
    [Key]
    [Column(TypeName = "varchar(32)")]
    public string Key { get; set; } = "";

    [Column(TypeName = "varchar(32)")] public string Name { get; set; } = "";
    [Column(TypeName = "varchar(32)")] public string? Description { get; set; } = "";
    [Column(TypeName = "varchar(16)")] public string Icon { get; set; } = "";

    public int Index { get; set; }

    public List<LinkModel> Links { get; set; } = [];
}

public class UserModel
{
    [Column(TypeName = "varchar(32)")] public string UserName { get; set; } = "";

    [Key]
    [Column(TypeName = "varchar(32)")]
    public string UserId { get; init; } = "";

    [Column(TypeName = "varchar(2)")] public string Gender { get; init; } = "";
    [Column(TypeName = "varchar(20)")] public string ClassName { get; init; } = "";

    /// <summary>
    /// Founder : 创始人
    /// Manager : 管理者
    /// Member : 普通成员
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string Identity { get; set; } = "Member";

    public static Dictionary<string, string> IdentityDictionary => new()
    {
        ["Founder"] = "创始人",
        ["Manager"] = "管理员",
        ["Member"] = "普通成员"
    };
}

public class LoginModel
{
    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
}

[Serializable]
public record IconModel(string name, string font_class);