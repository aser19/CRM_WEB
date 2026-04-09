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
    SmtpTeszt = 16,  // Új: teszt email küldéshez
    // Kockázatértékelés
    KockazatFelulvizsgalat90Nap = 50,
    KockazatFelulvizsgalat30Nap = 51,
    // Munkavédelmi oktatás
    MunkavedelmiOktatas90Nap = 60,
    MunkavedelmiOktatas30Nap = 61,
    // Zónatérkép
    ZonaterkepLejarat90Nap = 70,
    ZonaterkepLejarat30Nap = 71,
}