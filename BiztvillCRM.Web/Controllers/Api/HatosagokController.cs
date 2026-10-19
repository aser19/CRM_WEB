using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app hatóság-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/hatosagok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class HatosagokController : ControllerBase
{
    private readonly IHatosagService _hatosagService;

    public HatosagokController(IHatosagService hatosagService)
    {
        _hatosagService = hatosagService;
    }

    [HttpGet]
    public async Task<ActionResult<List<HatosagListItemDto>>> GetAll()
    {
        var hatosagok = await _hatosagService.GetAllAsync();

        var dto = hatosagok.Select(h => new HatosagListItemDto
        {
            Id = h.Id,
            Nev = h.Nev,
            Rovidites = h.Rovidites,
            Cim = h.Cim,
            Weboldal = h.Weboldal,
            Aktiv = h.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HatosagListItemDto>> GetById(int id)
    {
        var hatosag = await _hatosagService.GetByIdAsync(id);
        if (hatosag is null)
            return NotFound();

        var dto = new HatosagListItemDto
        {
            Id = hatosag.Id,
            Nev = hatosag.Nev,
            Rovidites = hatosag.Rovidites,
            Cim = hatosag.Cim,
            Weboldal = hatosag.Weboldal,
            Aktiv = hatosag.Aktiv
        };

        return Ok(dto);
    }
}
