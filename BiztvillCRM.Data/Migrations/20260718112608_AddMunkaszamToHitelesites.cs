using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMunkaszamToHitelesites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MunkaszamId",
                table: "Hitelesitesek",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hitelesitesek_MunkaszamId",
                table: "Hitelesitesek",
                column: "MunkaszamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hitelesitesek_Munkaszamok_MunkaszamId",
                table: "Hitelesitesek",
                column: "MunkaszamId",
                principalTable: "Munkaszamok",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hitelesitesek_Munkaszamok_MunkaszamId",
                table: "Hitelesitesek");

            migrationBuilder.DropIndex(
                name: "IX_Hitelesitesek_MunkaszamId",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "MunkaszamId",
                table: "Hitelesitesek");
        }
    }
}
