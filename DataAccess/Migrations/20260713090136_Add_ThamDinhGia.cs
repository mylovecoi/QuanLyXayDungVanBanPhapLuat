using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_ThamDinhGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThamDinhGiaCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HangHoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHangHoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenHangHoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuyCachChatLuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongSoKt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XuatXu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoLuong = table.Column<double>(type: "float", nullable: false),
                    DonGiaThamDinh = table.Column<double>(type: "float", nullable: false),
                    GiaTriTsThamDinh = table.Column<double>(type: "float", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamDinhGiaCts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThamDinhGiaDanhMucDonVis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaGCN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoThe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoQd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamDinhGiaDanhMucDonVis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThamDinhGiaDanhMucHangHoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDanhMucHangHoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamDinhGiaDanhMucHangHoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThamDinhGiaHoiDongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToTung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanCuPhapLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDeNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CapHoiDong = table.Column<int>(type: "int", nullable: false),
                    LoaiHoiDong = table.Column<int>(type: "int", nullable: false),
                    SoQd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoQuanBanHanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenHoiDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChuTichHoiDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NhiemVuHoiDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungQd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaTinhApDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHuyenApDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipf1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamDinhGiaHoiDongs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThamDinhGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViChuQuanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaDiem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DvYeuCau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DvThamDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTbKl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoSoTdGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoQdPheDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQdPheDuyet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNgayKq = table.Column<int>(type: "int", nullable: false),
                    TtTsTd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CongBo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thoidiem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ipf1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamDinhGias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThamDinhGiaDanhMucHangHoaCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HangHoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHangHoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenHangHoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuyCachChatLuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongSoKt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XuatXu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamDinhGiaDanhMucHangHoaCts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThamDinhGiaDanhMucHangHoaCts_ThamDinhGiaDanhMucHangHoas_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "ThamDinhGiaDanhMucHangHoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThamDinhGiaHoiDongCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoiDongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    STTSapXep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamDinhGiaHoiDongCts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThamDinhGiaHoiDongCts_ThamDinhGiaHoiDongs_HoiDongId",
                        column: x => x.HoiDongId,
                        principalTable: "ThamDinhGiaHoiDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThamDinhGiaDanhMucHangHoaCts_HangHoaId",
                table: "ThamDinhGiaDanhMucHangHoaCts",
                column: "HangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThamDinhGiaHoiDongCts_HoiDongId",
                table: "ThamDinhGiaHoiDongCts",
                column: "HoiDongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThamDinhGiaCts");

            migrationBuilder.DropTable(
                name: "ThamDinhGiaDanhMucDonVis");

            migrationBuilder.DropTable(
                name: "ThamDinhGiaDanhMucHangHoaCts");

            migrationBuilder.DropTable(
                name: "ThamDinhGiaHoiDongCts");

            migrationBuilder.DropTable(
                name: "ThamDinhGias");

            migrationBuilder.DropTable(
                name: "ThamDinhGiaDanhMucHangHoas");

            migrationBuilder.DropTable(
                name: "ThamDinhGiaHoiDongs");
        }
    }
}
