// BiztvillCRM.Shared/Models/FelhasznaloErtesitesBeallitas.cs
namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Felhasználói értesítési beállítások a lejáró képzésekről
/// </summary>
public class FelhasznaloErtesitesBeallitas
{
    public int Id { get; set; }
    public int FelhasznaloId { get; set; }
    
    /// <summary>Lejáró képzés popup engedélyezve</summary>
    public bool PopupEngedelyezve { get; set; } = true;
    
    /// <summary>Email értesítés engedélyezve</summary>
    public bool EmailEngedelyezve { get; set; } = true;
    
    /// <summary>Szüneteltetés eddig (null = nincs szüneteltetés)</summary>
    public DateTime? SzuneteltetesDatum { get; set; }
    
    /// <summary>Utolsó popup megjelenítés dátuma (naponta egyszer)</summary>
    public DateTime? UtolsoPopupDatum { get; set; }
}