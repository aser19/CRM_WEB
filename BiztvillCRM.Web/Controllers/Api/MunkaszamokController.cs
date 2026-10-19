using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app munkaszám-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/munkaszamok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MunkaszamokController : ControllerBase
{
    private readonly IMunkaszamTorzsService _munkaszamService;

    public MunkaszamokController(IMunkaszamTorzsService munkaszamService)
    {
        _munkaszamService = munkaszamService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MunkaszamListItemDto>>> GetAll()
    {
        var munkaszamok = await _munkaszamService.GetAllAsync();

        var dto = munkaszamok.Select(m => new MunkaszamListItemDto
        {
            Id = m.Id,
            Szam = m.Szam,
            Megnevezes = m.Megnevezes,
            Aktiv = m.Aktiv,
            CegNev = m.Ceg?.Nev
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MunkaszamListItemDto>> GetById(int id)
    {
        var munkaszam = await _munkaszamService.GetByIdAsync(id);
        if (munkaszam is null)
            return NotFound();

        var dto = new MunkaszamListItemDto
        {
            Id = munkaszam.Id,
            Szam = munkaszam.Szam,
            Megnevezes = munkaszam.Megnevezes,
            Aktiv = munkaszam.Aktiv,
            CegNev = munkaszam.Ceg?.Nev
        };

        return Ok(dto);
    }
}
