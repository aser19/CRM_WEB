using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Rb (robbanásbiztos) berendezés-lista beolvasása Excel fájlból.</summary>
public interface IRbExcelImportService
{
    /// <summary>Beolvassa a feltöltött Excel (.xlsx) tartalmát RbSor listává.</summary>
    /// <param name="fajlTartalom">A feltöltött fájl bájtjai.</param>
    List<RbSor> Beolvasas(byte[] fajlTartalom);
}
