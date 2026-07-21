using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_DanhMucTienBanQuyen_ChiTietTienBanQuyen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiTietTienBanQuyens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoiTuongSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThueSuat = table.Column<double>(type: "float", nullable: false),
                    DonGia1 = table.Column<double>(type: "float", nullable: false),
                    DonGia2 = table.Column<double>(type: "float", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    MaDoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoiTuongSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Style = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietTienBanQuyens");

            migrationBuilder.DropTable(
                name: "DanhMucTienBanQuyens");
        }
    }
}
