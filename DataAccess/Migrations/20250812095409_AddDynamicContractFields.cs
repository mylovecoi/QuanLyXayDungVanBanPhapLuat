using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicContractFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucHopDongChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucHopDongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ColSize = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucHopDongChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucHopDongChiTiets_DanhMucHopDongs_DanhMucHopDongId",
                        column: x => x.DanhMucHopDongId,
                        principalTable: "DanhMucHopDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoCCCTChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucHopDongChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoCCCTChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoCCCTChiTiets_DanhMucHopDongChiTiets_DanhMucHopDongChiTietId",
                        column: x => x.DanhMucHopDongChiTietId,
                        principalTable: "DanhMucHopDongChiTiets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoCCCTChiTiets_HoSoCCCTs_HoSoId",
                        column: x => x.HoSoId,
                        principalTable: "HoSoCCCTs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucHopDongChiTiets_DanhMucHopDongId",
                table: "DanhMucHopDongChiTiets",
                column: "DanhMucHopDongId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTChiTiets_DanhMucHopDongChiTietId",
                table: "HoSoCCCTChiTiets",
                column: "DanhMucHopDongChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTChiTiets_HoSoId_DanhMucHopDongChiTietId",
                table: "HoSoCCCTChiTiets",
                columns: new[] { "HoSoId", "DanhMucHopDongChiTietId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoCCCTChiTiets");

            migrationBuilder.DropTable(
                name: "DanhMucHopDongChiTiets");
        }
    }
}
