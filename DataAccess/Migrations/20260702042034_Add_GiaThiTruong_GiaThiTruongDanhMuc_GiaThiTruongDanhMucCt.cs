using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_GiaThiTruong_GiaThiTruongDanhMuc_GiaThiTruongDanhMucCt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiaThiTruongDanhMucCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThongTuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHhDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenHhDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DacDiemKt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XuatXu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaThiTruongDanhMucCts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiaThiTruongDanhMucs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenTT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiDiemBanHanhTT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaThiTruongDanhMucs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiaThiTruongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViChuQuanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThongTuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoQd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thoidiem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoQdLk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiDiemLk = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Thang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CongBo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LichSu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoaiHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChiTietExcel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaThiTruongs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaThiTruongDanhMucCts");

            migrationBuilder.DropTable(
                name: "GiaThiTruongDanhMucs");

            migrationBuilder.DropTable(
                name: "GiaThiTruongs");
        }
    }
}
