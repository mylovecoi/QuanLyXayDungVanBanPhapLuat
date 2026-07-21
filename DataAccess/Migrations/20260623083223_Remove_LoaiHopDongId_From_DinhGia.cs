using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Remove_LoaiHopDongId_From_DinhGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DinhGias_DanhMucHopDongs_LoaiHopDongId",
                table: "DinhGias");

            migrationBuilder.DropIndex(
                name: "IX_DinhGias_LoaiHopDongId",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "LoaiHopDongId",
                table: "DinhGias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LoaiHopDongId",
                table: "DinhGias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DinhGias_LoaiHopDongId",
                table: "DinhGias",
                column: "LoaiHopDongId");

            migrationBuilder.AddForeignKey(
                name: "FK_DinhGias_DanhMucHopDongs_LoaiHopDongId",
                table: "DinhGias",
                column: "LoaiHopDongId",
                principalTable: "DanhMucHopDongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
