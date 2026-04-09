using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Alapértelmezett email beállítások új cégek számára (csak Admin állíthatja).</summary>
public class AlapertelmezettEmailBeallitas
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    /// <summary>Milyen típusú értesítéseket kér alapértelmezetten (flags).</summary>
    public EmailErtesitesTipus ErtesitesTipusok { get; set; } = EmailErtesitesTipus.Nincs;

    /// <summary>Kinek küldjük az értesítést alapértelmezetten (flags).</summary>
    public EmailCimzettTipus CimzettTipusok { get; set; } = EmailCimzettTipus.Nincs;

    /// <summary>Értesítések alapértelmezetten engedélyezve.</summary>
    public bool Aktiv { get; set; } = true;
}