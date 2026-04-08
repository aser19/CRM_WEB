namespace BiztvillCRM.Shared.Enums;

/// <summary>Email címzett típusok - kit értesítsünk.</summary>
[Flags]
public enum EmailCimzettTipus
{
    Nincs = 0,
    Sajat = 1,              // Saját cég email címe
    EgyediCimek = 2,        // Manuálisan megadott email címek
    UgyfelEmail = 4,        // Ügyfél email címe
    TelephelyEmail = 8      // Telephely email címe
}