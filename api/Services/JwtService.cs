using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ResearchCopilot.Api.Models;

namespace ResearchCopilot.Api.Services;
public class JwtService
{
    private readonly string _key = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "dev";
    public string Create(User u)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, u.Id), new Claim(ClaimTypes.Email, u.Email) };
        var token = new JwtSecurityToken(issuer: "researchcopilot", audience: "researchcopilot", claims: claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);
        return tokenHandler.WriteToken(token);
    }
    public static string UserId(ClaimsPrincipal p) => p.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
}
