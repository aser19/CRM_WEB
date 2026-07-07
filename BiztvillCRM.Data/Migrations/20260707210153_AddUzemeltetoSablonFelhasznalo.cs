using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUzemeltetoSablonFelhasznalo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UzemeltetoSablonFelhasznalok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UzemeltetoSablonId = table.Column<int>(type: "int", nullable: false),
                    FelhasznaloId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HozzarendeloFelhasznaloId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UzemeltetoSablonFelhasznalok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UzemeltetoSablonFelhasznalok_AspNetUsers_FelhasznaloId",
                        column: x => x.FelhasznaloId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UzemeltetoSablonFelhasznalok_AspNetUsers_HozzarendeloFelhasznaloId",
                        column: x => x.HozzarendeloFelhasznaloId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UzemeltetoSablonFelhasznalok_UzemeltetoSablonok_UzemeltetoSablonId",
                        column: x => x.UzemeltetoSablonId,
                        principalTable: "UzemeltetoSablonok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonFelhasznalok_Aktiv",
                table: "UzemeltetoSablonFelhasznalok",
                column: "Aktiv");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonFelhasznalok_FelhasznaloId",
                table: "UzemeltetoSablonFelhasznalok",
                column: "FelhasznaloId");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonFelhasznalok_HozzarendeloFelhasznaloId",
                table: "UzemeltetoSablonFelhasznalok",
                column: "HozzarendeloFelhasznaloId");

            migrationBuilder.CreateIndex(
                name: "IX_UzemeltetoSablonFelhasznalok_UzemeltetoSablonId_FelhasznaloId",
                table: "UzemeltetoSablonFelhasznalok",
                columns: new[] { "UzemeltetoSablonId", "FelhasznaloId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UzemeltetoSablonFelhasznalok");
        }
    }
}
