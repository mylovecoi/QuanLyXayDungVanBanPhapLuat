using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHoSoVanBanGiaHan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var systemUser = "11111111-1111-1111-1111-111111111111";

            migrationBuilder.CreateTable(
                name: "HoSoVanBanGiaHans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocQuyTrinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NguoiGiaHanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HanXuLyCu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanXuLyMoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNgayGiaHan = table.Column<int>(type: "int", nullable: false),
                    LyDoGiaHan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanGiaHans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanGiaHans_DanhMucBuocQuyTrinhs_BuocQuyTrinhId",
                        column: x => x.BuocQuyTrinhId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanGiaHans_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanGiaHans_Users_NguoiGiaHanId",
                        column: x => x.NguoiGiaHanId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanGiaHans_BuocQuyTrinhId",
                table: "HoSoVanBanGiaHans",
                column: "BuocQuyTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanGiaHans_HoSoVanBanId",
                table: "HoSoVanBanGiaHans",
                column: "HoSoVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanGiaHans_NguoiGiaHanId",
                table: "HoSoVanBanGiaHans",
                column: "NguoiGiaHanId");

            migrationBuilder.Sql(
                $"""
                UPDATE RoleActions
                SET Controller = 'GiaHanXayDung',
                    Action = 'Index',
                    [Table] = 'HoSoVanBans',
                    UpdatedBy = '{systemUser}',
                    UpdatedDate = GETDATE()
                WHERE Id = '20000000-0000-0000-0000-000000000042';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoVanBanGiaHans");
        }
    }
}
