using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatProject.ConfigModels;
using Microsoft.IdentityModel.Tokens;

namespace ChatProject.Helpers;

public static class TokenGenerators 
{
    public static string GenerateAccessToken(string username, JwtSettings jwtSettings) 
    {
        var claims = new[] 
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, username)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)
        );

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(jwtSettings.ExpirationSeconds),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
};

