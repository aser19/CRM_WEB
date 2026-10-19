using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app jogszabály-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/jogszabalyok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JogszabalyokController : ControllerBase
{
    private readonly IJogszabalyService _jogszabalyService;

    public JogszabalyokController(IJogszabalyService jogszabalyService)
    {
        _jogszabalyService = jogszabalyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JogszabalyListItemDto>>> GetAll()
    {
        var jogszabalyok = await _jogszabalyService.GetAllAsync();

        var dto = jogszabalyok.Select(j => new JogszabalyListItemDto
        {
            Id = j.Id,
            Szam = j.Szam,
            Cim = j.Cim,
            Leiras = j.Leiras,
            Tipus = j.Tipus.ToString(),
            HatalyosKezdet = j.HatalyosKezdet,
            HatalyosVege = j.HatalyosVege,
            Url = j.Url,
            Megjegyzes = j.Megjegyzes,
            JelenlegHatalyos = j.JelenlegHatalyos,
            Aktiv = j.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JogszabalyListItemDto>> GetById(int id)
    {
        var jogszabaly = await _jogszabalyService.GetByIdAsync(id);
        if (jogszabaly is null)
            return NotFound();

        var dto = new JogszabalyListItemDto
        {
            Id = jogszabaly.Id,
            Szam = jogszabaly.Szam,
            Cim = jogszabaly.Cim,
            Leiras = jogszabaly.Leiras,
            Tipus = jogszabaly.Tipus.ToString(),
            HatalyosKezdet = jogszabaly.HatalyosKezdet,
            HatalyosVege = jogszabaly.HatalyosVege,
            Url = jogszabaly.Url,
            Megjegyzes = jogszabaly.Megjegyzes,
            JelenlegHatalyos = jogszabaly.JelenlegHatalyos,
            Aktiv = jogszabaly.Aktiv
        };

        return Ok(dto);
    }
}
