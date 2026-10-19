using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app tanúsítvány-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/tanusitvanyok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TanusitvanyokController : ControllerBase
{
    private readonly ITanusitvanyService _tanusitvanyService;

    public TanusitvanyokController(ITanusitvanyService tanusitvanyService)
    {
        _tanusitvanyService = tanusitvanyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TanusitvanyListItemDto>>> GetAll()
    {
        var tanusitvanyok = await _tanusitvanyService.GetAllAsync();

        var dto = tanusitvanyok.Select(t => new TanusitvanyListItemDto
        {
            Id = t.Id,
            Nev = t.Nev,
            Szam = t.Szam,
            KiadoDatum = t.KiadoDatum,
            LejaratDatum = t.LejaratDatum,
            UgyfelNev = t.Ugyfel?.Nev,
            Megjegyzes = t.Megjegyzes
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TanusitvanyListItemDto>> GetById(int id)
    {
        var tanusitvany = await _tanusitvanyService.GetByIdAsync(id);
        if (tanusitvany is null)
            return NotFound();

        var dto = new TanusitvanyListItemDto
        {
            Id = tanusitvany.Id,
            Nev = tanusitvany.Nev,
            Szam = tanusitvany.Szam,
            KiadoDatum = tanusitvany.KiadoDatum,
            LejaratDatum = tanusitvany.LejaratDatum,
            UgyfelNev = tanusitvany.Ugyfel?.Nev,
            Megjegyzes = tanusitvany.Megjegyzes
        };

        return Ok(dto);
    }
}
