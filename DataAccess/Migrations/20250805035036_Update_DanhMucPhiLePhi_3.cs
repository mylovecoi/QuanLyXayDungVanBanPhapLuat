using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhMucPhiLePhi_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPhiLePhis_DanhMucHopDongs_LoaiHopDongId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoaiHopDongId",
                table: "DanhMucPhiLePhis",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "PhanLoaiId",
                table: "DanhMucPhiLePhis",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPhiLePhis_PhanLoaiId",
                table: "DanhMucPhiLePhis",
                column: "PhanLoaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPhiLePhis_DanhMucHopDongs_LoaiHopDongId",
                table: "DanhMucPhiLePhis",
                column: "LoaiHopDongId",
                principalTable: "DanhMucHopDongs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPhiLePhis_OptionDatas_PhanLoaiId",
                table: "DanhMucPhiLePhis",
                column: "PhanLoaiId",
                principalTable: "OptionDatas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPhiLePhis_DanhMucHopDongs_LoaiHopDongId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPhiLePhis_OptionDatas_PhanLoaiId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucPhiLePhis_PhanLoaiId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.DropColumn(
                name: "PhanLoaiId",
                table: "DanhMucPhiLePhis");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoaiHopDongId",
                table: "DanhMucPhiLePhis",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPhiLePhis_DanhMucHopDongs_LoaiHopDongId",
                table: "DanhMucPhiLePhis",
                column: "LoaiHopDongId",
                principalTable: "DanhMucHopDongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
