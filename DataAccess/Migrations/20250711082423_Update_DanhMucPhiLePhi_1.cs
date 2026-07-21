using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhMucPhiLePhi_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCC",
                table: "DanhMucPhiLePhis");

            migrationBuilder.AddColumn<Guid>(
                name: "LoaiHopDongId",
                table: "DanhMucPhiLePhis",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPhiLePhis_LoaiHopDongId",
                table: "DanhMucPhiLePhis",
                column: "LoaiHopDongId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPhiLePhis_DanhMucHopDongs_LoaiHopDongId",
                table: "DanhMucPhiLePhis",
                column: "LoaiHopDongId",
                principalTable: "DanhMucHopDongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPhiLePhis_DanhMucHopDongs_LoaiHopDongId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucPhiLePhis_LoaiHopDongId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.DropColumn(
                name: "LoaiHopDongId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.AddColumn<bool>(
                name: "IsCC",
                table: "DanhMucPhiLePhis",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
