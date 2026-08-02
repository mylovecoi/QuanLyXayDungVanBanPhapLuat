using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddChamDiemXayDung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var systemUser = "11111111-1111-1111-1111-111111111111";

            migrationBuilder.CreateTable(
                name: "HoSoVanBanChamDiems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiChamDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayChamDiem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongDiem = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    XepLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanChamDiems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanChamDiems_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanChamDiems_Users_NguoiChamDiemId",
                        column: x => x.NguoiChamDiemId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoSoVanBanChamDiemChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanChamDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTieuChiDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaTieuChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenTieuChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LoaiTieuChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaTriTinhDiem = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DiemDeXuat = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    DiemChinhThuc = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DiemToiDa = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DienGiaiGiaTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanChamDiemChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanChamDiemChiTiets_DanhMucTieuChiDiems_DanhMucTieuChiDiemId",
                        column: x => x.DanhMucTieuChiDiemId,
                        principalTable: "DanhMucTieuChiDiems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanChamDiemChiTiets_HoSoVanBanChamDiems_HoSoVanBanChamDiemId",
                        column: x => x.HoSoVanBanChamDiemId,
                        principalTable: "HoSoVanBanChamDiems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanChamDiemChiTiets_DanhMucTieuChiDiemId",
                table: "HoSoVanBanChamDiemChiTiets",
                column: "DanhMucTieuChiDiemId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanChamDiemChiTiets_HoSoVanBanChamDiemId",
                table: "HoSoVanBanChamDiemChiTiets",
                column: "HoSoVanBanChamDiemId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanChamDiems_HoSoVanBanId",
                table: "HoSoVanBanChamDiems",
                column: "HoSoVanBanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanChamDiems_NguoiChamDiemId",
                table: "HoSoVanBanChamDiems",
                column: "NguoiChamDiemId");

            migrationBuilder.Sql(
                $"""
                IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000049')
                BEGIN
                    INSERT INTO RoleActions
                    (
                        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                        STTSapXep, PhanLoai, [Level], Role, RoleGroupId,
                        Title, Controller, Action, Parameter, [Table],
                        Status, UseGroup, Icon
                    )
                    VALUES
                    (
                        '20000000-0000-0000-0000-000000000049', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(),
                        5, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.ChamDiemXayDung', '20000000-0000-0000-0000-000000000025',
                        N'Chấm điểm xây dựng', 'ChamDiemXayDung', 'Index', NULL, 'HoSoVanBanChamDiems',
                        N'Kích hoạt', NULL, NULL
                    );
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000049';
                """);

            migrationBuilder.DropTable(
                name: "HoSoVanBanChamDiemChiTiets");

            migrationBuilder.DropTable(
                name: "HoSoVanBanChamDiems");
        }
    }
}
