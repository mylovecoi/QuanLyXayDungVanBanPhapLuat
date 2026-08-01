using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDraftVersionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoVanBanDuThaoVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanVersion = table.Column<int>(type: "int", nullable: false),
                    SoLanTraLai = table.Column<int>(type: "int", nullable: false),
                    TenVersion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AttachedFileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayTaoVersion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanDuThaoVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanDuThaoVersions_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanDuThaoVersions_HoSoVanBanId",
                table: "HoSoVanBanDuThaoVersions",
                column: "HoSoVanBanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoVanBanDuThaoVersions");
        }
    }
}
