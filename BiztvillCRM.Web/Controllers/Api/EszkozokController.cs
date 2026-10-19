using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app eszköz-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/eszkozok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EszkozokController : ControllerBase
{
    private readonly IEszkozService _eszkozService;

    public EszkozokController(IEszkozService eszkozService)
    {
        _eszkozService = eszkozService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EszkozListItemDto>>> GetAll()
    {
        var eszkozok = await _eszkozService.GetAllAsync();

        var dto = eszkozok.Select(e => new EszkozListItemDto
        {
            Id = e.Id,
            Nev = e.Nev,
            GyariSzam = e.GyariSzam,
            Tipus = e.Tipus,
            Kategoria = e.Kategoria,
            GyartoNev = e.Gyarto?.Nev,
            UgyfelNev = e.Ugyfel?.Nev,
            TelephelyNev = e.Telephely?.Nev,
            Aktiv = e.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EszkozListItemDto>> GetById(int id)
    {
        var eszkoz = await _eszkozService.GetByIdAsync(id);
        if (eszkoz is null)
            return NotFound();

        var dto = new EszkozListItemDto
        {
            Id = eszkoz.Id,
            Nev = eszkoz.Nev,
            GyariSzam = eszkoz.GyariSzam,
            Tipus = eszkoz.Tipus,
            Kategoria = eszkoz.Kategoria,
            GyartoNev = eszkoz.Gyarto?.Nev,
            UgyfelNev = eszkoz.Ugyfel?.Nev,
            TelephelyNev = eszkoz.Telephely?.Nev,
            Aktiv = eszkoz.Aktiv
        };

        return Ok(dto);
    }
}
