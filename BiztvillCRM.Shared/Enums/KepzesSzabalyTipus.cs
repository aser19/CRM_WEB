namespace BiztvillCRM.Shared.Enums;

/// <summary>
/// Képzések közötti szabály típusa
/// </summary>
public enum KepzesSzabalyTipus
{
    /// <summary>A forrás képzés/továbbképzés megújítja a cél képzést</summary>
    Megujit = 1,
    
    /// <summary>A forrás képzés megléte felment a cél képzés továbbképzése alól</summary>
    Felment = 2,
    
    /// <summary>A forrás képzés előfeltétele a cél képzésnek</summary>
    Elofeltetel = 3
}