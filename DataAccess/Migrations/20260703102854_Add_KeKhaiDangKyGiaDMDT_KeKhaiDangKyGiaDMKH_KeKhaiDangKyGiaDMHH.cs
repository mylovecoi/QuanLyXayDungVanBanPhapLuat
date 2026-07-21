using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_KeKhaiDangKyGiaDMDT_KeKhaiDangKyGiaDMKH_KeKhaiDangKyGiaDMHH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeKhaiDangKyGiaDMDTs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoanhNghiepQuanLyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenDT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeKhaiDangKyGiaDMDTs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeKhaiDangKyGiaDMHHs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoanhNghiepQuanLyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDVCU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenDvCungUng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuyCachChatLuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHH_BTC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeKhaiDangKyGiaDMHHs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeKhaiDangKyGiaDMKHs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoanhNghiepQuanLyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaKH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenKH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeKhaiDangKyGiaDMKHs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeKhaiDangKyGiaDMDTs");

            migrationBuilder.DropTable(
                name: "KeKhaiDangKyGiaDMHHs");

            migrationBuilder.DropTable(
                name: "KeKhaiDangKyGiaDMKHs");
        }
    }
}
