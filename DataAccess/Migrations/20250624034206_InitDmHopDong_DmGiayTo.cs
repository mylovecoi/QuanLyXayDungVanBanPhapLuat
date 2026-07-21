using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitDmHopDong_DmGiayTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucHopDongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhomNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCC = table.Column<bool>(type: "bit", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucHopDongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucHopDongs_DanhMucHopDongs_NhomNghiepVuId",
                        column: x => x.NhomNghiepVuId,
                        principalTable: "DanhMucHopDongs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DanhMucGiayTos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DmHopDongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenGiayTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaGiayTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiGiayTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    IsUploadFile = table.Column<bool>(type: "bit", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucGiayTos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucGiayTos_DanhMucHopDongs_DmHopDongId",
                        column: x => x.DmHopDongId,
                        principalTable: "DanhMucHopDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucGiayTos_DmHopDongId",
                table: "DanhMucGiayTos",
                column: "DmHopDongId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucHopDongs_NhomNghiepVuId",
                table: "DanhMucHopDongs",
                column: "NhomNghiepVuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhMucGiayTos");

            migrationBuilder.DropTable(
                name: "DanhMucHopDongs");
        }
    }
}
