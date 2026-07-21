using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDanhMucDonVi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChucVu",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GioiTinh",
                table: "DanhMucCanBos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LoaiLaoDong",
                table: "DanhMucCanBos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MucPhiBaoHiemTrachNhiem",
                table: "DanhMucCanBos",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKyHopDongLaoDong",
                table: "DanhMucCanBos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayQuyetDinhBoNhiem",
                table: "DanhMucCanBos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayQuyetDinhCapThe",
                table: "DanhMucCanBos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayQuyetDinhDung",
                table: "DanhMucCanBos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTuyenDung",
                table: "DanhMucCanBos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoHopDongLaoDong",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoQuyetDinhBoNhiem",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoQuyetDinhCapThe",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoQuyetDinhDung",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoTheCongChungVien",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienBHXH",
                table: "DanhMucCanBos",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienBHYT",
                table: "DanhMucCanBos",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TrinhDoChuyenMon",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ViTriViecLam",
                table: "DanhMucCanBos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChucVu",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "LoaiLaoDong",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "MucPhiBaoHiemTrachNhiem",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "NgayKyHopDongLaoDong",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "NgayQuyetDinhBoNhiem",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "NgayQuyetDinhCapThe",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "NgayQuyetDinhDung",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "NgayTuyenDung",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "SoHopDongLaoDong",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "SoQuyetDinhBoNhiem",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "SoQuyetDinhCapThe",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "SoQuyetDinhDung",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "SoTheCongChungVien",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "SoTienBHXH",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "SoTienBHYT",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "TrinhDoChuyenMon",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "ViTriViecLam",
                table: "DanhMucCanBos");
        }
    }
}
