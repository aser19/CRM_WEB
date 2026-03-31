using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ICegService
{
    Task<List<Ceg>> GetAllAsync();
}