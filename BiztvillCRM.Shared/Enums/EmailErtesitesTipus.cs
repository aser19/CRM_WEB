namespace BiztvillCRM.Shared.Enums;

/// <summary>Email értesítés típusok.</summary>
[Flags]
public enum EmailErtesitesTipus
{
    Nincs = 0,
    HitelesitesLejarat90Nap = 1,      // 2^0
    HitelesitesLejarat30Nap = 2,      // 2^1
    MeresLejarat90Nap = 4,            // 2^2
    MeresLejarat30Nap = 8,            // 2^3
    SmtpTeszt = 16,                   // 2^4
    // ✅ JAVÍTVA: 2 hatványok használata
    KockazatFelulvizsgalat90Nap = 32,  // 2^5
    KockazatFelulvizsgalat30Nap = 64,  // 2^6
    MunkavedelmiOktatas90Nap = 128,    // 2^7
    MunkavedelmiOktatas30Nap = 256,    // 2^8
    ZonaterkepLejarat90Nap = 512,      // 2^9
    ZonaterkepLejarat30Nap = 1024,     // 2^10
    KarbantartasLejarat90Nap = 2048,   // 2^11
    KarbantartasLejarat30Nap = 4096,   // 2^12
}