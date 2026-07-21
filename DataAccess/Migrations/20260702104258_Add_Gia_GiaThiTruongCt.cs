using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Gia_GiaThiTruongCt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GiaBaoCao",
                table: "GiaThiTruongCts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GiaKyNay",
                table: "GiaThiTruongCts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GiaKyTruoc",
                table: "GiaThiTruongCts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MucTangGiam",
                table: "GiaThiTruongCts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STTSapXep",
                table: "GiaThiTruongCts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TyLeTangGiam",
                table: "GiaThiTruongCts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaBaoCao",
                table: "GiaThiTruongCts");

            migrationBuilder.DropColumn(
                name: "GiaKyNay",
                table: "GiaThiTruongCts");

            migrationBuilder.DropColumn(
                name: "GiaKyTruoc",
                table: "GiaThiTruongCts");

            migrationBuilder.DropColumn(
                name: "MucTangGiam",
                table: "GiaThiTruongCts");

            migrationBuilder.DropColumn(
                name: "STTSapXep",
                table: "GiaThiTruongCts");

            migrationBuilder.DropColumn(
                name: "TyLeTangGiam",
                table: "GiaThiTruongCts");
        }
    }
}
