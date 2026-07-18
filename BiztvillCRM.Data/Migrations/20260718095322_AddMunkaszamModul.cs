using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMunkaszamModul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Munkaszamok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CegId = table.Column<int>(type: "int", nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modositva = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Szam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Megnevezes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Munkaszamok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Munkaszamok_Cegek_CegId",
                        column: x => x.CegId,
                        principalTable: "Cegek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Munkaszamok_CegId",
                table: "Munkaszamok",
                column: "CegId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Munkaszamok");
        }
    }
}
