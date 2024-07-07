using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Models;

public class TokenActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var bearer = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(bearer) || !bearer.Contains("Bearer")) return;
        var jwt = bearer.Split(' ');
        var tokenObj = new JwtSecurityToken(jwt[1]);

        var claimsIdentity = new ClaimsIdentity(tokenObj.Claims);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        context.HttpContext.User = claimsPrincipal;
    }
}

public static class TokenHelper
{
    public static UserModel? GetUser(this ClaimsPrincipal? claimsPrincipal)
    {
        var claimId = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid);
        var claimName = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        var claimIdentity = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (claimId.IsNull() || claimName.IsNull() || claimIdentity.IsNull())
        {
            return null;
        }

        return new UserModel()
        {
            UserName = claimName!.Value,
            UserId = claimId!.Value,
            Identity = claimIdentity!.Value
        };
    }

    private static bool IsNull(this Claim? claim)
        => claim == null || string.IsNullOrEmpty(claim.Value);
}