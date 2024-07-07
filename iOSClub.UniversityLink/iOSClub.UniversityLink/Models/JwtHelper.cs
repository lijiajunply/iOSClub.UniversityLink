using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UniversityLink.DataModels;

namespace iOSClub.UniversityLink.Models;

public class JwtHelper(IConfiguration configuration)
{
    public string GetMemberToken(UserModel model)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, model.UserName),
            new Claim(ClaimTypes.PrimarySid,model.UserId),
            new(ClaimTypes.Role,model.Identity)
        };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));
        const string algorithm = SecurityAlgorithms.HmacSha256;
        var signingCredentials = new SigningCredentials(secretKey, algorithm);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.Now, //notBefore
            expires: DateTime.Now.AddSeconds(30), //expires
            signingCredentials: signingCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}