using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Models;

public static class ProviderExtensions
{
    public static async Task Logout(this Provider provider)
    {
        await provider.UpdateAuthState(null);
    }
}

public class Provider(IJSRuntime js, IConfiguration configuration) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // 从本地存储或 Cookie 中获取 JWT 令牌
        string token;
        try
        {
            token = await js.InvokeAsync<string>("localStorageHelper.getItem", "jwt");
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }

        if (string.IsNullOrEmpty(token))
            return await Task.FromResult(new AuthenticationState(_anonymous));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, "Jwt");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public async Task UpdateAuthState(UserModel? user)
    {
        ClaimsPrincipal claimsPrincipal;
        if (user is not null)
        {
            var claims = new Claim[]
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Identity),
                new(ClaimTypes.NameIdentifier, user.UserId)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            await js.InvokeVoidAsync("localStorageHelper.setItem", "jwt", token);

            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Jwt"));
        }
        else
        {
            claimsPrincipal = _anonymous;
            await js.InvokeVoidAsync("localStorageHelper.removeItem", "jwt");
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}