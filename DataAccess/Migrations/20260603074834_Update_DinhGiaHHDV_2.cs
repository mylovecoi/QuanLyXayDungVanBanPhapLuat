using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DinhGiaHHDV_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DinhGiaCts_DanhMucDonVis_DanhMucDonViId",
                table: "DinhGiaCts");

            migrationBuilder.DropIndex(
                name: "IX_DinhGiaCts_DanhMucDonViId",
                table: "DinhGiaCts");

            migrationBuilder.DropColumn(
                name: "DanhMucDonViId",
                table: "DinhGiaCts");

            migrationBuilder.DropColumn(
                name: "MaDv",
                table: "DinhGiaCts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DanhMucDonViId",
                table: "DinhGiaCts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "MaDv",
                table: "DinhGiaCts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DinhGiaCts_DanhMucDonViId",
                table: "DinhGiaCts",
                column: "DanhMucDonViId");

            migrationBuilder.AddForeignKey(
                name: "FK_DinhGiaCts_DanhMucDonVis_DanhMucDonViId",
                table: "DinhGiaCts",
                column: "DanhMucDonViId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
