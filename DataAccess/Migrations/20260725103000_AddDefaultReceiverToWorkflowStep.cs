using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddDefaultReceiverToWorkflowStep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DonViTiepNhanMacDinhId",
                table: "DanhMucBuocQuyTrinhs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucBuocQuyTrinhs_DonViTiepNhanMacDinhId",
                table: "DanhMucBuocQuyTrinhs",
                column: "DonViTiepNhanMacDinhId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucBuocQuyTrinhs_DanhMucDonVis_DonViTiepNhanMacDinhId",
                table: "DanhMucBuocQuyTrinhs",
                column: "DonViTiepNhanMacDinhId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id");

            migrationBuilder.Sql(@"
UPDATE DanhMucBuocQuyTrinhs
SET DonViTiepNhanMacDinhId = '40000000-0000-0000-0000-000000000013'
WHERE MaBuoc = N'BUOC_02_THONG_NHAT' AND DonViTiepNhanMacDinhId IS NULL;

UPDATE DanhMucBuocQuyTrinhs
SET DonViTiepNhanMacDinhId = '40000000-0000-0000-0000-000000000002'
WHERE MaBuoc = N'BUOC_06_TRINH_THAM_QUYEN' AND DonViTiepNhanMacDinhId IS NULL;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucBuocQuyTrinhs_DanhMucDonVis_DonViTiepNhanMacDinhId",
                table: "DanhMucBuocQuyTrinhs");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucBuocQuyTrinhs_DonViTiepNhanMacDinhId",
                table: "DanhMucBuocQuyTrinhs");

            migrationBuilder.DropColumn(
                name: "DonViTiepNhanMacDinhId",
                table: "DanhMucBuocQuyTrinhs");
        }
    }
}
