using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityLink.DataModels;
using iOSClub.UniversityLink.Services;

namespace iOSClub.UniversityLink.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    // GET: api/user
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UserModel>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取用户列表失败", error = ex.Message });
        }
    }

    // GET: api/user/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserModel>> GetUserById(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            // 普通用户只能查看自己的信息
            if (!User.IsInRole("Admin"))
            {
                // 从JWT中获取当前用户ID进行验证
                var currentUserId = User.FindFirst("userId")?.Value ?? "0";
                if (currentUserId != id)
                {
                    return Forbid();
                }
            }

            // GetUserByIdAsync接受int类型的id
            var user = await userService.GetUserByIdAsync(id, cancellationToken);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取用户失败", error = ex.Message });
        }
    }

    // GET: api/user/username/{username}
    [HttpGet("username/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserModel>> GetUserByUsername(string username,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userService.GetUserByUsernameAsync(username, cancellationToken);

            if (user == null)
            {
                return NotFound(new { message = $"用户名 '{username}' 不存在" });
            }

            // 普通用户只能查看自己的信息
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = User.FindFirst("userId")?.Value ?? "0";
                if (currentUserId != user.UserId)
                {
                    return Forbid();
                }
            }

            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "获取用户失败", error = ex.Message });
        }
    }

    // POST: api/user
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserModel>> CreateUser([FromBody] UserModel user,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var createdUser = await userService.CreateUserAsync(user, "", cancellationToken);
            
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "创建用户失败", error = ex.Message });
        }
    }

    // PUT: api/user/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] UserModel user,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 普通用户只能更新自己的信息
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = User.FindFirst("userId")?.Value ?? "0";
                if (currentUserId != id)
                {
                    return Forbid();
                }
            }

            // 验证用户是否存在
            var existingUser = await userService.GetUserByIdAsync(id, cancellationToken);

            if (existingUser == null)
            {
                return NotFound(new { message = $"用户ID '{id}' 不存在" });
            }

            await userService.UpdateUserAsync(user, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "更新用户失败", error = ex.Message });
        }
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await userService.DeleteUserAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "删除用户失败", error = ex.Message });
        }
    }

    // PUT: api/user/{id}/password
    [HttpPut("{id}/password")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdatePassword(string id, [FromBody] PasswordUpdateModel passwordUpdate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 验证是否是用户自己或管理员在更新密码
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = User.FindFirst("userId")?.Value ?? "0";
                if (currentUserId != id)
                {
                    return Forbid();
                }
            }

            // 根据接口定义，UpdatePasswordAsync只有3个参数，移除oldPassword参数
            await userService.UpdatePasswordAsync(id, passwordUpdate.NewPassword, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "更新密码失败", error = ex.Message });
        }
    }
}

// 密码更新模型
public class PasswordUpdateModel
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}