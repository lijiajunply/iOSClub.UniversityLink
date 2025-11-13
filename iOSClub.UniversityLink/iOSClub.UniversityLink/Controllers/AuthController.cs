using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iOSClub.UniversityLink.Services;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Controllers;

// 请求和响应模型定义
[Serializable]
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[Serializable]
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

[Serializable]
public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    // POST: api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 生成令牌 - 我们假设有一个验证方法
            var isValidUser =
                await authService.ValidateCredentialsAsync(request.Username, request.Password, cancellationToken);
            if (!isValidUser)
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }

            // 生成JWT令牌
            var token = await authService.GenerateTokenAsync(request.Username, cancellationToken);

            return Ok(new TokenResponse
            {
                AccessToken = token,
                TokenType = "Bearer"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "登录失败", error = ex.Message });
        }
    }

    // POST: api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 创建用户对象
            var user = new UserModel
            {
                UserName = request.Username,
                // 这里假设UserId会在CreateUserAsync中生成或处理
                Identity = "Member" // 默认为普通成员
            };

            // 创建用户
            await userService.CreateUserAsync(user, request.Password, cancellationToken);

            return Ok(new { message = "注册成功" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "注册失败", error = ex.Message });
        }
    }

    // POST: api/auth/refresh-token
    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<ActionResult<TokenResponse>> RefreshToken(CancellationToken cancellationToken = default)
    {
        try
        {
            // 从当前令牌中获取用户名
            var username = authService.GetUsernameFromToken(HttpContext.Request.Headers.Authorization.ToString()
                .Replace("Bearer ", string.Empty));

            // 检查用户名是否为空
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "用户名不能为空" });
            }

            // 生成新的令牌
            var newToken = await authService.GenerateTokenAsync(username, cancellationToken);

            return Ok(new TokenResponse
            {
                AccessToken = newToken,
                TokenType = "Bearer"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "刷新令牌失败", error = ex.Message });
        }
    }

    // GET: api/auth/validate-token
    [HttpGet("validate-token")]
    [Authorize]
    public ActionResult<TokenValidationResponse> ValidateToken()
    {
        try
        {
            // 从Authorization头获取令牌
            var authHeader = HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "无效的授权头" });
            }

            var token = authHeader.Replace("Bearer ", string.Empty);

            // 验证令牌
            var isValid = authService.ValidateToken(token);

            if (!isValid)
            {
                return Unauthorized(new { message = "令牌无效" });
            }

            // 获取令牌中的用户名和角色信息
            var username = authService.GetUsernameFromToken(token);
            var role = User.FindFirst("role")?.Value ?? string.Empty;
            var userId = User.FindFirst("userId")?.Value ?? string.Empty;

            return Ok(new TokenValidationResponse
            {
                IsValid = true,
                Username = username,
                Role = role,
                UserId = userId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "验证令牌失败", error = ex.Message });
        }
    }

    // GET: api/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        try
        {
            // 对于JWT，服务端不需要特殊处理，客户端删除令牌即可
            // 这里可以记录用户登出日志等
            return Ok(new { message = "登出成功" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "登出失败", error = ex.Message });
        }
    }
}

// 添加TokenValidationResponse类定义
[Serializable]
public class TokenValidationResponse
{
    public bool IsValid { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? UserId { get; set; }
}