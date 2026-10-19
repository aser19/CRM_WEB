using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app jegyzőkönyv-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/jegyzokonyvek")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JegyzokonyvekController : ControllerBase
{
    private readonly IMeresService _meresService;
    private readonly IMellekletJegyzokonyvService _mellekletService;

    public JegyzokonyvekController(IMeresService meresService, IMellekletJegyzokonyvService mellekletService)
    {
        _meresService = meresService;
        _mellekletService = mellekletService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JegyzokonyvListItemDto>>> GetAll()
    {
        var meresek = await _meresService.GetAllAsync();

        var dto = meresek.Select(m => new JegyzokonyvListItemDto
        {
            Id = m.Id,
            Datum = m.Datum,
            KovetkezoDatum = m.KovetkezoDatum,
            UgyfelNev = m.Ugyfel?.Nev,
            TelephelyNev = m.Telephely?.Nev,
            MeresTipusNev = m.MeresTipus?.Nev,
            MeresStatusz = m.MeresStatusz.ToString(),
            Eredmeny = m.Eredmeny
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JegyzokonyvDetailDto>> GetById(int id)
    {
        var meres = await _meresService.GetByIdAsync(id);
        if (meres is null)
            return NotFound();

        var mellekletek = await _mellekletService.GetByMeresIdAsync(id);

        var dto = new JegyzokonyvDetailDto
        {
            Id = meres.Id,
            Datum = meres.Datum,
            KovetkezoDatum = meres.KovetkezoDatum,
            UgyfelNev = meres.Ugyfel?.Nev,
            TelephelyNev = meres.Telephely?.Nev,
            MeresTipusNev = meres.MeresTipus?.Nev,
            MeresStatusz = meres.MeresStatusz.ToString(),
            Eredmeny = meres.Eredmeny,
            Megjegyzes = meres.Megjegyzes,
            JegyzokonyvAdatokJson = meres.JegyzokonyvAdatokJson,
            VillamNemNormaAdatokJson = meres.VillamNemNormaAdatokJson,
            Mellekletek = mellekletek.Select(mj => new JegyzokonyvMellekletDto
            {
                Id = mj.Id,
                Tipus = mj.Tipus,
                TipusNev = BiztvillCRM.Shared.Models.MellekletJegyzokonyv.TipusNev(mj.Tipus),
                Szam = mj.Szam,
                Statusz = mj.Statusz
            }).ToList()
        };

        return Ok(dto);
    }
}
