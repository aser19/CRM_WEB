using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IVizsgalatiSablonService
{
    Task<List<VizsgalatiSablon>> GetByMeresTipusIdAsync(int meresTipusId);
    Task<List<VizsgalatiSablon>> GetAllAsync(int meresTipusId, int? cegId = null);
    Task<VizsgalatiSablon?> GetByIdAsync(int id);
    Task<VizsgalatiSablon> MentesAsync(VizsgalatiSablon sablon);
    Task TorlesAsync(int id);
    Task<List<string>> GetKategoriak(int meresTipusId);
}