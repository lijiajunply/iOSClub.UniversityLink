# 控制器层(Controllers)设计方案

## 1. 设计原则
- 控制器层负责处理HTTP请求，返回HTTP响应
- 控制器应该依赖于服务层接口，而不是直接操作数据上下文
- 遵循RESTful API设计规范
- 实现完整的CRUD操作
- 添加适当的错误处理和验证

## 2. 控制器列表

### 2.1 CategoryController
处理分类相关的API请求

#### API端点
- `GET /api/category` - 获取所有分类（包含链接）
- `GET /api/category/{key}` - 根据Key获取指定分类
- `POST /api/category` - 创建新分类
- `PUT /api/category/{key}` - 更新分类
- `DELETE /api/category/{key}` - 删除分类
- `PUT /api/category/{key}/reorder` - 重新排序分类

### 2.2 LinkController
处理链接相关的API请求（扩展现有控制器）

#### API端点
- `GET /api/link` - 获取所有链接
- `GET /api/link/{key}` - 根据Key获取指定链接
- `POST /api/link` - 创建新链接
- `PUT /api/link/{key}` - 更新链接
- `DELETE /api/link/{key}` - 删除链接
- `PUT /api/link/{key}/move` - 移动链接到其他分类
- `PUT /api/link/{key}/reorder` - 重新排序链接

### 2.3 UserController
处理用户相关的API请求

#### API端点
- `GET /api/user` - 获取所有用户
- `GET /api/user/{userId}` - 根据ID获取指定用户
- `POST /api/user` - 创建新用户
- `PUT /api/user/{userId}` - 更新用户信息
- `DELETE /api/user/{userId}` - 删除用户
- `POST /api/user/login` - 用户登录
- `GET /api/user/profile` - 获取当前登录用户信息

## 3. 接口设计

### 3.1 CategoryController 接口
```csharp
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryModel>>> GetAllCategories() { ... }
    
    [HttpGet("{key}")]
    public async Task<ActionResult<CategoryModel>> GetCategory(string key) { ... }
    
    [HttpPost]
    public async Task<ActionResult<CategoryModel>> CreateCategory(CategoryModel category) { ... }
    
    [HttpPut("{key}")]
    public async Task<ActionResult<CategoryModel>> UpdateCategory(string key, CategoryModel category) { ... }
    
    [HttpDelete("{key}")]
    public async Task<ActionResult> DeleteCategory(string key) { ... }
    
    [HttpPut("{key}/reorder")]
    public async Task<ActionResult> ReorderCategory(string key, int newIndex) { ... }
}
```

### 3.2 LinkController 接口
```csharp
[Route("api/[controller]")]
[ApiController]
public class LinkController : ControllerBase
{
    private readonly ILinkService _linkService;
    
    public LinkController(ILinkService linkService)
    {
        _linkService = linkService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LinkModel>>> GetAllLinks() { ... }
    
    [HttpGet("{key}")]
    public async Task<ActionResult<LinkModel>> GetLink(string key) { ... }
    
    [HttpPost]
    public async Task<ActionResult<LinkModel>> CreateLink(LinkModel link, string categoryKey) { ... }
    
    [HttpPut("{key}")]
    public async Task<ActionResult<LinkModel>> UpdateLink(string key, LinkModel link) { ... }
    
    [HttpDelete("{key}")]
    public async Task<ActionResult> DeleteLink(string key) { ... }
    
    [HttpPut("{key}/move")]
    public async Task<ActionResult> MoveLink(string key, string targetCategoryKey) { ... }
    
    [HttpPut("{key}/reorder")]
    public async Task<ActionResult> ReorderLink(string key, int newIndex) { ... }
}
```

### 3.3 UserController 接口
```csharp
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAllUsers() { ... }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserModel>> GetUser(string userId) { ... }
    
    [HttpPost]
    public async Task<ActionResult<UserModel>> CreateUser(UserModel user) { ... }
    
    [HttpPut("{userId}")]
    public async Task<ActionResult<UserModel>> UpdateUser(string userId, UserModel user) { ... }
    
    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteUser(string userId) { ... }
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginModel login) { ... }
    
    [HttpGet("profile")]
    public async Task<ActionResult<UserModel>> GetProfile() { ... }
}
```

## 4. 数据传输对象(DTOs)
为了更好的API设计，我们将创建以下DTOs：

### 4.1 CategoryDTO
```csharp
public class CategoryDTO
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public int Index { get; set; }
    public List<LinkDTO> Links { get; set; }
}
```

### 4.2 LinkDTO
```csharp
public class LinkDTO
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public int Index { get; set; }
}
```

### 4.3 UserDTO
```csharp
public class UserDTO
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Gender { get; set; }
    public string ClassName { get; set; }
    public string Identity { get; set; }
}
```

## 5. 错误处理
每个控制器方法都应该：
- 验证输入参数
- 处理异常情况
- 返回适当的HTTP状态码
- 提供清晰的错误消息

## 6. 权限控制
根据现有代码，需要实现基于用户角色的权限控制：
- Founder：创始人，可以执行所有操作
- Manager：管理员，可以执行大部分操作
- Member：普通成员，只能查看内容

在控制器层使用Authorize属性和自定义权限验证中间件实现。