using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhMucPhiLePhi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TiLeChietKhau",
                table: "HoSoCCCTChiPhis",
                newName: "TyLeVuotMuc");

            migrationBuilder.RenameColumn(
                name: "ChiPhi",
                table: "HoSoCCCTChiPhis",
                newName: "PhiToiDa");

            migrationBuilder.RenameColumn(
                name: "TyLe",
                table: "DanhMucPhiLePhis",
                newName: "TyLeVuotMuc");

            migrationBuilder.AddColumn<string>(
                name: "DonViTinh",
                table: "HoSoCCCTChiPhis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "GiaTriVuotMuc",
                table: "HoSoCCCTChiPhis",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PhiCoDinh",
                table: "HoSoCCCTChiPhis",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SoLuongToiDa",
                table: "HoSoCCCTChiPhis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SoLuongToiDa",
                table: "DanhMucPhiLePhis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonViTinh",
                table: "HoSoCCCTChiPhis");

            migrationBuilder.DropColumn(
                name: "GiaTriVuotMuc",
                table: "HoSoCCCTChiPhis");

            migrationBuilder.DropColumn(
                name: "PhiCoDinh",
                table: "HoSoCCCTChiPhis");

            migrationBuilder.DropColumn(
                name: "SoLuongToiDa",
                table: "HoSoCCCTChiPhis");

            migrationBuilder.DropColumn(
                name: "SoLuongToiDa",
                table: "DanhMucPhiLePhis");

            migrationBuilder.RenameColumn(
                name: "TyLeVuotMuc",
                table: "HoSoCCCTChiPhis",
                newName: "TiLeChietKhau");

            migrationBuilder.RenameColumn(
                name: "PhiToiDa",
                table: "HoSoCCCTChiPhis",
                newName: "ChiPhi");

            migrationBuilder.RenameColumn(
                name: "TyLeVuotMuc",
                table: "DanhMucPhiLePhis",
                newName: "TyLe");
        }
    }
}
