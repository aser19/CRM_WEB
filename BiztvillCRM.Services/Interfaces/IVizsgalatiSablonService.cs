using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IVizsgalatiSablonService
{
    Task<List<VizsgalatiSablon>> GetByMeresTipusIdAsync(int meresTipusId);
    Task<List<string>> GetKategoriak(int meresTipusId);
}