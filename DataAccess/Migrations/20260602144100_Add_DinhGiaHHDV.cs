using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_DinhGiaHHDV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DinhGiaCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenCt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaLk = table.Column<double>(type: "float", nullable: false),
                    GiaKk = table.Column<double>(type: "float", nullable: false),
                    Gia1 = table.Column<double>(type: "float", nullable: false),
                    Gia2 = table.Column<double>(type: "float", nullable: false),
                    Gia3 = table.Column<double>(type: "float", nullable: false),
                    Gia4 = table.Column<double>(type: "float", nullable: false),
                    Trangthai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Style = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhGiaCts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DinhGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDinhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDiaBan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoQd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CongBo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiDiem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaCqcq = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiDiem_h = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaCqcq_h = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv_h = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo_h = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTin_h = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai_h = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiDiem_t = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaCqcq_t = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv_t = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo_t = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTin_t = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai_t = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiDiem_ad = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaCqcq_ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv_ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo_ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTin_ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai_ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TuNam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DenNam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipf1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoaiHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeExcel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhGias", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DinhGiaCts");

            migrationBuilder.DropTable(
                name: "DinhGias");
        }
    }
}
