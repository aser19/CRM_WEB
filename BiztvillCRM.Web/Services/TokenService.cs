using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BiztvillCRM.Shared.Models;
using Microsoft.IdentityModel.Tokens;

namespace BiztvillCRM.Web.Services;

/// <summary>
/// JWT token előállítása a mobil app (és más API kliensek) számára.
/// A webes bejelentkezés (cookie) ezt nem használja.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GenerateTokenAsync(Felhasznalo felhasznalo, IList<string> szerepkorok)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"]!;
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expireMinutes = jwtSection.GetValue<int?>("ExpireMinutes") ?? 480;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, felhasznalo.Id),
            new(ClaimTypes.Name, felhasznalo.UserName ?? felhasznalo.Email ?? felhasznalo.Id),
            new("CegId", felhasznalo.CegId.ToString())
        };

        foreach (var szerepkor in szerepkorok)
        {
            claims.Add(new Claim(ClaimTypes.Role, szerepkor));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
