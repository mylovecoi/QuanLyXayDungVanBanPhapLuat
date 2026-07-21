using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhMuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STTHienThi",
                table: "DanhMucNuocSachCts");

            migrationBuilder.DropColumn(
                name: "Style",
                table: "DanhMucNuocSachCts");

            migrationBuilder.AddColumn<string>(
                name: "MaNghe",
                table: "DanhMucGiaChungs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaNghe",
                table: "DanhMucGiaChungCts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaNghe",
                table: "DanhMucGiaChungs");

            migrationBuilder.DropColumn(
                name: "MaNghe",
                table: "DanhMucGiaChungCts");

            migrationBuilder.AddColumn<string>(
                name: "STTHienThi",
                table: "DanhMucNuocSachCts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "DanhMucNuocSachCts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
