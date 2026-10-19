using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app karbantartás-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/karbantartasok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class KarbantartasokController : ControllerBase
{
    private readonly IKarbantartasService _karbantartasService;

    public KarbantartasokController(IKarbantartasService karbantartasService)
    {
        _karbantartasService = karbantartasService;
    }

    [HttpGet]
    public async Task<ActionResult<List<KarbantartasListItemDto>>> GetAll()
    {
        var karbantartasok = await _karbantartasService.GetAllAsync();

        var dto = karbantartasok.Select(k => new KarbantartasListItemDto
        {
            Id = k.Id,
            Datum = k.Datum,
            KovetkezoDatum = k.KovetkezoDatum,
            UgyfelNev = k.Ugyfel?.Nev,
            TelephelyNev = k.Telephely?.Nev,
            KarbantartasTipusNev = k.KarbantartasTipus?.Nev,
            Leiras = k.Leiras,
            Elvegzo = k.Elvegzo,
            Statusz = k.Statusz.ToString()
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<KarbantartasListItemDto>> GetById(int id)
    {
        var karbantartas = await _karbantartasService.GetByIdAsync(id);
        if (karbantartas is null)
            return NotFound();

        var dto = new KarbantartasListItemDto
        {
            Id = karbantartas.Id,
            Datum = karbantartas.Datum,
            KovetkezoDatum = karbantartas.KovetkezoDatum,
            UgyfelNev = karbantartas.Ugyfel?.Nev,
            TelephelyNev = karbantartas.Telephely?.Nev,
            KarbantartasTipusNev = karbantartas.KarbantartasTipus?.Nev,
            Leiras = karbantartas.Leiras,
            Elvegzo = karbantartas.Elvegzo,
            Statusz = karbantartas.Statusz.ToString()
        };

        return Ok(dto);
    }
}
