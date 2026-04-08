using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Cég email értesítési beállításai.</summary>
public class EmailBeallitas
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    /// <summary>Melyik céghez tartozik a beállítás.</summary>
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;

    /// <summary>Milyen típusú értesítéseket kér (flags).</summary>
    public EmailErtesitesTipus ErtesitesTipusok { get; set; } = EmailErtesitesTipus.Nincs;

    /// <summary>Kinek küldjük az értesítést (flags).</summary>
    public EmailCimzettTipus CimzettTipusok { get; set; } = EmailCimzettTipus.Nincs;

    /// <summary>Egyedi email címek (JSON lista, pl: ["a@b.com","c@d.com"]).</summary>
    public string? EgyediEmailCimek { get; set; }

    public bool Aktiv { get; set; } = true;
}