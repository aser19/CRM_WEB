using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class KarbantartasStatusz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lejarat",
                table: "UgyfelLekerdezesiTokenek",
                newName: "LejarDatum");

            migrationBuilder.RenameColumn(
                name: "Elvegezve",
                table: "Karbantartasok",
                newName: "Aktiv");

            migrationBuilder.AddColumn<string>(
                name: "AdatokJson",
                table: "VizsgalatiSablonok",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CegId",
                table: "VizsgalatiSablonok",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FelulvizsgalasraVar",
                table: "TularamvedelemTipusok",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FoMeres",
                table: "MeresTipusok",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Aktiv",
                table: "Meresek",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Statusz",
                table: "Karbantartasok",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CegId",
                table: "JegyzokonyvSablonTetelek",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Aktiv",
                table: "Hitelesitesek",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CsoportTagLejaratok",
                table: "Hitelesitesek",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EszkozAzonosito",
                table: "Hitelesitesek",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VanAvk",
                table: "ErintesvedelmiModOsztalyok",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VanPeFolyt",
                table: "ErintesvedelmiModOsztalyok",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KarbantartasId",
                table: "EmailKuldesNaplok",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavLoginName",
                table: "Cegek",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavPassword",
                table: "Cegek",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavTaxNumber",
                table: "Cegek",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NavTesztKornyezet",
                table: "Cegek",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NavXmlSignKey",
                table: "Cegek",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FelulvizsgalasraVar",
                table: "AvkVedelemTipusok",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FelhasznaloCegek",
                columns: table => new
                {
                    FelhasznaloId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    Szerep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hozzaadva = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FelhasznaloCegek", x => new { x.FelhasznaloId, x.CegId });
                    table.ForeignKey(
                        name: "FK_FelhasznaloCegek_AspNetUsers_FelhasznaloId",
                        column: x => x.FelhasznaloId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FelhasznaloCegek_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HitelesitesCsoportok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FoEszkozTipusId = table.Column<int>(type: "int", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitelesitesCsoportok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HitelesitesCsoportok_EszkozTipusok_FoEszkozTipusId",
                        column: x => x.FoEszkozTipusId,
                        principalTable: "EszkozTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JogszabalyTagek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Szin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogszabalyTagek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeresCsoportok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FoMeresTipusId = table.Column<int>(type: "int", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeresCsoportok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeresCsoportok_MeresTipusok_FoMeresTipusId",
                        column: x => x.FoMeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HitelesitesCsoportTagok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HitelesitesCsoportId = table.Column<int>(type: "int", nullable: false),
                    EszkozTipusId = table.Column<int>(type: "int", nullable: false),
                    Kotelezo = table.Column<bool>(type: "bit", nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitelesitesCsoportTagok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HitelesitesCsoportTagok_EszkozTipusok_EszkozTipusId",
                        column: x => x.EszkozTipusId,
                        principalTable: "EszkozTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HitelesitesCsoportTagok_HitelesitesCsoportok_HitelesitesCsoportId",
                        column: x => x.HitelesitesCsoportId,
                        principalTable: "HitelesitesCsoportok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JogszabalyTagKapcsolatok",
                columns: table => new
                {
                    JogszabalyId = table.Column<int>(type: "int", nullable: false),
                    TagekId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogszabalyTagKapcsolatok", x => new { x.JogszabalyId, x.TagekId });
                    table.ForeignKey(
                        name: "FK_JogszabalyTagKapcsolatok_JogszabalyTagek_TagekId",
                        column: x => x.TagekId,
                        principalTable: "JogszabalyTagek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JogszabalyTagKapcsolatok_Jogszabalyok_JogszabalyId",
                        column: x => x.JogszabalyId,
                        principalTable: "Jogszabalyok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeresCsoportTagok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeresCsoportId = table.Column<int>(type: "int", nullable: false),
                    MeresTipusId = table.Column<int>(type: "int", nullable: false),
                    Kotelezo = table.Column<bool>(type: "bit", nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeresCsoportTagok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeresCsoportTagok_MeresCsoportok_MeresCsoportId",
                        column: x => x.MeresCsoportId,
                        principalTable: "MeresCsoportok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeresCsoportTagok_MeresTipusok_MeresTipusId",
                        column: x => x.MeresTipusId,
                        principalTable: "MeresTipusok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "ErintesvedelmiModOsztalyok",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "VanAvk", "VanPeFolyt" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "ErintesvedelmiModOsztalyok",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "VanAvk", "VanPeFolyt" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "ErintesvedelmiModOsztalyok",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "VanAvk", "VanPeFolyt" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "ErintesvedelmiModOsztalyok",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "VanAvk", "VanPeFolyt" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "TularamvedelemTipusok",
                keyColumn: "Id",
                keyValue: 1,
                column: "FelulvizsgalasraVar",
                value: false);

            migrationBuilder.UpdateData(
                table: "TularamvedelemTipusok",
                keyColumn: "Id",
                keyValue: 2,
                column: "FelulvizsgalasraVar",
                value: false);

            migrationBuilder.UpdateData(
                table: "TularamvedelemTipusok",
                keyColumn: "Id",
                keyValue: 3,
                column: "FelulvizsgalasraVar",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_VizsgalatiSablonok_CegId",
                table: "VizsgalatiSablonok",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_JegyzokonyvSablonTetelek_CegId",
                table: "JegyzokonyvSablonTetelek",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_FelhasznaloCegek_CegId",
                table: "FelhasznaloCegek",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_HitelesitesCsoportok_FoEszkozTipusId",
                table: "HitelesitesCsoportok",
                column: "FoEszkozTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_HitelesitesCsoportTagok_EszkozTipusId",
                table: "HitelesitesCsoportTagok",
                column: "EszkozTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_HitelesitesCsoportTagok_HitelesitesCsoportId_EszkozTipusId",
                table: "HitelesitesCsoportTagok",
                columns: new[] { "HitelesitesCsoportId", "EszkozTipusId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JogszabalyTagKapcsolatok_TagekId",
                table: "JogszabalyTagKapcsolatok",
                column: "TagekId");

            migrationBuilder.CreateIndex(
                name: "IX_MeresCsoportok_FoMeresTipusId",
                table: "MeresCsoportok",
                column: "FoMeresTipusId");

            migrationBuilder.CreateIndex(
                name: "IX_MeresCsoportTagok_MeresCsoportId_MeresTipusId",
                table: "MeresCsoportTagok",
                columns: new[] { "MeresCsoportId", "MeresTipusId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeresCsoportTagok_MeresTipusId",
                table: "MeresCsoportTagok",
                column: "MeresTipusId");

            migrationBuilder.AddForeignKey(
                name: "FK_JegyzokonyvSablonTetelek_Cegek_CegId",
                table: "JegyzokonyvSablonTetelek",
                column: "CegId",
                principalTable: "Cegek",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VizsgalatiSablonok_Cegek_CegId",
                table: "VizsgalatiSablonok",
                column: "CegId",
                principalTable: "Cegek",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JegyzokonyvSablonTetelek_Cegek_CegId",
                table: "JegyzokonyvSablonTetelek");

            migrationBuilder.DropForeignKey(
                name: "FK_VizsgalatiSablonok_Cegek_CegId",
                table: "VizsgalatiSablonok");

            migrationBuilder.DropTable(
                name: "FelhasznaloCegek");

            migrationBuilder.DropTable(
                name: "HitelesitesCsoportTagok");

            migrationBuilder.DropTable(
                name: "JogszabalyTagKapcsolatok");

            migrationBuilder.DropTable(
                name: "MeresCsoportTagok");

            migrationBuilder.DropTable(
                name: "HitelesitesCsoportok");

            migrationBuilder.DropTable(
                name: "JogszabalyTagek");

            migrationBuilder.DropTable(
                name: "MeresCsoportok");

            migrationBuilder.DropIndex(
                name: "IX_VizsgalatiSablonok_CegId",
                table: "VizsgalatiSablonok");

            migrationBuilder.DropIndex(
                name: "IX_JegyzokonyvSablonTetelek_CegId",
                table: "JegyzokonyvSablonTetelek");

            migrationBuilder.DropColumn(
                name: "AdatokJson",
                table: "VizsgalatiSablonok");

            migrationBuilder.DropColumn(
                name: "CegId",
                table: "VizsgalatiSablonok");

            migrationBuilder.DropColumn(
                name: "FelulvizsgalasraVar",
                table: "TularamvedelemTipusok");

            migrationBuilder.DropColumn(
                name: "FoMeres",
                table: "MeresTipusok");

            migrationBuilder.DropColumn(
                name: "Aktiv",
                table: "Meresek");

            migrationBuilder.DropColumn(
                name: "Statusz",
                table: "Karbantartasok");

            migrationBuilder.DropColumn(
                name: "CegId",
                table: "JegyzokonyvSablonTetelek");

            migrationBuilder.DropColumn(
                name: "Aktiv",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "CsoportTagLejaratok",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "EszkozAzonosito",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "VanAvk",
                table: "ErintesvedelmiModOsztalyok");

            migrationBuilder.DropColumn(
                name: "VanPeFolyt",
                table: "ErintesvedelmiModOsztalyok");

            migrationBuilder.DropColumn(
                name: "KarbantartasId",
                table: "EmailKuldesNaplok");

            migrationBuilder.DropColumn(
                name: "NavLoginName",
                table: "Cegek");

            migrationBuilder.DropColumn(
                name: "NavPassword",
                table: "Cegek");

            migrationBuilder.DropColumn(
                name: "NavTaxNumber",
                table: "Cegek");

            migrationBuilder.DropColumn(
                name: "NavTesztKornyezet",
                table: "Cegek");

            migrationBuilder.DropColumn(
                name: "NavXmlSignKey",
                table: "Cegek");

            migrationBuilder.DropColumn(
                name: "FelulvizsgalasraVar",
                table: "AvkVedelemTipusok");

            migrationBuilder.RenameColumn(
                name: "LejarDatum",
                table: "UgyfelLekerdezesiTokenek",
                newName: "Lejarat");

            migrationBuilder.RenameColumn(
                name: "Aktiv",
                table: "Karbantartasok",
                newName: "Elvegezve");
        }
    }
}
