using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiztvillCRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHitelesitesFajlok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MunkalapPath",
                table: "Hitelesitesek",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BizonyitvanyPath",
                table: "Hitelesitesek",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MunkalapPath",
                table: "Hitelesitesek");

            migrationBuilder.DropColumn(
                name: "BizonyitvanyPath",
                table: "Hitelesitesek");
        }
    }
}
