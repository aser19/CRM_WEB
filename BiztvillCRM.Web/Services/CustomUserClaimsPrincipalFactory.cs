using System.Security.Claims;
using BiztvillCRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BiztvillCRM.Web.Services;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<Felhasznalo, IdentityRole>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<Felhasznalo> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Felhasznalo user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        
        // CegId claim hozzáadása - mindig!
        Console.WriteLine($"[GenerateClaimsAsync] user.Id: {user.Id}, user.CegId: {user.CegId}");
        identity.AddClaim(new Claim("CegId", user.CegId.ToString()));
        
        return identity;
    }
}