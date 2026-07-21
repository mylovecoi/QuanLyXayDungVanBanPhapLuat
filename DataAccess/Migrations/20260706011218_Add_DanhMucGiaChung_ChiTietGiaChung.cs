using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_DanhMucGiaChung_ChiTietGiaChung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietTienBanQuyens");

            migrationBuilder.DropTable(
                name: "DanhMucTienBanQuyens");

            migrationBuilder.CreateTable(
                name: "ChiTietGiaChungs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonGia1 = table.Column<double>(type: "float", nullable: false),
                    DonGia2 = table.Column<double>(type: "float", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietGiaChungs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietGiaChungs_DanhMucDonVis_DonViQuanLyId",
                        column: x => x.DonViQuanLyId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucGiaChungs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucGiaChungs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiaChungs_DonViQuanLyId",
                table: "ChiTietGiaChungs",
                column: "DonViQuanLyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietGiaChungs");

            migrationBuilder.DropTable(
                name: "DanhMucGiaChungs");

            migrationBuilder.CreateTable(
                name: "ChiTietTienBanQuyens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoiTuongSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonGia1 = table.Column<double>(type: "float", nullable: false),
                    DonGia2 = table.Column<double>(type: "float", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThueSuat = table.Column<double>(type: "float", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietTienBanQuyens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietTienBanQuyens_DanhMucDonVis_DonViQuanLyId",
                        column: x => x.DonViQuanLyId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucTienBanQuyens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoiTuongSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    Style = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucTienBanQuyens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietTienBanQuyens_DonViQuanLyId",
                table: "ChiTietTienBanQuyens",
                column: "DonViQuanLyId");
        }
    }
}
