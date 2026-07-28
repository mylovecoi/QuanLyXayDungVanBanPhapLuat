using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddHoSoStepDeadlines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoVanBanBuocThoiHans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocQuyTrinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    SoNgayXuLy = table.Column<int>(type: "int", nullable: true),
                    SoNgayCanhBaoSapHan = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoVanBanBuocThoiHans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoVanBanBuocThoiHans_DanhMucBuocQuyTrinhs_BuocQuyTrinhId",
                        column: x => x.BuocQuyTrinhId,
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoVanBanBuocThoiHans_HoSoVanBans_HoSoVanBanId",
                        column: x => x.HoSoVanBanId,
                        principalTable: "HoSoVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanBuocThoiHans_BuocQuyTrinhId",
                table: "HoSoVanBanBuocThoiHans",
                column: "BuocQuyTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoVanBanBuocThoiHans_HoSoVanBanId",
                table: "HoSoVanBanBuocThoiHans",
                column: "HoSoVanBanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoVanBanBuocThoiHans");
        }
    }
}
