namespace BiztvillCRM.Shared.Enums;

/// <summary>Email értesítés típusok.</summary>
[Flags]
public enum EmailErtesitesTipus
{
    Nincs = 0,
    HitelesitesLejarat90Nap = 1,
    HitelesitesLejarat30Nap = 2,
    MeresLejarat90Nap = 4,
    MeresLejarat30Nap = 8,
    SmtpTeszt = 16  // Új: teszt email küldéshez
}