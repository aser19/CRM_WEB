using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app képzés-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/kepzesek")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class KepzesekController : ControllerBase
{
    private readonly IKepzesService _kepzesService;

    public KepzesekController(IKepzesService kepzesService)
    {
        _kepzesService = kepzesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<KepzesListItemDto>>> GetAll()
    {
        var kepzesek = await _kepzesService.GetAllAsync();

        var dto = kepzesek.Select(k => new KepzesListItemDto
        {
            Id = k.Id,
            Nev = k.Nev,
            Jogosultsag = k.Jogosultsag.ToString(),
            KepzesTipusNev = k.KepzesTipus?.Nev,
            BizonyitvanySzam = k.BizonyitvanySzam,
            BizonyitvanyKelte = k.BizonyitvanyKelte,
            BizonyitvanyLejarat = k.BizonyitvanyLejarat,
            TovabbkepzesSzam = k.TovabbkepzesSzam,
            UtolsoTovabbkepzes = k.UtolsoTovabbkepzes,
            Megjegyzes = k.Megjegyzes,
            Aktiv = k.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<KepzesListItemDto>> GetById(int id)
    {
        var kepzes = await _kepzesService.GetByIdAsync(id);
        if (kepzes is null)
            return NotFound();

        var dto = new KepzesListItemDto
        {
            Id = kepzes.Id,
            Nev = kepzes.Nev,
            Jogosultsag = kepzes.Jogosultsag.ToString(),
            KepzesTipusNev = kepzes.KepzesTipus?.Nev,
            BizonyitvanySzam = kepzes.BizonyitvanySzam,
            BizonyitvanyKelte = kepzes.BizonyitvanyKelte,
            BizonyitvanyLejarat = kepzes.BizonyitvanyLejarat,
            TovabbkepzesSzam = kepzes.TovabbkepzesSzam,
            UtolsoTovabbkepzes = kepzes.UtolsoTovabbkepzes,
            Megjegyzes = kepzes.Megjegyzes,
            Aktiv = kepzes.Aktiv
        };

        return Ok(dto);
    }
}
