using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DinhGiaHHDV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DanhMucDonViId",
                table: "DinhGias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DanhMucDonViId",
                table: "DinhGiaCts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DinhGias_DanhMucDonViId",
                table: "DinhGias",
                column: "DanhMucDonViId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_DinhGias_DanhMucDonVis_DanhMucDonViId",
                table: "DinhGias",
                column: "DanhMucDonViId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DinhGiaCts_DanhMucDonVis_DanhMucDonViId",
                table: "DinhGiaCts");

            migrationBuilder.DropForeignKey(
                name: "FK_DinhGias_DanhMucDonVis_DanhMucDonViId",
                table: "DinhGias");

            migrationBuilder.DropIndex(
                name: "IX_DinhGias_DanhMucDonViId",
                table: "DinhGias");

            migrationBuilder.DropIndex(
                name: "IX_DinhGiaCts_DanhMucDonViId",
                table: "DinhGiaCts");

            migrationBuilder.DropColumn(
                name: "DanhMucDonViId",
                table: "DinhGias");

            migrationBuilder.DropColumn(
                name: "DanhMucDonViId",
                table: "DinhGiaCts");
        }
    }
}
