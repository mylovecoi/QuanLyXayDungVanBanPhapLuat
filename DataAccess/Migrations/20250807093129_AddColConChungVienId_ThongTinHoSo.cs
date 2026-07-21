using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddColConChungVienId_ThongTinHoSo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CongChungVienId",
                table: "HoSoCCCTs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoCCCTs_CongChungVienId",
                table: "HoSoCCCTs",
                column: "CongChungVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoCCCTs_DanhMucCanBos_CongChungVienId",
                table: "HoSoCCCTs",
                column: "CongChungVienId",
                principalTable: "DanhMucCanBos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoCCCTs_DanhMucCanBos_CongChungVienId",
                table: "HoSoCCCTs");

            migrationBuilder.DropIndex(
                name: "IX_HoSoCCCTs_CongChungVienId",
                table: "HoSoCCCTs");

            migrationBuilder.DropColumn(
                name: "CongChungVienId",
                table: "HoSoCCCTs");
        }
    }
}
