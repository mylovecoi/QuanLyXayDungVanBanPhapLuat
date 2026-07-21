using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DinhGiaCt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GiaLk",
                table: "DinhGiaCts",
                newName: "GiaLienKe");

            migrationBuilder.RenameColumn(
                name: "GiaKk",
                table: "DinhGiaCts",
                newName: "GiaKeKhai");

            migrationBuilder.AddColumn<string>(
                name: "DonViTinh",
                table: "DinhGiaCts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonViTinh",
                table: "DinhGiaCts");

            migrationBuilder.RenameColumn(
                name: "GiaLienKe",
                table: "DinhGiaCts",
                newName: "GiaLk");

            migrationBuilder.RenameColumn(
                name: "GiaKeKhai",
                table: "DinhGiaCts",
                newName: "GiaKk");
        }
    }
}
