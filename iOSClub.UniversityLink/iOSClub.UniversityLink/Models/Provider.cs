using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Models;

public class Provider(ProtectedSessionStorage sessionStorage) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var storageResult = await sessionStorage.GetAsync<UserModel>("User");
        var user = storageResult.Success ? storageResult.Value : null;
        if (user == null)
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new (ClaimTypes.NameIdentifier,user.UserId),
            new(ClaimTypes.Role,user.Identity)
        }, "Auth"));
            
        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public async Task UpdateAuthState(UserModel? user)
    {
        ClaimsPrincipal claimsPrincipal;
        if (user is not null)
        {
            await sessionStorage.SetAsync("User", user);
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new (ClaimTypes.NameIdentifier,user.UserId),
                new(ClaimTypes.Role,user.Identity)
            }));
        }
        else
        {
            await sessionStorage.DeleteAsync("User");
            claimsPrincipal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
    
}

public static class ProviderExtensions
{
    public static async Task Logout(this Provider provider)
    {
        await provider.UpdateAuthState(null);
    }
}