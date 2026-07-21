using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DinhGia1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DinhGias_DanhMucDonVis_DanhMucDonViId",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "DenNam",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "TuNam",
                table: "DinhGias");

            migrationBuilder.RenameColumn(
                name: "DanhMucDonViId",
                table: "DinhGias",
                newName: "LoaiHopDongId");

            migrationBuilder.RenameIndex(
                name: "IX_DinhGias_DanhMucDonViId",
                table: "DinhGias",
                newName: "IX_DinhGias_LoaiHopDongId");

            migrationBuilder.AddColumn<Guid>(
                name: "DonViQuanLyId",
                table: "DinhGias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DinhGias_DonViQuanLyId",
                table: "DinhGias",
                column: "DonViQuanLyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DinhGias_DanhMucDonVis_DonViQuanLyId",
                table: "DinhGias",
                column: "DonViQuanLyId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DinhGias_DanhMucHopDongs_LoaiHopDongId",
                table: "DinhGias",
                column: "LoaiHopDongId",
                principalTable: "DanhMucHopDongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DinhGias_DanhMucDonVis_DonViQuanLyId",
                table: "DinhGias");

            migrationBuilder.DropForeignKey(
                name: "FK_DinhGias_DanhMucHopDongs_LoaiHopDongId",
                table: "DinhGias");

            migrationBuilder.DropIndex(
                name: "IX_DinhGias_DonViQuanLyId",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "DonViQuanLyId",
                table: "DinhGias");

            migrationBuilder.RenameColumn(
                name: "LoaiHopDongId",
                table: "DinhGias",
                newName: "DanhMucDonViId");

            migrationBuilder.RenameIndex(
                name: "IX_DinhGias_LoaiHopDongId",
                table: "DinhGias",
                newName: "IX_DinhGias_DanhMucDonViId");

            migrationBuilder.AddColumn<string>(
                name: "DenNam",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TuNam",
                table: "DinhGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DinhGias_DanhMucDonVis_DanhMucDonViId",
                table: "DinhGias",
                column: "DanhMucDonViId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
