namespace BiztvillCRM.Shared.Enums;

/// <summary>SMTP titkosítás típusok.</summary>
public enum SmtpTitkositasTipus
{
    /// <summary>Nincs titkosítás (nem ajánlott)</summary>
    Nincs = 0,
    
    /// <summary>Automatikus felismerés (ajánlott)</summary>
    Auto = 1,
    
    /// <summary>STARTTLS (Port 587, 25)</summary>
    StartTls = 2,
    
    /// <summary>Implicit SSL (Port 465)</summary>
    SslOnConnect = 3
}