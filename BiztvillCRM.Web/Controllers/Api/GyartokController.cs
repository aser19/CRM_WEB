using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app gyártó-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/gyartok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GyartokController : ControllerBase
{
    private readonly IGyartoService _gyartoService;

    public GyartokController(IGyartoService gyartoService)
    {
        _gyartoService = gyartoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GyartoListItemDto>>> GetAll()
    {
        var gyartok = await _gyartoService.GetAllAsync();

        var dto = gyartok.Select(g => new GyartoListItemDto
        {
            Id = g.Id,
            Nev = g.Nev,
            Orszag = g.Orszag,
            Weboldal = g.Weboldal,
            Aktiv = g.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GyartoListItemDto>> GetById(int id)
    {
        var gyarto = await _gyartoService.GetByIdAsync(id);
        if (gyarto is null)
            return NotFound();

        var dto = new GyartoListItemDto
        {
            Id = gyarto.Id,
            Nev = gyarto.Nev,
            Orszag = gyarto.Orszag,
            Weboldal = gyarto.Weboldal,
            Aktiv = gyarto.Aktiv
        };

        return Ok(dto);
    }
}
