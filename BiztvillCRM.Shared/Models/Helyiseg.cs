namespace BiztvillCRM.Shared.Models;

public class Helyiseg
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }

    // e.g. "Titkárság", "Konyha", "Tárgyaló"
    public string Nev { get; set; } = string.Empty;

    // optional: which telephely this room belongs to
    public int? TelephelyId { get; set; }

    // navigation
    public Telephely? Telephely { get; set; }
}