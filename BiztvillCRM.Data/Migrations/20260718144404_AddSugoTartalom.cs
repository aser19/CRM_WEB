using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSugoTartalom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SugoKategoriak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SugoKategoriak", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SugoTemak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SugoKategoriaId = table.Column<int>(type: "int", nullable: false),
                    Cim = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Leiras = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SugoTemak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SugoTemak_SugoKategoriak_SugoKategoriaId",
                        column: x => x.SugoKategoriaId,
                        principalTable: "SugoKategoriak",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SugoTemak_SugoKategoriaId",
                table: "SugoTemak",
                column: "SugoKategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SugoTemak");

            migrationBuilder.DropTable(
                name: "SugoKategoriak");
        }
    }
}
