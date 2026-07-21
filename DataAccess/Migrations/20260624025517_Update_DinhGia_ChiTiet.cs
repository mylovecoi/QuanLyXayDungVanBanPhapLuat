using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DinhGia_ChiTiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeExcel",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "Ipf1",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "LyDo_ad",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "LyDo_h",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "LyDo_t",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaCqcq",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaCqcq_ad",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaCqcq_h",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaCqcq_t",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaDv_ad",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaDv_h",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "MaDv_t",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "ThoiDiem_ad",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "ThongTin_ad",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "ThongTin_h",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "ThongTin_t",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "TrangThai_ad",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "TrangThai_h",
                table: "DinhGias");

            migrationBuilder.RenameColumn(
                name: "TrangThai_t",
                table: "DinhGias",
                newName: "ChiTietExcel");

            migrationBuilder.RenameColumn(
                name: "ThoiDiem_t",
                table: "DinhGias",
                newName: "NgayDuyet");

            migrationBuilder.RenameColumn(
                name: "ThoiDiem_h",
                table: "DinhGias",
                newName: "NgayCongBo");

            migrationBuilder.CreateTable(
                name: "ChiTietNuocSachs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoiTuongSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TyTrongTieuThu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SanLuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThueSuat = table.Column<double>(type: "float", nullable: false),
                    DonGia1 = table.Column<double>(type: "float", nullable: false),
                    DonGia2 = table.Column<double>(type: "float", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietNuocSachs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietNuocSachs_DanhMucDonVis_DonViQuanLyId",
                        column: x => x.DonViQuanLyId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietNuocSachs_DonViQuanLyId",
                table: "ChiTietNuocSachs",
                column: "DonViQuanLyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietNuocSachs");

            migrationBuilder.RenameColumn(
                name: "NgayDuyet",
                table: "DinhGias",
                newName: "ThoiDiem_t");

            migrationBuilder.RenameColumn(
                name: "NgayCongBo",
                table: "DinhGias",
                newName: "ThoiDiem_h");

            migrationBuilder.RenameColumn(
                name: "ChiTietExcel",
                table: "DinhGias",
                newName: "TrangThai_t");

            migrationBuilder.AddColumn<string>(
                name: "CodeExcel",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ipf1",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDo_ad",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDo_h",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDo_t",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaCqcq",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaCqcq_ad",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaCqcq_h",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaCqcq_t",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDv_ad",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDv_h",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDv_t",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiDiem_ad",
                table: "DinhGias",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ThongTin_ad",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThongTin_h",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThongTin_t",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai_ad",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai_h",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
