using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Remove_KeKhaiDangKyGiaCsKd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeKhaiDangKyGiaCts_KeKhaiDangKyGiaCsKds_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGiaCts");

            migrationBuilder.DropForeignKey(
                name: "FK_KeKhaiDangKyGias_KeKhaiDangKyGiaCsKds_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGias");

            migrationBuilder.DropTable(
                name: "KeKhaiDangKyGiaCsKds");

            migrationBuilder.RenameColumn(
                name: "CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGias",
                newName: "DoanhNghiepQuanLyId");

            migrationBuilder.RenameIndex(
                name: "IX_KeKhaiDangKyGias_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGias",
                newName: "IX_KeKhaiDangKyGias_DoanhNghiepQuanLyId");

            migrationBuilder.RenameColumn(
                name: "CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGiaCts",
                newName: "DoanhNghiepQuanLyId");

            migrationBuilder.RenameIndex(
                name: "IX_KeKhaiDangKyGiaCts_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGiaCts",
                newName: "IX_KeKhaiDangKyGiaCts_DoanhNghiepQuanLyId");

            migrationBuilder.AddColumn<Guid>(
                name: "DoanhNghiepId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_KeKhaiDangKyGiaCts_DoanhNghieps_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGiaCts",
                column: "DoanhNghiepQuanLyId",
                principalTable: "DoanhNghieps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeKhaiDangKyGias_DoanhNghieps_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGias",
                column: "DoanhNghiepQuanLyId",
                principalTable: "DoanhNghieps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeKhaiDangKyGiaCts_DoanhNghieps_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGiaCts");

            migrationBuilder.DropForeignKey(
                name: "FK_KeKhaiDangKyGias_DoanhNghieps_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGias");

            migrationBuilder.DropColumn(
                name: "DoanhNghiepId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGias",
                newName: "CoSoKinhDoanhQuanLyId");

            migrationBuilder.RenameIndex(
                name: "IX_KeKhaiDangKyGias_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGias",
                newName: "IX_KeKhaiDangKyGias_CoSoKinhDoanhQuanLyId");

            migrationBuilder.RenameColumn(
                name: "DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGiaCts",
                newName: "CoSoKinhDoanhQuanLyId");

            migrationBuilder.RenameIndex(
                name: "IX_KeKhaiDangKyGiaCts_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGiaCts",
                newName: "IX_KeKhaiDangKyGiaCts_CoSoKinhDoanhQuanLyId");

            migrationBuilder.CreateTable(
                name: "KeKhaiDangKyGiaCsKds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoanhNghiepQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNghe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKetNoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetNoi_DMDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetNoi_DMHH = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetNoi_DMKH = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenCsKd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG_DMDT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG_DMHH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCSDLQG_DMKH = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_KeKhaiDangKyGiaCsKds_DoanhNghiepQuanLyId",
                table: "KeKhaiDangKyGiaCsKds",
                column: "DoanhNghiepQuanLyId");

            migrationBuilder.AddForeignKey(
                name: "FK_KeKhaiDangKyGiaCts_KeKhaiDangKyGiaCsKds_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGiaCts",
                column: "CoSoKinhDoanhQuanLyId",
                principalTable: "KeKhaiDangKyGiaCsKds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeKhaiDangKyGias_KeKhaiDangKyGiaCsKds_CoSoKinhDoanhQuanLyId",
                table: "KeKhaiDangKyGias",
                column: "CoSoKinhDoanhQuanLyId",
                principalTable: "KeKhaiDangKyGiaCsKds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
