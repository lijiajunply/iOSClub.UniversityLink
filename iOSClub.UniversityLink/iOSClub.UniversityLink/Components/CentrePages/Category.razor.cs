using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Components.CentrePages;

public partial class Category
{
    [Parameter] public string Key { get; set; } = "";

    private List<CategoryModel> Categories { get; set; } = [];
    private CategoryModel Model { get; set; } = new();
    private IconModel[] AllIcons { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var json = await File.ReadAllTextAsync("wwwroot/iconfont.json");
        AllIcons = JsonConvert.DeserializeObject<IconModel[]>(json) ?? [];
        Console.WriteLine(AllIcons.Length);
        await using var context = await DbFactory.CreateDbContextAsync();
        if (string.IsNullOrEmpty(Key))
        {
            Categories = await context.Categories
                .Include(x => x.Links.OrderBy(y => y.Index))
                .OrderBy(x => x.Index)
                .ToListAsync();
            return;
        }

        var model = await context.Categories
            .Include(x => x.Links.OrderBy(y => y.Index))
            .FirstOrDefaultAsync(x => x.Key == Key);
        Categories = [];
        if (model == null) return;

        Model = model;
        await base.OnInitializedAsync();
    }


    CategoryModel? _categoryModel;

    private async Task OnDrop(DragEventArgs e, CategoryModel s)
    {
        if (Member.Identity != "Founder" && Member.Identity != "Manager") return;
        if (string.IsNullOrEmpty(s.Key) || _categoryModel == null) return;
        await using var context = await DbFactory.CreateDbContextAsync();
        var index = Categories.IndexOf(s);
        if (index == -1) return;
        var previous = await context.Categories.FirstOrDefaultAsync(x => x.Key == _categoryModel.Key);
        if (previous == null) return;
        var next = await context.Categories.FirstOrDefaultAsync(x => x.Key == s.Key);
        if (next == null) return;
        next.Index = _categoryModel.Index;
        previous.Index = s.Index;
        (s.Index, _categoryModel.Index) = (_categoryModel.Index, s.Index);
        Categories = Categories.OrderBy(x => x.Index).ToList();
        _categoryModel = null;
        await context.SaveChangesAsync();
        StateHasChanged();
    }

    private void OnDragStart(DragEventArgs e, CategoryModel s)
    {
        e.DataTransfer.DropEffect = "move";
        e.DataTransfer.EffectAllowed = "move";
        _categoryModel = s;
    }

    LinkModel? _link;

    private async Task OnDrop(DragEventArgs e, LinkModel s)
    {
        if (Member.Identity != "Founder" && Member.Identity != "Manager") return;
        if (string.IsNullOrEmpty(s.Key) || _link == null) return;
        await using var context = await DbFactory.CreateDbContextAsync();
        var index = Model.Links.IndexOf(s);
        if (index == -1) return;
        var previous = await context.Links.FirstOrDefaultAsync(x => x.Key == _link.Key);
        if (previous == null) return;
        var next = await context.Links.FirstOrDefaultAsync(x => x.Key == s.Key);
        if (next == null) return;
        next.Index = _link.Index;
        previous.Index = s.Index;
        (s.Index, _link.Index) = (_link.Index, s.Index);
        Model.Links = Model.Links.OrderBy(x => x.Index).ToList();
        _link = null;
        await context.SaveChangesAsync();
        StateHasChanged();
    }

    private void OnDragStart(DragEventArgs e, LinkModel s)
    {
        e.DataTransfer.DropEffect = "move";
        e.DataTransfer.EffectAllowed = "move";
        _link = s;
    }

    private async Task Remove(LinkModel model)
    {
        if (Member.Identity != "Founder" && Member.Identity != "Manager") return;
        await using var context = await DbFactory.CreateDbContextAsync();
        context.Links.Remove(model);
        Model.Links.Remove(model);
        await context.SaveChangesAsync();
    }
    
    bool _addLinkVisible;
    private Form<LinkModel> _linkForm = new();
    private LinkModel Link { get; set; } = new();
    private string _categoryKey { get; set; } = "";

    private void ShowLinkModal(LinkModel? link = null, string? key = null)
    {
        if (Member.Identity != "Founder" && Member.Identity != "Manager") return;
        Link = link ?? new LinkModel();
        if (!string.IsNullOrEmpty(key)) _categoryKey = key;
        _addLinkVisible = true;
    }

    private void LinkHandleOk()
    {
        _linkForm.Submit();
    }

    private async Task LinkOnFinish()
    {
        if (Member.Identity != "Founder" && Member.Identity != "Manager") return;
        await using var context = await DbFactory.CreateDbContextAsync();
        var key = string.IsNullOrEmpty(Key) ? _categoryKey : Key;
        if (string.IsNullOrEmpty(key)) return;
        var category = await context.Categories.Include(x => x.Links)
            .FirstOrDefaultAsync(x => x.Key == key);
        if (category == null) return;
        if (string.IsNullOrEmpty(Link.Key))
        {
            Link.Key = Link.ToString();
            category.Links.Add(Link);
            Model.Links.Add(Link);
        }
        else
        {
            var link = await context.Links.FirstOrDefaultAsync(x => x.Key == Link.Key);
            if (link == null) return;
            link.Name = Link.Name;
            link.Description = Link.Description;
            link.Url = Link.Url;
            link.Icon = Link.Icon;
        }

        await context.SaveChangesAsync();
        _addModelVisible = false;
        _categoryKey = "";
    }
    
    [CascadingParameter] public UserModel Member { get; set; } = new();

    bool _addModelVisible;
    private Form<CategoryModel> _form = new();

    private void ShowModal()
    {
        if (Member.Identity != "Founder" && Member.Identity != "Manager") return;
        _addModelVisible = true;
    }

    private void HandleOk()
    {
        _form.Submit();
    }

    private async Task OnFinish()
    {
        if (Member.Identity != "Founder" && Member.Identity != "Manager") return;
        await using var context = await DbFactory.CreateDbContextAsync();
        if (string.IsNullOrEmpty(Model.Key))
        {
            Model.Key = Model.ToString();
            await context.Categories.AddAsync(Model);
            Categories.Add(Model);
        }
        else
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Key == Model.Key);
            if (category == null) return;
            category.Name = Model.Name;
            category.Description = Model.Description;
            category.Icon = Model.Icon;
        }

        await context.SaveChangesAsync();
        _addModelVisible = false;
    }

}