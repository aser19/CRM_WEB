using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app kockázatértékelés-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/kockazatertekelesek")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class KockazatertekelesekController : ControllerBase
{
    private readonly IKockazatertekelésService _kockazatertekelesService;

    public KockazatertekelesekController(IKockazatertekelésService kockazatertekelesService)
    {
        _kockazatertekelesService = kockazatertekelesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<KockazatertekelesListItemDto>>> GetAll()
    {
        var ertekelesek = await _kockazatertekelesService.GetAllAsync();

        var dto = ertekelesek.Select(k => new KockazatertekelesListItemDto
        {
            Id = k.Id,
            Megnevezes = k.Megnevezes,
            ErtekelesDatuma = k.ErtekelesDatuma,
            KovetkezoFelulvizsgalat = k.KovetkezoFelulvizsgalat,
            KockazatiSzint = k.KockazatiSzint.ToString(),
            Leiras = k.Leiras,
            Intezkedesek = k.Intezkedesek,
            FelelosNeve = k.FelelosNeve,
            Statusz = k.Statusz.ToString(),
            UgyfelNev = k.Ugyfel?.Nev,
            TelephelyNev = k.Telephely?.Nev,
            Aktiv = k.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<KockazatertekelesListItemDto>> GetById(int id)
    {
        var ertekeles = await _kockazatertekelesService.GetByIdAsync(id);
        if (ertekeles is null)
            return NotFound();

        var dto = new KockazatertekelesListItemDto
        {
            Id = ertekeles.Id,
            Megnevezes = ertekeles.Megnevezes,
            ErtekelesDatuma = ertekeles.ErtekelesDatuma,
            KovetkezoFelulvizsgalat = ertekeles.KovetkezoFelulvizsgalat,
            KockazatiSzint = ertekeles.KockazatiSzint.ToString(),
            Leiras = ertekeles.Leiras,
            Intezkedesek = ertekeles.Intezkedesek,
            FelelosNeve = ertekeles.FelelosNeve,
            Statusz = ertekeles.Statusz.ToString(),
            UgyfelNev = ertekeles.Ugyfel?.Nev,
            TelephelyNev = ertekeles.Telephely?.Nev,
            Aktiv = ertekeles.Aktiv
        };

        return Ok(dto);
    }
}
