using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhMucCanBo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PhongBanId",
                table: "DanhMucCanBos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucCanBos_PhongBanId",
                table: "DanhMucCanBos",
                column: "PhongBanId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucCanBos_DanhMucPhongBans_PhongBanId",
                table: "DanhMucCanBos",
                column: "PhongBanId",
                principalTable: "DanhMucPhongBans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucCanBos_DanhMucPhongBans_PhongBanId",
                table: "DanhMucCanBos");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucCanBos_PhongBanId",
                table: "DanhMucCanBos");

            migrationBuilder.DropColumn(
                name: "PhongBanId",
                table: "DanhMucCanBos");
        }
    }
}
