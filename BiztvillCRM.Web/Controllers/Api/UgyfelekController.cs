using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app ügyfél-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/ugyfelek")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UgyfelekController : ControllerBase
{
    private readonly IUgyfelService _ugyfelService;

    public UgyfelekController(IUgyfelService ugyfelService)
    {
        _ugyfelService = ugyfelService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UgyfelListItemDto>>> GetAll()
    {
        var ugyfelek = await _ugyfelService.GetAllAsync();

        var dto = ugyfelek.Select(u => new UgyfelListItemDto
        {
            Id = u.Id,
            Nev = u.Nev,
            Adoszam = u.Adoszam,
            Cim = u.Cim,
            Email = u.Email,
            Telefon = u.Telefon,
            Aktiv = u.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UgyfelListItemDto>> GetById(int id)
    {
        var ugyfel = await _ugyfelService.GetByIdAsync(id);
        if (ugyfel is null)
            return NotFound();

        var dto = new UgyfelListItemDto
        {
            Id = ugyfel.Id,
            Nev = ugyfel.Nev,
            Adoszam = ugyfel.Adoszam,
            Cim = ugyfel.Cim,
            Email = ugyfel.Email,
            Telefon = ugyfel.Telefon,
            Aktiv = ugyfel.Aktiv
        };

        return Ok(dto);
    }
}
