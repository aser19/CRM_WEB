using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Cég (tenant) törzsadatai - multi-tenant rendszer alapja.</summary>
public class Ceg
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Adoszam { get; set; }
    public string? Cim { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public string? Weboldal { get; set; }
    public bool Aktiv { get; set; } = true;    /// <summary>Maximum 3 karakteres matrica előtag (pl. "BTV", "AB", "HK2")</summary>
    [StringLength(3, ErrorMessage = "A matrica előtag maximum 3 karakter lehet!")]
    [RegularExpression(@"^[A-Z0-9]{1,3}$", ErrorMessage = "A matrica előtag 1-3 nagybetű vagy szám lehet (pl. BTV, AB, HK2)!")]
    public string? MatricaElotag { get; set; }

    /// <summary>Tevékenységi körök (üzleti kategorizálás)</summary>
    public TevekenysegTipus Tevekenyseg { get; set; } = TevekenysegTipus.Nincs;

    /// <summary>Aktív modul jogosultságok (funkcióhoz való hozzáférés)</summary>
    public ModulJogosultsag AktivModulok { get; set; } = ModulJogosultsag.Ugyfelek;

    // Navigációs property-k
    public List<Felhasznalo> Felhasznalok { get; set; } = new();
    public List<FelhasznaloCeg> FelhasznaloCegek { get; set; } = new();
    public List<Ugyfel> Ugyfelek { get; set; } = new();  // ← ezt add vissza!

    /// <summary>NAV Online Számla API - technikai felhasználó beállítások (cégenkénti)</summary>
    public string? NavLoginName { get; set; }
    public string? NavPassword { get; set; }      // titkosítva tárolva
    public string? NavXmlSignKey { get; set; }    // titkosítva tárolva
    public string? NavTaxNumber { get; set; }     // 8 jegyű adószám
    public bool NavTesztKornyezet { get; set; } = true; // false = éles

    /// <summary>Cégbélyegző kép fájl elérési útja (relatív a Uploads mappához). Ha ki van töltve, a jegyzőkönyvekben/PDF-ekben az aláírás mellett megjelenik.</summary>
    public string? BelyegzoPath { get; set; }

}