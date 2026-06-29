using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class HitelesitesFileUploads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BizonyitvanyPath",
                table: "Hitelesitesek",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MunkalapPath",
                table: "Hitelesitesek",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BizonyitvanyPath",
                table: "FelulvizsgaloKepzesek",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlairasPath",
                table: "Felulvizsgalok",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BizonyitvanyPath",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "MunkalapPath",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "BizonyitvanyPath",
                table: "FelulvizsgaloKepzesek");

            migrationBuilder.DropColumn(
                name: "AlairasPath",
                table: "Felulvizsgalok");
        }
    }
}
