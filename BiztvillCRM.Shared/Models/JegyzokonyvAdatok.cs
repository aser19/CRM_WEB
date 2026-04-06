namespace BiztvillCRM.Shared.Models;

public class JegyzokonyvAdatok
{
    public string JegyzokonyvSzam { get; set; } = "";
    public string CegNev { get; set; } = "";
    public string CegCim { get; set; } = "";
    public string VizsgalatHelye { get; set; } = "";
    public string VizsgalatTargya { get; set; } = "";
    public string VizsgaltBerendezes { get; set; } = "";
    public string Megrendelo { get; set; } = "";
    public string VizsgalatIdotartama { get; set; } = "";
    public string Eredmeny { get; set; } = "";
    public string Megjegyzes { get; set; } = "";
    public string UzemiKisero { get; set; } = "";
    public string KapcsolatTarto { get; set; } = "";
    
    // Felelős felülvizsgáló
    public string FelulvizsgaloNev { get; set; } = "";
    public string FelulvizsgaloBizonyitvany { get; set; } = "";
    public string FelulvizsgaloKepzes { get; set; } = "";
    
    // Segítő felülvizsgáló
    public string SegitoFelulvizsgalo { get; set; } = "";
    public string SegitoBizonyitvany { get; set; } = "";
    public string SegitoKepzes { get; set; } = "";
    
    // Ellenőr
    public string Ellenor { get; set; } = "";
    public string EllenorBizonyitvany { get; set; } = "";
    public string EllenorKepzes { get; set; } = "";
    
    // === MINŐSÍTŐ IRAT - 2. OLDAL ===
    public string HibakB { get; set; } = "";           // B) Életveszélyes hibák
    public string HibakC { get; set; } = "";           // C) Súlyos hibák
    public string HibakD { get; set; } = "";           // D) Karbantartáskor javítandó
    public string HibakE { get; set; } = "";           // E) Felújításkor javítandó
    public string VegsoMinosites { get; set; } = "";   // Végső minősítés
    public string MellekletekSzama { get; set; } = ""; // Mellékletek száma
    public string HibavedelmiJkv { get; set; } = "";
    public string AvkJegyzokonyv { get; set; } = "";
}