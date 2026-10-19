using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app munkavédelmi oktatás-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/munkavedelmioktatasok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MunkavedelmiOktatasokController : ControllerBase
{
    private readonly IMunkavedelmiOktatasService _oktatasService;

    public MunkavedelmiOktatasokController(IMunkavedelmiOktatasService oktatasService)
    {
        _oktatasService = oktatasService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MunkavedelmiOktatasListItemDto>>> GetAll()
    {
        var oktatasok = await _oktatasService.GetAllAsync();

        var dto = oktatasok.Select(o => new MunkavedelmiOktatasListItemDto
        {
            Id = o.Id,
            Megnevezes = o.Megnevezes,
            Leiras = o.Leiras,
            OktatasDatuma = o.OktatasDatuma,
            KovetkezoOktatas = o.KovetkezoOktatas,
            OktatoNeve = o.OktatoNeve,
            Megjegyzes = o.Megjegyzes,
            UgyfelNev = o.Ugyfel?.Nev,
            TelephelyNev = o.Telephely?.Nev,
            Aktiv = o.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MunkavedelmiOktatasListItemDto>> GetById(int id)
    {
        var oktatas = await _oktatasService.GetByIdAsync(id);
        if (oktatas is null)
            return NotFound();

        var dto = new MunkavedelmiOktatasListItemDto
        {
            Id = oktatas.Id,
            Megnevezes = oktatas.Megnevezes,
            Leiras = oktatas.Leiras,
            OktatasDatuma = oktatas.OktatasDatuma,
            KovetkezoOktatas = oktatas.KovetkezoOktatas,
            OktatoNeve = oktatas.OktatoNeve,
            Megjegyzes = oktatas.Megjegyzes,
            UgyfelNev = oktatas.Ugyfel?.Nev,
            TelephelyNev = oktatas.Telephely?.Nev,
            Aktiv = oktatas.Aktiv
        };

        return Ok(dto);
    }
}
