using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJogszabalyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cegek", x => x.Id);
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
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
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
                    Rovidites = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
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
                name: "Kepzesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LejaratDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resztvevo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kepzesek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeresTipusok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ErvenyessegHonap = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeresTipusok", x => x.Id);
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
                name: "Ugyszamok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Szam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Targy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UgyfelId = table.Column<int>(type: "int", nullable: true),
                    HatosagId = table.Column<int>(type: "int", nullable: true),
                    Beerkezett = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Hatarido = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Lezart = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ugyszamok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ugyszamok_Hatosagok_HatosagId",
                        column: x => x.HatosagId,
                        principalTable: "Hatosagok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ugyszamok_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                name: "Hitelesitesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EszkozId = table.Column<int>(type: "int", nullable: false),
                    HatosagId = table.Column<int>(type: "int", nullable: true),
                    Ugyszam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LejaratDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HitelesitesStatusz = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hitelesitesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hitelesitesek_Eszkozok_EszkozId",
                        column: x => x.EszkozId,
                        principalTable: "Eszkozok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hitelesitesek_Hatosagok_HatosagId",
                        column: x => x.HatosagId,
                        principalTable: "Hatosagok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                name: "Karbantartasok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EszkozId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Leiras = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Elvegzo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Karbantartasok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Karbantartasok_Eszkozok_EszkozId",
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
                    EszkozId = table.Column<int>(type: "int", nullable: false),
                    MeresTipusId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eredmeny = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MeresStatusz = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meresek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meresek_Eszkozok_EszkozId",
                        column: x => x.EszkozId,
                        principalTable: "Eszkozok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Meresek_MeresTipusok_MeresTipusId",
                        column: x => x.MeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_Hitelesitesek_EszkozId",
                table: "Hitelesitesek",
                column: "EszkozId");

            migrationBuilder.CreateIndex(
                name: "IX_Hitelesitesek_HatosagId",
                table: "Hitelesitesek",
                column: "HatosagId");

            migrationBuilder.CreateIndex(
                name: "IX_Kalibraciok_EszkozId",
                table: "Kalibraciok",
                column: "EszkozId");

            migrationBuilder.CreateIndex(
                name: "IX_Karbantartasok_EszkozId",
                table: "Karbantartasok",
                column: "EszkozId");

            migrationBuilder.CreateIndex(
                name: "IX_Meresek_EszkozId",
                table: "Meresek",
                column: "EszkozId");

            migrationBuilder.CreateIndex(
                name: "IX_Meresek_MeresTipusId",
                table: "Meresek",
                column: "MeresTipusId");

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
                name: "IX_Ugyfelek_CegId",
                table: "Ugyfelek",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_Ugyszamok_HatosagId",
                table: "Ugyszamok",
                column: "HatosagId");

            migrationBuilder.CreateIndex(
                name: "IX_Ugyszamok_UgyfelId",
                table: "Ugyszamok",
                column: "UgyfelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Hitelesitesek");

            migrationBuilder.DropTable(
                name: "Jogszabalyok");

            migrationBuilder.DropTable(
                name: "Kalibraciok");

            migrationBuilder.DropTable(
                name: "Karbantartasok");

            migrationBuilder.DropTable(
                name: "Kepzesek");

            migrationBuilder.DropTable(
                name: "Meresek");

            migrationBuilder.DropTable(
                name: "Tanusitvanyok");

            migrationBuilder.DropTable(
                name: "Terminalok");

            migrationBuilder.DropTable(
                name: "Ugyszamok");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Eszkozok");

            migrationBuilder.DropTable(
                name: "MeresTipusok");

            migrationBuilder.DropTable(
                name: "Hatosagok");

            migrationBuilder.DropTable(
                name: "Gyartok");

            migrationBuilder.DropTable(
                name: "Telephelyek");

            migrationBuilder.DropTable(
                name: "Ugyfelek");

            migrationBuilder.DropTable(
                name: "Cegek");
        }
    }
}
