using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Level_DanhMucGiaThueTaiNguyen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "DanhMucGiaThueTaiNguyenCts");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "ChiTietGiaThueTaiNguyens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "DanhMucGiaThueTaiNguyenCts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "ChiTietGiaThueTaiNguyens",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
