using BiztvillCRM.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiztvillCRM.Web.Controllers.Api;

/// <summary>
/// Fájlok (munkalap/bizonyítvány PDF) megtekintése a mobil appból.
/// Csak a hitelesítéshez tartozó, a felhasználó cégéhez tartozó fájlok érhetők el.
/// </summary>
[ApiController]
[Route("api/fajlok")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FajlokController : ControllerBase
{
    private readonly IHitelesitesService _hitelesitesService;
    private readonly IFelulvizsgaloService _felulvizsgaloService;
    private readonly IFileStorageService _fileStorageService;

    public FajlokController(IHitelesitesService hitelesitesService, IFelulvizsgaloService felulvizsgaloService, IFileStorageService fileStorageService)
    {
        _hitelesitesService = hitelesitesService;
        _felulvizsgaloService = felulvizsgaloService;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("hitelesites/{id:int}/munkalap/{index:int}")]
    public async Task<IActionResult> GetMunkalap(int id, int index) =>
        await GetHitelesitesFajl(id, index, munkalap: true);

    [HttpGet("hitelesites/{id:int}/bizonyitvany/{index:int}")]
    public async Task<IActionResult> GetBizonyitvany(int id, int index) =>
        await GetHitelesitesFajl(id, index, munkalap: false);

    [HttpGet("felulvizsgalo-kepzes/{kepzesId:int}/bizonyitvany")]
    public async Task<IActionResult> GetFelulvizsgaloBizonyitvany(int kepzesId)
    {
        var felulvizsgalok = await _felulvizsgaloService.GetAllAsync();
        var kepzes = felulvizsgalok.SelectMany(f => f.Kepzesek).FirstOrDefault(k => k.Id == kepzesId);
        if (kepzes is null || string.IsNullOrEmpty(kepzes.BizonyitvanyPath))
            return NotFound();

        var stream = await _fileStorageService.GetFileStreamAsync(kepzes.BizonyitvanyPath);
        if (stream is null)
            return NotFound();

        var fileName = Path.GetFileName(kepzes.BizonyitvanyPath);
        return File(stream, "application/pdf", fileName);
    }

    private async Task<IActionResult> GetHitelesitesFajl(int id, int index, bool munkalap)
    {
        var hitelesites = await _hitelesitesService.GetByIdAsync(id);
        if (hitelesites is null)
            return NotFound();

        var lista = munkalap ? hitelesites.MunkalapPathsLista : hitelesites.BizonyitvanyPathsLista;
        if (index < 0 || index >= lista.Count)
            return NotFound();

        var relativePath = lista[index];
        var stream = await _fileStorageService.GetFileStreamAsync(relativePath);
        if (stream is null)
            return NotFound();

        var fileName = Path.GetFileName(relativePath);
        return File(stream, "application/pdf", fileName);
    }
}
