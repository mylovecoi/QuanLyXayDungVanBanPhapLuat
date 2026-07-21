using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_DanhMucKinhDoanh_DanhMucGiaHHDV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucGiaHHDVs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucGiaHHDVs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucKinhDoanhs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucKinhDoanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucGiaHHDVChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaGiaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenGiaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDvDongChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHH_BTC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleGoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhMucGiaHHDVId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucGiaHHDVChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucGiaHHDVChiTiets_DanhMucGiaHHDVs_DanhMucGiaHHDVId",
                        column: x => x.DanhMucGiaHHDVId,
                        principalTable: "DanhMucGiaHHDVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucKinhDoanhChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDvDongChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheoDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaHH_BTC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleGoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhMucKinhDoanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucKinhDoanhChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucKinhDoanhChiTiets_DanhMucKinhDoanhs_DanhMucKinhDoanhId",
                        column: x => x.DanhMucKinhDoanhId,
                        principalTable: "DanhMucKinhDoanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucGiaHHDVChiTiets_DanhMucGiaHHDVId",
                table: "DanhMucGiaHHDVChiTiets",
                column: "DanhMucGiaHHDVId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucKinhDoanhChiTiets_DanhMucKinhDoanhId",
                table: "DanhMucKinhDoanhChiTiets",
                column: "DanhMucKinhDoanhId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhMucGiaHHDVChiTiets");

            migrationBuilder.DropTable(
                name: "DanhMucKinhDoanhChiTiets");

            migrationBuilder.DropTable(
                name: "DanhMucGiaHHDVs");

            migrationBuilder.DropTable(
                name: "DanhMucKinhDoanhs");
        }
    }
}
