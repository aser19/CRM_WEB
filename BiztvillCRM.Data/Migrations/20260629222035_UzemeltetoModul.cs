using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class UzemeltetoModul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UzemeltetoSablonok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    JogszabalyiHivatkozas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EllenorzesiIdoszakHonap = table.Column<int>(type: "int", nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    LetrehozoFelhasznaloId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UzemeltetoSablonok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UzemeltetoSablonok_AspNetUsers_LetrehozoFelhasznaloId",
                        column: x => x.LetrehozoFelhasznaloId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UzemeltetoSablonok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UzemeltetoAdatok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UzemeltetoSablonId = table.Column<int>(type: "int", nullable: false),
                    RogzitesDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KovetkezoEsedekesseg = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statusz = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    RogzitoFelhasznaloId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MezoErtekekJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Megjegyzes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UzemeltetoAdatok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UzemeltetoAdatok_AspNetUsers_RogzitoFelhasznaloId",
                        column: x => x.RogzitoFelhasznaloId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UzemeltetoAdatok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UzemeltetoAdatok_UzemeltetoSablonok_UzemeltetoSablonId",
                        column: x => x.UzemeltetoSablonId,
                        principalTable: "UzemeltetoSablonok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UzemeltetoSablonMezok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UzemeltetoSablonId = table.Column<int>(type: "int", nullable: false),
                    MezoNev = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MezoTipus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Kotelezo = table.Column<bool>(type: "bit", nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    AlapErtek = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sugo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidaciosSzabaly = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UzemeltetoSablonMezok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UzemeltetoSablonMezok_UzemeltetoSablonok_UzemeltetoSablonId",
                        column: x => x.UzemeltetoSablonId,
                        principalTable: "UzemeltetoSablonok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoAdatok_Aktiv",
                table: "UzemeltetoAdatok",
                column: "Aktiv");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoAdatok_CegId",
                table: "UzemeltetoAdatok",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoAdatok_KovetkezoEsedekesseg",
                table: "UzemeltetoAdatok",
                column: "KovetkezoEsedekesseg");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoAdatok_RogzitesDatum",
                table: "UzemeltetoAdatok",
                column: "RogzitesDatum");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoAdatok_RogzitoFelhasznaloId",
                table: "UzemeltetoAdatok",
                column: "RogzitoFelhasznaloId");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoAdatok_UzemeltetoSablonId",
                table: "UzemeltetoAdatok",
                column: "UzemeltetoSablonId");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonMezok_UzemeltetoSablonId_Sorrend",
                table: "UzemeltetoSablonMezok",
                columns: new[] { "UzemeltetoSablonId", "Sorrend" });

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonok_Aktiv",
                table: "UzemeltetoSablonok",
                column: "Aktiv");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonok_CegId",
                table: "UzemeltetoSablonok",
                column: "CegId");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonok_LetrehozoFelhasznaloId",
                table: "UzemeltetoSablonok",
                column: "LetrehozoFelhasznaloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UzemeltetoAdatok");

            migrationBuilder.DropTable(
                name: "UzemeltetoSablonMezok");

            migrationBuilder.DropTable(
                name: "UzemeltetoSablonok");
        }
    }
}
