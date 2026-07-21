using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDanhMucNuocSachParentChild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoiTuongSuDung",
                table: "DanhMucNuocSachs");

            migrationBuilder.DropColumn(
                name: "STTSapXep",
                table: "DanhMucNuocSachs");

            migrationBuilder.DropColumn(
                name: "STTSapXep",
                table: "DanhMucGiaChungs");

            migrationBuilder.RenameColumn(
                name: "Style",
                table: "DanhMucNuocSachs",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "STTHienThi",
                table: "DanhMucNuocSachs",
                newName: "TenDanhMuc");

            migrationBuilder.RenameColumn(
                name: "MaDoiTuong",
                table: "DanhMucNuocSachs",
                newName: "MaDanhMuc");

            migrationBuilder.RenameColumn(
                name: "TenChiTiet",
                table: "DanhMucGiaChungs",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "MaChiTiet",
                table: "DanhMucGiaChungs",
                newName: "TenDanhMuc");

            migrationBuilder.AddColumn<string>(
                name: "MaDanhMuc",
                table: "DanhMucGiaChungs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DanhMucGiaChungCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucGiaChungId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucGiaChungCts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucGiaChungCts_DanhMucGiaChungs_DanhMucGiaChungId",
                        column: x => x.DanhMucGiaChungId,
                        principalTable: "DanhMucGiaChungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucNuocSachCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucNuocSachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_DanhMucNuocSachCts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucNuocSachCts_DanhMucNuocSachs_DanhMucNuocSachId",
                        column: x => x.DanhMucNuocSachId,
                        principalTable: "DanhMucNuocSachs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucGiaChungCts_DanhMucGiaChungId",
                table: "DanhMucGiaChungCts",
                column: "DanhMucGiaChungId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucNuocSachCts_DanhMucNuocSachId",
                table: "DanhMucNuocSachCts",
                column: "DanhMucNuocSachId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhMucGiaChungCts");

            migrationBuilder.DropTable(
                name: "DanhMucNuocSachCts");

            migrationBuilder.DropColumn(
                name: "MaDanhMuc",
                table: "DanhMucGiaChungs");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "DanhMucNuocSachs",
                newName: "Style");

            migrationBuilder.RenameColumn(
                name: "TenDanhMuc",
                table: "DanhMucNuocSachs",
                newName: "STTHienThi");

            migrationBuilder.RenameColumn(
                name: "MaDanhMuc",
                table: "DanhMucNuocSachs",
                newName: "MaDoiTuong");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "DanhMucGiaChungs",
                newName: "TenChiTiet");

            migrationBuilder.RenameColumn(
                name: "TenDanhMuc",
                table: "DanhMucGiaChungs",
                newName: "MaChiTiet");

            migrationBuilder.AddColumn<string>(
                name: "DoiTuongSuDung",
                table: "DanhMucNuocSachs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "STTSapXep",
                table: "DanhMucNuocSachs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "STTSapXep",
                table: "DanhMucGiaChungs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
