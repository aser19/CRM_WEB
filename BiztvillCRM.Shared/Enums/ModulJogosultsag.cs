namespace BiztvillCRM.Shared.Enums;

/// <summary>
/// Cégekhez rendelhető modul jogosultságok.
/// </summary>
[Flags]
public enum ModulJogosultsag
{
    Nincs = 0,
    
    // Alap modulok
    Ugyfelek = 1,
    Meresek = 2,
    
    // Benzinkút modul
    Hitelesitesek = 4,
    Karbantartasok = 8,
    
    // Munkavédelmi modul
    Zonaterkepek = 16,
    MunkavedelmiOktatasok = 32,
    Kockazatertekelesek = 64,
    
    // Kombinált flag a munkavédelemhez
    Munkavedelem = MunkavedelmiOktatasok | Kockazatertekelesek | Zonaterkepek,

    // Beállítások
    EmailSablonok = 128,
    Jogszabalyok = 256,
    
    // === ÚJ MODULOK ===
    Kalibraciok = 512,
    Jegyzokonyvek = 1024,
    Riportok = 2048,           // Admin only
    KepzesTipusok = 4096,      // Admin only
    KepzesSzabalyok = 8192,    // Admin only
    AdminFunkciok = 16384      // Admin menü összes funkciója
}