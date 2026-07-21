using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_KeKhaiGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoanhNghieps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDoanhNghiep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiayPhepKd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoanhNghieps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoanhNghieps_DanhMucDonVis_DonViQuanLyId",
                        column: x => x.DonViQuanLyId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoanhNghiepLvKds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoanhNghiepQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoanhNghiepLvKds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoanhNghiepLvKds_DoanhNghieps_DoanhNghiepQuanLyId",
                        column: x => x.DoanhNghiepQuanLyId,
                        principalTable: "DoanhNghieps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeKhaiDangKyGiaCsKds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenCsKd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoanhNghiepQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiCSDLQG_DMHH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi_DMHH = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiCSDLQG_DMDT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi_DMDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiCSDLQG_DMKH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi_DMKH = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeKhaiDangKyGiaCsKds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeKhaiDangKyGiaCsKds_DoanhNghieps_DoanhNghiepQuanLyId",
                        column: x => x.DoanhNghiepQuanLyId,
                        principalTable: "DoanhNghieps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeKhaiDangKyGiaCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoSoKinhDoanhQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenDvCungUng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuyCachChatLuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianThucHien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MucGiaKeKhaiLk = table.Column<double>(type: "float", nullable: false),
                    MucGiaKeKhai = table.Column<double>(type: "float", nullable: false),
                    HinhThucKinhDoanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeKhaiDangKyGiaCts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeKhaiDangKyGiaCts_KeKhaiDangKyGiaCsKds_CoSoKinhDoanhQuanLyId",
                        column: x => x.CoSoKinhDoanhQuanLyId,
                        principalTable: "KeKhaiDangKyGiaCsKds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeKhaiDangKyGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoSoKinhDoanhQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViDongChuyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoQd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoQdLk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQdLk = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraHoSo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTinNguoiChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDtNguoiChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayChuyen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoHsDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YtCauThanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThyDgGadGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thoidiem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChucDanhKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTenNguoiKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeKhaiDangKyGias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeKhaiDangKyGias_KeKhaiDangKyGiaCsKds_CoSoKinhDoanhQuanLyId",
                        column: x => x.CoSoKinhDoanhQuanLyId,
                        principalTable: "KeKhaiDangKyGiaCsKds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoanhNghiepLvKds_DoanhNghiepQuanLyId",
                table: "DoanhNghiepLvKds",
                column: "DoanhNghiepQuanLyId");

            migrationBuilder.CreateIndex(
                name: "IX_DoanhNghieps_DonViQuanLyId",
                table: "DoanhNghieps",
                column: "DonViQuanLyId");

            migrationBuilder.CreateIndex(
                name: "IX_KeKhaiDangKyGiaCsKds_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGiaCsKds",
                column: "DoanhNghiepQuanLyId");

            migrationBuilder.CreateIndex(
                name: "IX_KeKhaiDangKyGiaCts_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGiaCts",
                column: "CoSoKinhDoanhQuanLyId");

            migrationBuilder.CreateIndex(
                name: "IX_KeKhaiDangKyGias_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGias",
                column: "CoSoKinhDoanhQuanLyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoanhNghiepLvKds");

            migrationBuilder.DropTable(
                name: "KeKhaiDangKyGiaCts");

            migrationBuilder.DropTable(
                name: "KeKhaiDangKyGias");

            migrationBuilder.DropTable(
                name: "KeKhaiDangKyGiaCsKds");

            migrationBuilder.DropTable(
                name: "DoanhNghieps");
        }
    }
}
