using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_GiaThiTruongTongHop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiaThiTruongTongHopCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHhDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenHhDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DacDiemKt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaBaoCao = table.Column<double>(type: "float", nullable: false),
                    GiaKyTruoc = table.Column<double>(type: "float", nullable: false),
                    GiaKyNay = table.Column<double>(type: "float", nullable: false),
                    MucTangGiam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TyLeTangGiam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NguonThongTin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    STTSapXep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaThiTruongTongHopCts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiaThiTruongTongHops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHoSoTongHop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThongTuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoBc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayChotBc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Thang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CongBo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LichSu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoaiHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChiTietExcel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipf_Word = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipf_Word_Base64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipf_Pdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipf_Pdf_Base64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipf_Excel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaThiTruongTongHops", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaThiTruongTongHopCts");

            migrationBuilder.DropTable(
                name: "GiaThiTruongTongHops");
        }
    }
}
