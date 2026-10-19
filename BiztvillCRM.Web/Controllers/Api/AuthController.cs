using BiztvillCRM.Shared.Dtos.Mobile;
using BiztvillCRM.Shared.Models;
using BiztvillCRM.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Mobil app bejelentkezés: ugyanazokat a hitelesítő adatokat fogadja, mint a webes
/// /account/login végpont, de JWT tokent ad vissza cookie helyett.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Felhasznalo> _userManager;
    private readonly SignInManager<Felhasznalo> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<Felhasznalo> userManager,
        SignInManager<Felhasznalo> signInManager,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Felhasználónév/email és jelszó megadása kötelező.");

        var user = await _userManager.FindByNameAsync(request.UsernameOrEmail)
                   ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);

        if (user is null || !user.Aktiv)
            return Unauthorized("Hibás felhasználónév vagy jelszó.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Sikertelen mobil bejelentkezés: {UsernameOrEmail}", request.UsernameOrEmail);
            return Unauthorized("Hibás felhasználónév vagy jelszó.");
        }

        var szerepkorok = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.GenerateTokenAsync(user, szerepkorok);

        user.UtolsoBelepes = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var jwtExpireMinutes = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
            .GetSection("Jwt").GetValue<int?>("ExpireMinutes") ?? 480;

        return Ok(new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(jwtExpireMinutes),
            UserName = user.UserName ?? user.Email ?? string.Empty,
            TeljesNev = user.Nev
        });
    }
}
