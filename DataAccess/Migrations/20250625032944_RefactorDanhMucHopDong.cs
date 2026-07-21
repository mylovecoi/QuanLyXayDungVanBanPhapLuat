using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDanhMucHopDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucHopDongs_DanhMucHopDongs_NhomNghiepVuId",
                table: "DanhMucHopDongs");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucHopDongs_NhomNghiepVuId",
                table: "DanhMucHopDongs");

            migrationBuilder.DropColumn(
                name: "NhomNghiepVuId",
                table: "DanhMucHopDongs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NhomNghiepVuId",
                table: "DanhMucHopDongs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucHopDongs_NhomNghiepVuId",
                table: "DanhMucHopDongs",
                column: "NhomNghiepVuId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucHopDongs_DanhMucHopDongs_NhomNghiepVuId",
                table: "DanhMucHopDongs",
                column: "NhomNghiepVuId",
                principalTable: "DanhMucHopDongs",
                principalColumn: "Id");
        }
    }
}
