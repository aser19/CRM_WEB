namespace BiztvillCRM.Shared.Models;

/// <summary>Méréstípushoz rendelt jogszabály (N:M kapcsolat)</summary>
public class MeresTipusJogszabaly
{
    public int Id { get; set; }
    public int MeresTipusId { get; set; }
    public MeresTipus? MeresTipus { get; set; }
    public int JogszabalyId { get; set; }
    public Jogszabaly? Jogszabaly { get; set; }

    /// <summary>Megjelenítési sorrend a dokumentumban</summary>
    public int Sorrend { get; set; } = 0;
}