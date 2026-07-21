using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhMucPhiLePhi_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GiaTriVuotMuc",
                table: "HoSoCCCTChiPhis",
                newName: "NguongVuotMuc");

            migrationBuilder.RenameColumn(
                name: "GiaTriVuotMuc",
                table: "DanhMucPhiLePhis",
                newName: "NguongVuotMuc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NguongVuotMuc",
                table: "HoSoCCCTChiPhis",
                newName: "GiaTriVuotMuc");

            migrationBuilder.RenameColumn(
                name: "NguongVuotMuc",
                table: "DanhMucPhiLePhis",
                newName: "GiaTriVuotMuc");
        }
    }
}
