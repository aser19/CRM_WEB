using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services;

public class SablonService : ISablonService
{
    private readonly string _sablonMappa;

    public SablonService(string sablonMappa)
    {
        _sablonMappa = sablonMappa;
    }

    public Task<List<JegyzokonyvSablon>> GetAllAsync()
    {
        var sablonok = new List<JegyzokonyvSablon>();

        if (!Directory.Exists(_sablonMappa))
            return Task.FromResult(sablonok);

        foreach (var fajl in Directory.GetFiles(_sablonMappa, "*.docx"))
        {
            var fajlNev = Path.GetFileName(fajl);
            var fajlInfo = new FileInfo(fajl);
            
            // Sablon info fájlból vagy fájlnévből
            var sablon = ParseSablonInfo(fajlNev, fajlInfo);
            sablonok.Add(sablon);
        }

        return Task.FromResult(sablonok.OrderBy(s => s.Kategoria).ThenBy(s => s.Nev).ToList());
    }

    public Task<JegyzokonyvSablon?> GetByIdAsync(string id)
    {
        var fajlNev = $"{id}.docx";
        var fajlPath = Path.Combine(_sablonMappa, fajlNev);
        
        if (!File.Exists(fajlPath))
            return Task.FromResult<JegyzokonyvSablon?>(null);

        var fajlInfo = new FileInfo(fajlPath);
        return Task.FromResult<JegyzokonyvSablon?>(ParseSablonInfo(fajlNev, fajlInfo));
    }

    public string GetSablonPath(string fajlNev)
    {
        return Path.Combine(_sablonMappa, fajlNev);
    }

    private JegyzokonyvSablon ParseSablonInfo(string fajlNev, FileInfo fajlInfo)
    {
        // Fájlnév alapján kategorizálás
        // Példa: VBF_KIF_MINTA.docx → Kategória: VBF, Név: Kisfeszültségű
        var nev = Path.GetFileNameWithoutExtension(fajlNev);
        var kategoria = nev.Split('_').FirstOrDefault() ?? "Egyéb";
        
        // Szép név generálása
        var szepNev = nev switch
        {
            "VBF_KIF_MINTA" => "Kisfeszültségű felülvizsgálat",
            "VBF_NAF_MINTA" => "Nagyfeszültségű felülvizsgálat",
            "TUZV_MINTA" => "Tűzvédelmi jegyzőkönyv",
            "ERINT_MINTA" => "Érintésvédelmi mérés",
            "VILLAM_MINTA" => "Villámvédelmi felülvizsgálat",
            _ => nev.Replace("_", " ")
        };

        return new JegyzokonyvSablon
        {
            Id = nev,
            Nev = szepNev,
            FajlNev = fajlNev,
            Kategoria = kategoria,
            UtolsoModositas = fajlInfo.LastWriteTime,
            Aktiv = true
        };
    }
}