using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Csak olvasási (read-only) végpontok a mobil app terminál-megtekintő nézetéhez.
/// Szerkesztés a mobil appból nem támogatott.
/// </summary>
[ApiController]
[Route("api/terminalok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TerminalokController : ControllerBase
{
    private readonly ITerminalService _terminalService;

    public TerminalokController(ITerminalService terminalService)
    {
        _terminalService = terminalService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TerminalListItemDto>>> GetAll()
    {
        var terminalok = await _terminalService.GetAllAsync();

        var dto = terminalok.Select(t => new TerminalListItemDto
        {
            Id = t.Id,
            Nev = t.Nev,
            Azonosito = t.Azonosito,
            IpCim = t.IpCim,
            TelephelyNev = t.Telephely?.Nev,
            Megjegyzes = t.Megjegyzes,
            Aktiv = t.Aktiv
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TerminalListItemDto>> GetById(int id)
    {
        var terminal = await _terminalService.GetByIdAsync(id);
        if (terminal is null)
            return NotFound();

        var dto = new TerminalListItemDto
        {
            Id = terminal.Id,
            Nev = terminal.Nev,
            Azonosito = terminal.Azonosito,
            IpCim = terminal.IpCim,
            TelephelyNev = terminal.Telephely?.Nev,
            Megjegyzes = terminal.Megjegyzes,
            Aktiv = terminal.Aktiv
        };

        return Ok(dto);
    }
}
