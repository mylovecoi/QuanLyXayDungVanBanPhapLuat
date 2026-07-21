using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHierarchicalHopDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "DanhMucHopDongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "DanhMucHopDongs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucHopDongs_ParentId",
                table: "DanhMucHopDongs",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucHopDongs_DanhMucHopDongs_ParentId",
                table: "DanhMucHopDongs",
                column: "ParentId",
                principalTable: "DanhMucHopDongs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucHopDongs_DanhMucHopDongs_ParentId",
                table: "DanhMucHopDongs");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucHopDongs_ParentId",
                table: "DanhMucHopDongs");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "DanhMucHopDongs");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "DanhMucHopDongs");
        }
    }
}
