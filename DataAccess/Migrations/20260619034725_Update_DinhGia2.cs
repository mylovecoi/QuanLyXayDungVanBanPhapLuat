using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DinhGia2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeExcel",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaDiaBan",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaDv",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "PhanLoaiHoSo",
                table: "DinhGias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeExcel",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDiaBan",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDv",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhanLoaiHoSo",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
