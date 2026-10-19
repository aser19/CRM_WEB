using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app felülvizsgáló-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/felulvizsgalok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FelulvizsgalokController : ControllerBase
{
    private readonly IFelulvizsgaloService _felulvizsgaloService;

    public FelulvizsgalokController(IFelulvizsgaloService felulvizsgaloService)
    {
        _felulvizsgaloService = felulvizsgaloService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FelulvizsgaloListItemDto>>> GetAll()
    {
        var felulvizsgalok = await _felulvizsgaloService.GetAllAsync();

        var dto = felulvizsgalok.Select(f => new FelulvizsgaloListItemDto
        {
            Id = f.Id,
            Nev = f.Nev,
            Jogosultsag = f.Jogosultsag.ToString(),
            Email = f.Email,
            Telefon = f.Telefon,
            Megjegyzes = f.Megjegyzes,
            Aktiv = f.Aktiv,
            VanAlairas = !string.IsNullOrEmpty(f.AlairasPath)
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FelulvizsgaloDetailDto>> GetById(int id)
    {
        var felulvizsgalo = await _felulvizsgaloService.GetByIdWithDetailsAsync(id);
        if (felulvizsgalo is null)
            return NotFound();

        var dto = new FelulvizsgaloDetailDto
        {
            Id = felulvizsgalo.Id,
            Nev = felulvizsgalo.Nev,
            Jogosultsag = felulvizsgalo.Jogosultsag.ToString(),
            Email = felulvizsgalo.Email,
            Telefon = felulvizsgalo.Telefon,
            Megjegyzes = felulvizsgalo.Megjegyzes,
            Aktiv = felulvizsgalo.Aktiv,
            VanAlairas = !string.IsNullOrEmpty(felulvizsgalo.AlairasPath),
            Kepzesek = felulvizsgalo.Kepzesek.Select(k => new FelulvizsgaloKepzesDto
            {
                Id = k.Id,
                KepzesTipusNev = k.KepzesTipus?.Nev,
                BizonyitvanySzam = k.BizonyitvanySzam,
                BizonyitvanyKelte = k.BizonyitvanyKelte,
                BizonyitvanyLejarat = k.BizonyitvanyLejarat,
                Megjegyzes = k.Megjegyzes,
                Aktiv = k.Aktiv,
                VanBizonyitvanyFajl = !string.IsNullOrEmpty(k.BizonyitvanyPath)
            }).ToList()
        };

        return Ok(dto);
    }
}
