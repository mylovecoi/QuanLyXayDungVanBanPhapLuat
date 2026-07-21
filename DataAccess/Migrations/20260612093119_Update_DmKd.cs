using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DmKd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhMucGiaHHDVChiTiets");

            migrationBuilder.DropTable(
                name: "DanhMucKinhDoanhChiTiets");

            migrationBuilder.DropTable(
                name: "DanhMucGiaHHDVs");

            migrationBuilder.RenameColumn(
                name: "TenNganh",
                table: "DanhMucKinhDoanhs",
                newName: "TenNghe");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "DanhMucKinhDoanhs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LoaiGia",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDv",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDvDongChuyen",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaHH_BTC",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaNghe",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhanLoai",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Report",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleGoc",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STTHienThi",
                table: "DanhMucKinhDoanhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "STTSapXep",
                table: "DanhMucKinhDoanhs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "LoaiGia",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "MaDv",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "MaDvDongChuyen",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "MaHH_BTC",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "MaNghe",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "PhanLoai",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "Report",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "RoleGoc",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "STTHienThi",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.DropColumn(
                name: "STTSapXep",
                table: "DanhMucKinhDoanhs");

            migrationBuilder.RenameColumn(
                name: "TenNghe",
                table: "DanhMucKinhDoanhs",
                newName: "TenNganh");

            migrationBuilder.CreateTable(
                name: "DanhMucGiaHHDVs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucGiaHHDVs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucKinhDoanhChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucKinhDoanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    LoaiGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDvDongChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHH_BTC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleGoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    TenNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucKinhDoanhChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucKinhDoanhChiTiets_DanhMucKinhDoanhs_DanhMucKinhDoanhId",
                        column: x => x.DanhMucKinhDoanhId,
                        principalTable: "DanhMucKinhDoanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucGiaHHDVChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucGiaHHDVId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    MaDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDvDongChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaGiaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHH_BTC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleGoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    TenGiaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucGiaHHDVChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucGiaHHDVChiTiets_DanhMucGiaHHDVs_DanhMucGiaHHDVId",
                        column: x => x.DanhMucGiaHHDVId,
                        principalTable: "DanhMucGiaHHDVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucGiaHHDVChiTiets_DanhMucGiaHHDVId",
                table: "DanhMucGiaHHDVChiTiets",
                column: "DanhMucGiaHHDVId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucKinhDoanhChiTiets_DanhMucKinhDoanhId",
                table: "DanhMucKinhDoanhChiTiets",
                column: "DanhMucKinhDoanhId");
        }
    }
}
