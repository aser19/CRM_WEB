using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app kalibráció-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/kalibraciok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class KalibraciokController : ControllerBase
{
    private readonly IKalibracioService _kalibracioService;

    public KalibraciokController(IKalibracioService kalibracioService)
    {
        _kalibracioService = kalibracioService;
    }

    [HttpGet]
    public async Task<ActionResult<List<KalibracioListItemDto>>> GetAll()
    {
        var kalibraciok = await _kalibracioService.GetAllAsync();

        var dto = kalibraciok.Select(k => new KalibracioListItemDto
        {
            Id = k.Id,
            Datum = k.Datum,
            KovetkezoDatum = k.KovetkezoDatum,
            EszkozNev = k.Eszkoz?.Nev,
            UgyfelNev = k.Eszkoz?.Ugyfel?.Nev,
            Bizonyitvany = k.Bizonyitvany,
            Elvegzo = k.Elvegzo,
            Megjegyzes = k.Megjegyzes,
            Sikeres = k.Sikeres
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<KalibracioListItemDto>> GetById(int id)
    {
        var kalibracio = await _kalibracioService.GetByIdAsync(id);
        if (kalibracio is null)
            return NotFound();

        var dto = new KalibracioListItemDto
        {
            Id = kalibracio.Id,
            Datum = kalibracio.Datum,
            KovetkezoDatum = kalibracio.KovetkezoDatum,
            EszkozNev = kalibracio.Eszkoz?.Nev,
            UgyfelNev = kalibracio.Eszkoz?.Ugyfel?.Nev,
            Bizonyitvany = kalibracio.Bizonyitvany,
            Elvegzo = kalibracio.Elvegzo,
            Megjegyzes = kalibracio.Megjegyzes,
            Sikeres = kalibracio.Sikeres
        };

        return Ok(dto);
    }
}
