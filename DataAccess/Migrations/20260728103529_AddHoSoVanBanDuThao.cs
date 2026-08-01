using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHoSoVanBanDuThao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoVanBanDuThaos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDuThao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SoLanDuThao = table.Column<int>(type: "int", nullable: false),
                    NgayCapNhatDuThao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiDuThao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NoiDungTomTat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KetQuaThucHien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayBaoCaoKetQua = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDungBaoCao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaDuDieuKienChuyenBuoc = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanDuThaos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDuThaos_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDuThaos_HoSoVanBanId",
                table: "HoSoVanBanDuThaos",
                column: "HoSoVanBanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoVanBanDuThaos");
        }
    }
}
