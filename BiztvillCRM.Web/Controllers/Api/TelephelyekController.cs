using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app telephely-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/telephelyek")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TelephelyekController : ControllerBase
{
    private readonly ITelephelyService _telephelyService;

    public TelephelyekController(ITelephelyService telephelyService)
    {
        _telephelyService = telephelyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TelephelyListItemDto>>> GetAll()
    {
        var telephelyek = await _telephelyService.GetAllAsync();

        var dto = telephelyek.Select(t => new TelephelyListItemDto
        {
            Id = t.Id,
            Nev = t.Nev,
            Cim = t.Cim,
            UgyfelNev = t.Ugyfel?.Nev,
            Kapcsolattarto = t.Kapcsolattarto,
            Telefon = t.Telefon,
            Email = t.Email,
            Aktiv = t.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TelephelyListItemDto>> GetById(int id)
    {
        var telephely = await _telephelyService.GetByIdAsync(id);
        if (telephely is null)
            return NotFound();

        var dto = new TelephelyListItemDto
        {
            Id = telephely.Id,
            Nev = telephely.Nev,
            Cim = telephely.Cim,
            UgyfelNev = telephely.Ugyfel?.Nev,
            Kapcsolattarto = telephely.Kapcsolattarto,
            Telefon = telephely.Telefon,
            Email = telephely.Email,
            Aktiv = telephely.Aktiv
        };

        return Ok(dto);
    }
}
