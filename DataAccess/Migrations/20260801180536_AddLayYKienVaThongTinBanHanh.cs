using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLayYKienVaThongTinBanHanh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChucVuNguoiKy",
                table: "HoSoVanBans",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CoQuanBanHanhId",
                table: "HoSoVanBans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DuongDanCongKhai",
                table: "HoSoVanBans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoTenNguoiKy",
                table: "HoSoVanBans",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiVanBanBanHanh",
                table: "HoSoVanBans",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayBanHanh",
                table: "HoSoVanBans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCoHieuLuc",
                table: "HoSoVanBans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCongKhai",
                table: "HoSoVanBans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayHetHieuLuc",
                table: "HoSoVanBans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKy",
                table: "HoSoVanBans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NguoiKyId",
                table: "HoSoVanBans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoKyHieuBanHanh",
                table: "HoSoVanBans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThaiBanHanh",
                table: "HoSoVanBans",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "CHUA_BAN_HANH");

            migrationBuilder.AddColumn<string>(
                name: "TrichYeuBanHanh",
                table: "HoSoVanBans",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VanBanPhapLuatId",
                table: "HoSoVanBans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiTaiLieu",
                table: "AttachedFiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhamViCongKhai",
                table: "AttachedFiles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "NOI_BO");

            migrationBuilder.CreateTable(
                name: "HoSoVanBanDotLayYKiens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocQuyTrinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanLayYKien = table.Column<int>(type: "int", nullable: false),
                    CoQuanLayYKien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CheDoNhapYKien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HinhThucLayYKien = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SoVanBanLayYKien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayGuiLayYKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HanPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCoKetQua = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDungYeuCau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TongSoThanhVien = table.Column<int>(type: "int", nullable: true),
                    SoDongY = table.Column<int>(type: "int", nullable: true),
                    SoDongYCoYKien = table.Column<int>(type: "int", nullable: true),
                    SoKhongDongY = table.Column<int>(type: "int", nullable: true),
                    SoKhongPhanHoi = table.Column<int>(type: "int", nullable: true),
                    TyLeDongY = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    KetQuaChung = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NoiDungTongHop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungTiepThu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiTongHopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayTongHop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanDotLayYKiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDotLayYKiens_DanhMucBuocQuyTrinhs_BuocQuyTrinhId",
                        column: x => x.BuocQuyTrinhId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDotLayYKiens_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDotLayYKiens_Users_NguoiTongHopId",
                        column: x => x.NguoiTongHopId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoSoVanBanYKienThanhViens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DotLayYKienId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThanhVienId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HoTenThanhVien = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DonViId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenDonVi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ThuTuHienThi = table.Column<int>(type: "int", nullable: false),
                    CoQuyenBieuQuyet = table.Column<bool>(type: "bit", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HanPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KetQuaYKien = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NoiDungYKien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungTiepThu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiPhanHoi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanYKienThanhViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanYKienThanhViens_DanhMucDonVis_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanYKienThanhViens_HoSoVanBanDotLayYKiens_DotLayYKienId",
                        column: x => x.DotLayYKienId,
                        principalTable: "HoSoVanBanDotLayYKiens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanYKienThanhViens_Users_ThanhVienId",
                        column: x => x.ThanhVienId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_CoQuanBanHanhId",
                table: "HoSoVanBans",
                column: "CoQuanBanHanhId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBans_NguoiKyId",
                table: "HoSoVanBans",
                column: "NguoiKyId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDotLayYKiens_BuocQuyTrinhId",
                table: "HoSoVanBanDotLayYKiens",
                column: "BuocQuyTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDotLayYKiens_HoSoVanBanId_BuocQuyTrinhId_CoQuanLayYKien_LanLayYKien",
                table: "HoSoVanBanDotLayYKiens",
                columns: new[] { "HoSoVanBanId", "BuocQuyTrinhId", "CoQuanLayYKien", "LanLayYKien" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDotLayYKiens_NguoiTongHopId",
                table: "HoSoVanBanDotLayYKiens",
                column: "NguoiTongHopId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanYKienThanhViens_DonViId",
                table: "HoSoVanBanYKienThanhViens",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanYKienThanhViens_DotLayYKienId_ThanhVienId",
                table: "HoSoVanBanYKienThanhViens",
                columns: new[] { "DotLayYKienId", "ThanhVienId" },
                unique: true,
                filter: "[ThanhVienId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanYKienThanhViens_ThanhVienId",
                table: "HoSoVanBanYKienThanhViens",
                column: "ThanhVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoVanBans_DanhMucDonVis_CoQuanBanHanhId",
                table: "HoSoVanBans",
                column: "CoQuanBanHanhId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoVanBans_Users_NguoiKyId",
                table: "HoSoVanBans",
                column: "NguoiKyId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoVanBans_DanhMucDonVis_CoQuanBanHanhId",
                table: "HoSoVanBans");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoVanBans_Users_NguoiKyId",
                table: "HoSoVanBans");

            migrationBuilder.DropTable(
                name: "HoSoVanBanYKienThanhViens");

            migrationBuilder.DropTable(
                name: "HoSoVanBanDotLayYKiens");

            migrationBuilder.DropIndex(
                name: "IX_HoSoVanBans_CoQuanBanHanhId",
                table: "HoSoVanBans");

            migrationBuilder.DropIndex(
                name: "IX_HoSoVanBans_NguoiKyId",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "ChucVuNguoiKy",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "CoQuanBanHanhId",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "DuongDanCongKhai",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "HoTenNguoiKy",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "LoaiVanBanBanHanh",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "NgayBanHanh",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "NgayCoHieuLuc",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "NgayCongKhai",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "NgayHetHieuLuc",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "NgayKy",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "NguoiKyId",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "SoKyHieuBanHanh",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "TrangThaiBanHanh",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "TrichYeuBanHanh",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "VanBanPhapLuatId",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "LoaiTaiLieu",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "PhamViCongKhai",
                table: "AttachedFiles");
        }
    }
}
