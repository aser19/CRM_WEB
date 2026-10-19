using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app kötelező hitelesítés-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/kotelezohitelesitesek")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class KotelezoHitelesitesekController : ControllerBase
{
    private readonly IKotelezoHitelesitesService _kotelezoHitelesitesService;

    public KotelezoHitelesitesekController(IKotelezoHitelesitesService kotelezoHitelesitesService)
    {
        _kotelezoHitelesitesService = kotelezoHitelesitesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<KotelezoHitelesitesListItemDto>>> GetAll()
    {
        var kotelezoHitelesitesek = await _kotelezoHitelesitesService.GetAllAsync();

        var dto = kotelezoHitelesitesek.Select(k => new KotelezoHitelesitesListItemDto
        {
            Id = k.Id,
            Megnevezes = k.Megnevezes,
            JogszabalyiHivatkozas = k.JogszabalyiHivatkozas,
            HitelesitesiIdoszakHonap = k.HitelesitesiIdoszakHonap,
            Megjegyzes = k.Megjegyzes,
            Aktiv = k.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<KotelezoHitelesitesListItemDto>> GetById(int id)
    {
        var kotelezoHitelesites = await _kotelezoHitelesitesService.GetByIdAsync(id);
        if (kotelezoHitelesites is null)
            return NotFound();

        var dto = new KotelezoHitelesitesListItemDto
        {
            Id = kotelezoHitelesites.Id,
            Megnevezes = kotelezoHitelesites.Megnevezes,
            JogszabalyiHivatkozas = kotelezoHitelesites.JogszabalyiHivatkozas,
            HitelesitesiIdoszakHonap = kotelezoHitelesites.HitelesitesiIdoszakHonap,
            Megjegyzes = kotelezoHitelesites.Megjegyzes,
            Aktiv = kotelezoHitelesites.Aktiv
        };

        return Ok(dto);
    }
}
