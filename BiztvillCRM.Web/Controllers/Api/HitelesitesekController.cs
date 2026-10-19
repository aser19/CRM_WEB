using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app hitelesítés-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/hitelesitesek")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class HitelesitesekController : ControllerBase
{
    private readonly IHitelesitesService _hitelesitesService;

    public HitelesitesekController(IHitelesitesService hitelesitesService)
    {
        _hitelesitesService = hitelesitesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<HitelesitesListItemDto>>> GetAll()
    {
        var hitelesitesek = await _hitelesitesService.GetAllAsync();

        var dto = hitelesitesek.Select(h => new HitelesitesListItemDto
        {
            Id = h.Id,
            Datum = h.Datum,
            LejaratDatum = h.LejaratDatum,
            UgyfelNev = h.Ugyfel?.Nev,
            TelephelyNev = h.Telephely?.Nev,
            EszkozTipusNev = h.EszkozTipus?.Nev,
            EszkozAzonosito = h.EszkozAzonosito,
            HitelesitesStatusz = h.HitelesitesStatusz.ToString(),
            VanMunkalap = h.MunkalapPathsLista.Count > 0,
            VanBizonyitvany = h.BizonyitvanyPathsLista.Count > 0
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HitelesitesDetailDto>> GetById(int id)
    {
        var h = await _hitelesitesService.GetByIdAsync(id);
        if (h is null)
            return NotFound();

        var dto = new HitelesitesDetailDto
        {
            Id = h.Id,
            Datum = h.Datum,
            LejaratDatum = h.LejaratDatum,
            UgyfelNev = h.Ugyfel?.Nev,
            TelephelyNev = h.Telephely?.Nev,
            EszkozTipusNev = h.EszkozTipus?.Nev,
            EszkozAzonosito = h.EszkozAzonosito,
            HitelesitesStatusz = h.HitelesitesStatusz.ToString(),
            VanMunkalap = h.MunkalapPathsLista.Count > 0,
            VanBizonyitvany = h.BizonyitvanyPathsLista.Count > 0,
            Megjegyzes = h.Megjegyzes,
            HatosagNev = h.Hatosag?.Nev,
            MunkalapFajlokSzama = h.MunkalapPathsLista.Count,
            BizonyitvanyFajlokSzama = h.BizonyitvanyPathsLista.Count
        };

        return Ok(dto);
    }
}
