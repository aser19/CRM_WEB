namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Cégenkénti és évenkénti munkaszám számlálót tároló entitás.
/// </summary>
public class MunkaszamSzamlalo
{
    public int Id { get; set; }
    public int CegId { get; set; }
    public int Ev { get; set; }
    public int UtolsoSorszam { get; set; }
    
    public Ceg? Ceg { get; set; }
}