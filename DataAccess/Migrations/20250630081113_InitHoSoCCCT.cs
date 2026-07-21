using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitHoSoCCCT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoCCCTs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaSoHoSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiHopDongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoTenNguoiNop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoCCCDNguoiNop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDTNguoiNop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayThuLy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThongTinBenA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTinBenB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiTaiSanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThongTinChiTietTaiSan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenNganHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanBoTinDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChietKhau = table.Column<int>(type: "int", nullable: false),
                    SoTrang = table.Column<int>(type: "int", nullable: false),
                    SoVanBan = table.Column<int>(type: "int", nullable: false),
                    NoiLuuTru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriHopDong = table.Column<double>(type: "float", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    DonVi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViTiepNhan = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonVisDongChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayChuyen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThongTinChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDoTraLai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoQDDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThongTinDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoCCCTs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoCCCTs_DanhMucDiaDanhs_DiaBanId",
                        column: x => x.DiaBanId,
                        principalTable: "DanhMucDiaDanhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoCCCTs_DanhMucDonVis_DonViQuanLyId",
                        column: x => x.DonViQuanLyId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoCCCTs_DanhMucHopDongs_LoaiHopDongId",
                        column: x => x.LoaiHopDongId,
                        principalTable: "DanhMucHopDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoCCCTs_OptionDatas_LoaiTaiSanId",
                        column: x => x.LoaiTaiSanId,
                        principalTable: "OptionDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoSoCCCTChiPhis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    ChiPhi = table.Column<double>(type: "float", nullable: false),
                    TiLeChietKhau = table.Column<double>(type: "float", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoCCCTChiPhis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoCCCTChiPhis_HoSoCCCTs_HoSoId",
                        column: x => x.HoSoId,
                        principalTable: "HoSoCCCTs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoCCCTHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThongTinThayDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TruongBiThayDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriMoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoCCCTHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoCCCTHistories_HoSoCCCTs_HoSoId",
                        column: x => x.HoSoId,
                        principalTable: "HoSoCCCTs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTChiPhis_HoSoId",
                table: "HoSoCCCTChiPhis",
                column: "HoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTHistories_HoSoId",
                table: "HoSoCCCTHistories",
                column: "HoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTs_DiaBanId",
                table: "HoSoCCCTs",
                column: "DiaBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTs_DonViQuanLyId",
                table: "HoSoCCCTs",
                column: "DonViQuanLyId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTs_LoaiHopDongId",
                table: "HoSoCCCTs",
                column: "LoaiHopDongId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTs_LoaiTaiSanId",
                table: "HoSoCCCTs",
                column: "LoaiTaiSanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoCCCTChiPhis");

            migrationBuilder.DropTable(
                name: "HoSoCCCTHistories");

            migrationBuilder.DropTable(
                name: "HoSoCCCTs");
        }
    }
}
