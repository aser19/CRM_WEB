using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class UgyfelLekerdezesiToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlapertelmezettEmailBeallitasok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErtesitesTipusok = table.Column<int>(type: "int", nullable: false),
                    CimzettTipusok = table.Column<int>(type: "int", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlapertelmezettEmailBeallitasok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutomatikaFutasNaplok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FutasiIdo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sikeres = table.Column<bool>(type: "bit", nullable: false),
                    FeldolgozottHitelesitesek = table.Column<int>(type: "int", nullable: false),
                    FeldolgozottMeresek = table.Column<int>(type: "int", nullable: false),
                    KuldottEmailek = table.Column<int>(type: "int", nullable: false),
                    SikertelenEmailek = table.Column<int>(type: "int", nullable: false),
                    Hiba = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomatikaFutasNaplok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvkVedelemTipusok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipusKod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    In = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    IDn = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Un = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Polusszam = table.Column<int>(type: "int", nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvkVedelemTipusok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cegek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Adoszam = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Cim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Weboldal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    MatricaElotag = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Tevekenyseg = table.Column<int>(type: "int", nullable: false),
                    AktivModulok = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cegek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailKuldesNaplok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kuldve = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: true),
                    Tipus = table.Column<int>(type: "int", nullable: false),
                    Cimzett = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Targy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Sikeres = table.Column<bool>(type: "bit", nullable: false),
                    Hiba = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HitelesitesId = table.Column<int>(type: "int", nullable: true),
                    MeresId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailKuldesNaplok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErintesvedelmiModOsztalyok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErintesvedelmiModOsztalyok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EszkozTipusok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HitelesitesiIdotartamHonap = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EszkozTipusok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FelhasznaloErtesitesBeallitasok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FelhasznaloId = table.Column<int>(type: "int", nullable: false),
                    PopupEngedelyezve = table.Column<bool>(type: "bit", nullable: false),
                    EmailEngedelyezve = table.Column<bool>(type: "bit", nullable: false),
                    SzuneteltetesDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtolsoPopupDatum = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FelhasznaloErtesitesBeallitasok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gyartok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Orszag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Weboldal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Tevekenyseg = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gyartok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hatosagok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rovidites = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Cim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Weboldal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hatosagok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jogszabalyok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Szam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Tipus = table.Column<int>(type: "int", nullable: false),
                    Terulet = table.Column<int>(type: "int", nullable: false),
                    HatalyosKezdet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HatalyosVege = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogszabalyok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KarbantartasTipusok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsmetlodesHonap = table.Column<int>(type: "int", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KarbantartasTipusok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KepzesTipusok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lejar = table.Column<bool>(type: "bit", nullable: false),
                    LejaratEvek = table.Column<int>(type: "int", nullable: true),
                    TovabbkepzesKotelezo = table.Column<bool>(type: "bit", nullable: false),
                    TovabbkepzesEvek = table.Column<int>(type: "int", nullable: true),
                    TovabbkepzesCsakFelulvizsgalonak = table.Column<bool>(type: "bit", nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KepzesTipusok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KotelezoHitelesitesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Megnevezes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JogszabalyiHivatkozas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HitelesitesiIdoszakHonap = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KotelezoHitelesitesek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeresTipusok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ErvenyessegHonap = table.Column<int>(type: "int", nullable: true),
                    JegyzokonyvPrefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "JKV"),
                    SablonId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OcrModelId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MellekletTipusKod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeresTipusok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmtpBeallitasok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SzerverCim = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    TitkositasTipus = table.Column<int>(type: "int", nullable: false),
                    SslHasznalata = table.Column<bool>(type: "bit", nullable: false),
                    FelhasznaloNev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Jelszo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KuldoNev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KuldoEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    EmailFutasiOra = table.Column<int>(type: "int", nullable: false),
                    EmailFutasiPerc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmtpBeallitasok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TularamvedelemTipusok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NevlegesAram = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KarakterisztikaFeluliras = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TularamvedelemTipusok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtolsoBelepes = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Beosztas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailBeallitasok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    ErtesitesTipusok = table.Column<int>(type: "int", nullable: false),
                    CimzettTipusok = table.Column<int>(type: "int", nullable: false),
                    EgyediEmailCimek = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailBeallitasok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailBeallitasok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailSablonok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tipus = table.Column<int>(type: "int", nullable: false),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Targy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Szoveg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSablonok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailSablonok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EszkozSablonok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CegId = table.Column<int>(type: "int", nullable: true),
                    Megnevezes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Azonosito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VedelmiOsztaly = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Telj = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Megtekint = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EszkozSablonok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EszkozSablonok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Felulvizsgalok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Jogosultsag = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Felulvizsgalok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Felulvizsgalok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MunkaszamSzamlalok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    Ev = table.Column<int>(type: "int", nullable: false),
                    UtolsoSorszam = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunkaszamSzamlalok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MunkaszamSzamlalok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ugyfelek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Adoszam = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Cim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UgyfelTipus = table.Column<int>(type: "int", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Tevekenyseg = table.Column<int>(type: "int", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ugyfelek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ugyfelek_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kepzesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Jogosultsag = table.Column<int>(type: "int", nullable: false),
                    KepzesTipusId = table.Column<int>(type: "int", nullable: true),
                    BizonyitvanySzam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BizonyitvanyKelte = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BizonyitvanyLejarat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TovabbkepzesSzam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UtolsoTovabbkepzes = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FelujtoKepzesSzam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KepzesLejarat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kepzesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kepzesek_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kepzesek_KepzesTipusok_KepzesTipusId",
                        column: x => x.KepzesTipusId,
                        principalTable: "KepzesTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KepzesSzabalyok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipus = table.Column<int>(type: "int", nullable: false),
                    ForrasKepzesTipusId = table.Column<int>(type: "int", nullable: false),
                    CelKepzesTipusId = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KepzesSzabalyok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KepzesSzabalyok_KepzesTipusok_CelKepzesTipusId",
                        column: x => x.CelKepzesTipusId,
                        principalTable: "KepzesTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KepzesSzabalyok_KepzesTipusok_ForrasKepzesTipusId",
                        column: x => x.ForrasKepzesTipusId,
                        principalTable: "KepzesTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JegyzokonyvSablonTetelek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeresTipusId = table.Column<int>(type: "int", nullable: false),
                    OldalSzam = table.Column<int>(type: "int", nullable: false),
                    Kategoria = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    Felirat = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LehetsegesErtekek = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AlapertelmezettErtek = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VanMegjegyzesMezo = table.Column<bool>(type: "bit", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JegyzokonyvSablonTetelek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JegyzokonyvSablonTetelek_MeresTipusok_MeresTipusId",
                        column: x => x.MeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeresTipusJogszabalyok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeresTipusId = table.Column<int>(type: "int", nullable: false),
                    JogszabalyId = table.Column<int>(type: "int", nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeresTipusJogszabalyok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeresTipusJogszabalyok_Jogszabalyok_JogszabalyId",
                        column: x => x.JogszabalyId,
                        principalTable: "Jogszabalyok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeresTipusJogszabalyok_MeresTipusok_MeresTipusId",
                        column: x => x.MeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeresTipusKepzesKovetelemenyei",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeresTipusId = table.Column<int>(type: "int", nullable: false),
                    KepzesTipusId = table.Column<int>(type: "int", nullable: false),
                    SablonLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Kotelezo = table.Column<bool>(type: "bit", nullable: false),
                    AlternativaCsoport = table.Column<int>(type: "int", nullable: false),
                    Prioritas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeresTipusKepzesKovetelemenyei", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeresTipusKepzesKovetelemenyei_KepzesTipusok_KepzesTipusId",
                        column: x => x.KepzesTipusId,
                        principalTable: "KepzesTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeresTipusKepzesKovetelemenyei_MeresTipusok_MeresTipusId",
                        column: x => x.MeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VizsgalatiSablonok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeresTipusId = table.Column<int>(type: "int", nullable: false),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VizsgalatiSablonok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VizsgalatiSablonok_MeresTipusok_MeresTipusId",
                        column: x => x.MeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EszkozSablonAlkatreszek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EszkozSablonId = table.Column<int>(type: "int", nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    Megnevezes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Azonosito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VedelmiOsztaly = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Megtekint = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EszkozSablonAlkatreszek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EszkozSablonAlkatreszek_EszkozSablonok_EszkozSablonId",
                        column: x => x.EszkozSablonId,
                        principalTable: "EszkozSablonok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FelulvizsgaloKepzesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FelulvizsgaloId = table.Column<int>(type: "int", nullable: false),
                    KepzesTipusId = table.Column<int>(type: "int", nullable: true),
                    BizonyitvanySzam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BizonyitvanyKelte = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BizonyitvanyLejarat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FelulvizsgaloKepzesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FelulvizsgaloKepzesek_Felulvizsgalok_FelulvizsgaloId",
                        column: x => x.FelulvizsgaloId,
                        principalTable: "Felulvizsgalok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FelulvizsgaloKepzesek_KepzesTipusok_KepzesTipusId",
                        column: x => x.KepzesTipusId,
                        principalTable: "KepzesTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tanusitvanyok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Szam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KiadoDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LejaratDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanusitvanyok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tanusitvanyok_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Telephelyek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    Kapcsolattarto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telephelyek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telephelyek_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UgyfelLekerdezesiTokenek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Lejarat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtolsoHasznalat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UgyfelLekerdezesiTokenek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UgyfelLekerdezesiTokenek_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VizsgalatiSablonTetelErtekek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SablonId = table.Column<int>(type: "int", nullable: false),
                    TetelId = table.Column<int>(type: "int", nullable: false),
                    AlapertelmezettErtek = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VizsgalatiSablonTetelErtekek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VizsgalatiSablonTetelErtekek_JegyzokonyvSablonTetelek_TetelId",
                        column: x => x.TetelId,
                        principalTable: "JegyzokonyvSablonTetelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VizsgalatiSablonTetelErtekek_VizsgalatiSablonok_SablonId",
                        column: x => x.SablonId,
                        principalTable: "VizsgalatiSablonok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KepzesTovabbkepzesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FelulvizsgaloKepzesId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BizonyitvanySzam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Hely = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KepzesTovabbkepzesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KepzesTovabbkepzesek_FelulvizsgaloKepzesek_FelulvizsgaloKepzesId",
                        column: x => x.FelulvizsgaloKepzesId,
                        principalTable: "FelulvizsgaloKepzesek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Eszkozok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GyariSzam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tipus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kategoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GyartoId = table.Column<int>(type: "int", nullable: false),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    TelephelyId = table.Column<int>(type: "int", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eszkozok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eszkozok_Gyartok_GyartoId",
                        column: x => x.GyartoId,
                        principalTable: "Gyartok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Eszkozok_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Eszkozok_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Helyisegek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TelephelyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helyisegek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Helyisegek_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hitelesitesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UgyfelId = table.Column<int>(type: "int", nullable: true),
                    TelephelyId = table.Column<int>(type: "int", nullable: true),
                    EszkozTipusId = table.Column<int>(type: "int", nullable: false),
                    HatosagId = table.Column<int>(type: "int", nullable: true),
                    Darabszam = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LejaratDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HitelesitesStatusz = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EgyediLejaratok = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hitelesitesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hitelesitesek_EszkozTipusok_EszkozTipusId",
                        column: x => x.EszkozTipusId,
                        principalTable: "EszkozTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hitelesitesek_Hatosagok_HatosagId",
                        column: x => x.HatosagId,
                        principalTable: "Hatosagok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Hitelesitesek_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Hitelesitesek_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Karbantartasok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    TelephelyId = table.Column<int>(type: "int", nullable: false),
                    KarbantartasTipusId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Leiras = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Elvegzo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Elvegezve = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Karbantartasok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Karbantartasok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Karbantartasok_KarbantartasTipusok_KarbantartasTipusId",
                        column: x => x.KarbantartasTipusId,
                        principalTable: "KarbantartasTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Karbantartasok_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Karbantartasok_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kockazatertekelesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Megnevezes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErtekelesDatuma = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoFelulvizsgalat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KockazatiSzint = table.Column<int>(type: "int", nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Intezkedesek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FelelosNeve = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Statusz = table.Column<int>(type: "int", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    TelephelyId = table.Column<int>(type: "int", nullable: true),
                    CegId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kockazatertekelesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kockazatertekelesek_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kockazatertekelesek_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Kockazatertekelesek_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MunkavedelmiOktatasok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Megnevezes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OktatasDatuma = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoOktatas = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdoszakHonap = table.Column<int>(type: "int", nullable: false),
                    OktatoNeve = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    TelephelyId = table.Column<int>(type: "int", nullable: true),
                    CegId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunkavedelmiOktatasok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MunkavedelmiOktatasok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MunkavedelmiOktatasok_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MunkavedelmiOktatasok_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terminalok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Azonosito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpCim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TelephelyId = table.Column<int>(type: "int", nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminalok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terminalok_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Zonaterkepek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Megnevezes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZonaTipus = table.Column<int>(type: "int", nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FajlNev = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FajlUtvonal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErvenyessegKezdete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErvenyessegVege = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    TelephelyId = table.Column<int>(type: "int", nullable: true),
                    CegId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zonaterkepek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zonaterkepek_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zonaterkepek_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Zonaterkepek_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kalibraciok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EszkozId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Bizonyitvany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Elvegzo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Sikeres = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kalibraciok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kalibraciok_Eszkozok_EszkozId",
                        column: x => x.EszkozId,
                        principalTable: "Eszkozok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meresek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UgyfelId = table.Column<int>(type: "int", nullable: false),
                    TelephelyId = table.Column<int>(type: "int", nullable: false),
                    HelyisegId = table.Column<int>(type: "int", nullable: true),
                    MeresTipusId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eredmeny = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MeresStatusz = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    JegyzokonyvAdatokJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meresek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meresek_Helyisegek_HelyisegId",
                        column: x => x.HelyisegId,
                        principalTable: "Helyisegek",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Meresek_MeresTipusok_MeresTipusId",
                        column: x => x.MeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Meresek_Telephelyek_TelephelyId",
                        column: x => x.TelephelyId,
                        principalTable: "Telephelyek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Meresek_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MunkavedelmiOktatasResztvevok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MunkavedelmiOktatasId = table.Column<int>(type: "int", nullable: false),
                    Nev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beosztas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunkavedelmiOktatasResztvevok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MunkavedelmiOktatasResztvevok_MunkavedelmiOktatasok_MunkavedelmiOktatasId",
                        column: x => x.MunkavedelmiOktatasId,
                        principalTable: "MunkavedelmiOktatasok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MellekletJegyzokonyvek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeresId = table.Column<int>(type: "int", nullable: false),
                    Tipus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Szam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Statusz = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AdatokJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MellekletMeresId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MellekletJegyzokonyvek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MellekletJegyzokonyvek_Meresek_MellekletMeresId",
                        column: x => x.MellekletMeresId,
                        principalTable: "Meresek",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MellekletJegyzokonyvek_Meresek_MeresId",
                        column: x => x.MeresId,
                        principalTable: "Meresek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ErintesvedelmiModOsztalyok",
                columns: new[] { "Id", "Aktiv", "Leiras", "Letrehozva", "Nev", "Sorrend" },
                values: new object[,]
                {
                    { 1, true, "I. védelmi osztály, 0 ohm ellenállás", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "I/0Ω", 1 },
                    { 2, true, "II. védelmi osztály (kettős vagy megerősített szigetelés)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "II/Ω", 2 },
                    { 3, true, "III. védelmi osztály (SELV/PELV - kisfeszültség)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "III/Ω", 3 },
                    { 4, true, "I. védelmi osztály - alternatív", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "I/0Ω", 4 }
                });

            migrationBuilder.InsertData(
                table: "EszkozTipusok",
                columns: new[] { "Id", "Aktiv", "HitelesitesiIdotartamHonap", "Letrehozva", "Nev" },
                values: new object[,]
                {
                    { 1, true, 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kútoszlop" },
                    { 2, true, 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Szintmérő" },
                    { 3, true, 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Átfolyásmérő" },
                    { 4, true, 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tartály" }
                });

            migrationBuilder.InsertData(
                table: "KarbantartasTipusok",
                columns: new[] { "Id", "Aktiv", "IsmetlodesHonap", "Leiras", "Letrehozva", "Modositva", "Nev" },
                values: new object[,]
                {
                    { 1, true, 0, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Eseti karbantartás" },
                    { 2, true, 3, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Negyedéves karbantartás" },
                    { 3, true, 6, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Féléves karbantartás" },
                    { 4, true, 12, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Éves karbantartás" }
                });

            migrationBuilder.InsertData(
                table: "KepzesTipusok",
                columns: new[] { "Id", "Aktiv", "Label", "Leiras", "Lejar", "LejaratEvek", "Letrehozva", "Nev", "TovabbkepzesCsakFelulvizsgalonak", "TovabbkepzesEvek", "TovabbkepzesKotelezo" },
                values: new object[,]
                {
                    { 1, true, null, null, false, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alapító okirat szerinti képzés", false, null, false },
                    { 2, true, null, null, false, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ISO 9001:2015 átállási képzés", false, null, false },
                    { 3, true, null, null, false, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Minőségügyi auditor képzés", false, null, false },
                    { 4, true, null, null, false, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vezetői képzés", false, null, false }
                });

            migrationBuilder.InsertData(
                table: "TularamvedelemTipusok",
                columns: new[] { "Id", "Aktiv", "KarakterisztikaFeluliras", "Leiras", "Letrehozva", "Nev", "NevlegesAram" },
                values: new object[,]
                {
                    { 1, true, null, "Schneider Electric 16A típusú védőkapcsoló", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A9Z422316", 16m },
                    { 2, true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TDK-C16", 16m },
                    { 3, true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TDK-C25", 25m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CegId",
                table: "AspNetUsers",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AutomatikaFutasNaplok_FutasiIdo",
                table: "AutomatikaFutasNaplok",
                column: "FutasiIdo",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_AutomatikaFutasNaplok_Sikeres_FutasiIdo",
                table: "AutomatikaFutasNaplok",
                columns: new[] { "Sikeres", "FutasiIdo" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailBeallitasok_CegId",
                table: "EmailBeallitasok",
                column: "CegId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailKuldesNaplok_Kuldve",
                table: "EmailKuldesNaplok",
                column: "Kuldve");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSablonok_CegId",
                table: "EmailSablonok",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_Eszkozok_GyartoId",
                table: "Eszkozok",
                column: "GyartoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eszkozok_TelephelyId",
                table: "Eszkozok",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_Eszkozok_UgyfelId",
                table: "Eszkozok",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_EszkozSablonAlkatreszek_EszkozSablonId_Sorrend",
                table: "EszkozSablonAlkatreszek",
                columns: new[] { "EszkozSablonId", "Sorrend" });

            migrationBuilder.CreateIndex(
                name: "IX_EszkozSablonok_CegId_Megnevezes",
                table: "EszkozSablonok",
                columns: new[] { "CegId", "Megnevezes" });

            migrationBuilder.CreateIndex(
                name: "IX_FelhasznaloErtesitesBeallitasok_FelhasznaloId",
                table: "FelhasznaloErtesitesBeallitasok",
                column: "FelhasznaloId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Felulvizsgalok_CegId",
                table: "Felulvizsgalok",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_FelulvizsgaloKepzesek_FelulvizsgaloId_KepzesTipusId",
                table: "FelulvizsgaloKepzesek",
                columns: new[] { "FelulvizsgaloId", "KepzesTipusId" },
                unique: true,
                filter: "[KepzesTipusId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FelulvizsgaloKepzesek_KepzesTipusId",
                table: "FelulvizsgaloKepzesek",
                column: "KepzesTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_Helyisegek_TelephelyId",
                table: "Helyisegek",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_Hitelesitesek_EszkozTipusId",
                table: "Hitelesitesek",
                column: "EszkozTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_Hitelesitesek_HatosagId",
                table: "Hitelesitesek",
                column: "HatosagId");

            migrationBuilder.CreateIndex(
                name: "IX_Hitelesitesek_TelephelyId",
                table: "Hitelesitesek",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_Hitelesitesek_UgyfelId",
                table: "Hitelesitesek",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_JegyzokonyvSablonTetelek_MeresTipusId",
                table: "JegyzokonyvSablonTetelek",
                column: "MeresTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_Kalibraciok_EszkozId",
                table: "Kalibraciok",
                column: "EszkozId");

            migrationBuilder.CreateIndex(
                name: "IX_Karbantartasok_CegId",
                table: "Karbantartasok",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_Karbantartasok_KarbantartasTipusId",
                table: "Karbantartasok",
                column: "KarbantartasTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_Karbantartasok_TelephelyId",
                table: "Karbantartasok",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_Karbantartasok_UgyfelId",
                table: "Karbantartasok",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_Kepzesek_CegId",
                table: "Kepzesek",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_Kepzesek_KepzesTipusId",
                table: "Kepzesek",
                column: "KepzesTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_KepzesSzabalyok_CelKepzesTipusId",
                table: "KepzesSzabalyok",
                column: "CelKepzesTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_KepzesSzabalyok_ForrasKepzesTipusId",
                table: "KepzesSzabalyok",
                column: "ForrasKepzesTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_KepzesSzabalyok_Tipus_ForrasKepzesTipusId_CelKepzesTipusId",
                table: "KepzesSzabalyok",
                columns: new[] { "Tipus", "ForrasKepzesTipusId", "CelKepzesTipusId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KepzesTovabbkepzesek_FelulvizsgaloKepzesId",
                table: "KepzesTovabbkepzesek",
                column: "FelulvizsgaloKepzesId");

            migrationBuilder.CreateIndex(
                name: "IX_Kockazatertekelesek_CegId",
                table: "Kockazatertekelesek",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_Kockazatertekelesek_TelephelyId",
                table: "Kockazatertekelesek",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_Kockazatertekelesek_UgyfelId",
                table: "Kockazatertekelesek",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_MellekletJegyzokonyvek_MellekletMeresId",
                table: "MellekletJegyzokonyvek",
                column: "MellekletMeresId");

            migrationBuilder.CreateIndex(
                name: "IX_MellekletJegyzokonyvek_MeresId",
                table: "MellekletJegyzokonyvek",
                column: "MeresId");

            migrationBuilder.CreateIndex(
                name: "IX_Meresek_HelyisegId",
                table: "Meresek",
                column: "HelyisegId");

            migrationBuilder.CreateIndex(
                name: "IX_Meresek_MeresTipusId",
                table: "Meresek",
                column: "MeresTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_Meresek_TelephelyId",
                table: "Meresek",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_Meresek_UgyfelId",
                table: "Meresek",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_MeresTipusJogszabalyok_JogszabalyId",
                table: "MeresTipusJogszabalyok",
                column: "JogszabalyId");

            migrationBuilder.CreateIndex(
                name: "IX_MeresTipusJogszabalyok_MeresTipusId",
                table: "MeresTipusJogszabalyok",
                column: "MeresTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_MeresTipusKepzesKovetelemenyei_KepzesTipusId",
                table: "MeresTipusKepzesKovetelemenyei",
                column: "KepzesTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_MeresTipusKepzesKovetelemenyei_MeresTipusId_KepzesTipusId",
                table: "MeresTipusKepzesKovetelemenyei",
                columns: new[] { "MeresTipusId", "KepzesTipusId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MunkaszamSzamlalok_CegId_Ev",
                table: "MunkaszamSzamlalok",
                columns: new[] { "CegId", "Ev" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MunkavedelmiOktatasok_CegId",
                table: "MunkavedelmiOktatasok",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_MunkavedelmiOktatasok_TelephelyId",
                table: "MunkavedelmiOktatasok",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_MunkavedelmiOktatasok_UgyfelId",
                table: "MunkavedelmiOktatasok",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_MunkavedelmiOktatasResztvevok_MunkavedelmiOktatasId",
                table: "MunkavedelmiOktatasResztvevok",
                column: "MunkavedelmiOktatasId");

            migrationBuilder.CreateIndex(
                name: "IX_Tanusitvanyok_UgyfelId",
                table: "Tanusitvanyok",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_Telephelyek_UgyfelId",
                table: "Telephelyek",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminalok_TelephelyId",
                table: "Terminalok",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_TularamvedelemTipusok_Nev",
                table: "TularamvedelemTipusok",
                column: "Nev",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ugyfelek_CegId",
                table: "Ugyfelek",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_UgyfelLekerdezesiTokenek_UgyfelId",
                table: "UgyfelLekerdezesiTokenek",
                column: "UgyfelId");

            migrationBuilder.CreateIndex(
                name: "IX_VizsgalatiSablonok_MeresTipusId",
                table: "VizsgalatiSablonok",
                column: "MeresTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_VizsgalatiSablonTetelErtekek_SablonId",
                table: "VizsgalatiSablonTetelErtekek",
                column: "SablonId");

            migrationBuilder.CreateIndex(
                name: "IX_VizsgalatiSablonTetelErtekek_TetelId",
                table: "VizsgalatiSablonTetelErtekek",
                column: "TetelId");

            migrationBuilder.CreateIndex(
                name: "IX_Zonaterkepek_CegId",
                table: "Zonaterkepek",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_Zonaterkepek_TelephelyId",
                table: "Zonaterkepek",
                column: "TelephelyId");

            migrationBuilder.CreateIndex(
                name: "IX_Zonaterkepek_UgyfelId",
                table: "Zonaterkepek",
                column: "UgyfelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlapertelmezettEmailBeallitasok");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AutomatikaFutasNaplok");

            migrationBuilder.DropTable(
                name: "AvkVedelemTipusok");

            migrationBuilder.DropTable(
                name: "EmailBeallitasok");

            migrationBuilder.DropTable(
                name: "EmailKuldesNaplok");

            migrationBuilder.DropTable(
                name: "EmailSablonok");

            migrationBuilder.DropTable(
                name: "ErintesvedelmiModOsztalyok");

            migrationBuilder.DropTable(
                name: "EszkozSablonAlkatreszek");

            migrationBuilder.DropTable(
                name: "FelhasznaloErtesitesBeallitasok");

            migrationBuilder.DropTable(
                name: "Hitelesitesek");

            migrationBuilder.DropTable(
                name: "Kalibraciok");

            migrationBuilder.DropTable(
                name: "Karbantartasok");

            migrationBuilder.DropTable(
                name: "Kepzesek");

            migrationBuilder.DropTable(
                name: "KepzesSzabalyok");

            migrationBuilder.DropTable(
                name: "KepzesTovabbkepzesek");

            migrationBuilder.DropTable(
                name: "Kockazatertekelesek");

            migrationBuilder.DropTable(
                name: "KotelezoHitelesitesek");

            migrationBuilder.DropTable(
                name: "MellekletJegyzokonyvek");

            migrationBuilder.DropTable(
                name: "MeresTipusJogszabalyok");

            migrationBuilder.DropTable(
                name: "MeresTipusKepzesKovetelemenyei");

            migrationBuilder.DropTable(
                name: "MunkaszamSzamlalok");

            migrationBuilder.DropTable(
                name: "MunkavedelmiOktatasResztvevok");

            migrationBuilder.DropTable(
                name: "SmtpBeallitasok");

            migrationBuilder.DropTable(
                name: "Tanusitvanyok");

            migrationBuilder.DropTable(
                name: "Terminalok");

            migrationBuilder.DropTable(
                name: "TularamvedelemTipusok");

            migrationBuilder.DropTable(
                name: "UgyfelLekerdezesiTokenek");

            migrationBuilder.DropTable(
                name: "VizsgalatiSablonTetelErtekek");

            migrationBuilder.DropTable(
                name: "Zonaterkepek");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EszkozSablonok");

            migrationBuilder.DropTable(
                name: "EszkozTipusok");

            migrationBuilder.DropTable(
                name: "Hatosagok");

            migrationBuilder.DropTable(
                name: "Eszkozok");

            migrationBuilder.DropTable(
                name: "KarbantartasTipusok");

            migrationBuilder.DropTable(
                name: "FelulvizsgaloKepzesek");

            migrationBuilder.DropTable(
                name: "Meresek");

            migrationBuilder.DropTable(
                name: "Jogszabalyok");

            migrationBuilder.DropTable(
                name: "MunkavedelmiOktatasok");

            migrationBuilder.DropTable(
                name: "JegyzokonyvSablonTetelek");

            migrationBuilder.DropTable(
                name: "VizsgalatiSablonok");

            migrationBuilder.DropTable(
                name: "Gyartok");

            migrationBuilder.DropTable(
                name: "Felulvizsgalok");

            migrationBuilder.DropTable(
                name: "KepzesTipusok");

            migrationBuilder.DropTable(
                name: "Helyisegek");

            migrationBuilder.DropTable(
                name: "MeresTipusok");

            migrationBuilder.DropTable(
                name: "Telephelyek");

            migrationBuilder.DropTable(
                name: "Ugyfelek");

            migrationBuilder.DropTable(
                name: "Cegek");
        }
    }
}
