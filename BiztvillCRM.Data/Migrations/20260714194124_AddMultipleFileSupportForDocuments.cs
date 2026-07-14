using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleFileSupportForDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BizonyitvanyPaths",
                table: "Hitelesitesek",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MunkalapPaths",
                table: "Hitelesitesek",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BizonyitvanyPaths",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "MunkalapPaths",
                table: "Hitelesitesek");
        }
    }
}
