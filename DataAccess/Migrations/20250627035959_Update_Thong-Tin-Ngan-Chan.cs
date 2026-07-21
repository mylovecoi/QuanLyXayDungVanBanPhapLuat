using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_ThongTinNganChan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongTinNganChans_DanhMucDonVis_DonViId",
                table: "ThongTinNganChans");

            migrationBuilder.DropColumn(
                name: "TenCoQuan",
                table: "ThongTinNganChans");

            migrationBuilder.DropColumn(
                name: "TenDonViNhap",
                table: "ThongTinNganChans");

            migrationBuilder.RenameColumn(
                name: "NgayNhanCongVan",
                table: "ThongTinNganChans",
                newName: "NgayQuyetDinh");

            migrationBuilder.RenameColumn(
                name: "NgayDung",
                table: "ThongTinNganChans",
                newName: "NgayApDung");

            migrationBuilder.RenameColumn(
                name: "DonViId",
                table: "ThongTinNganChans",
                newName: "DonViBanHanhId");

            migrationBuilder.RenameIndex(
                name: "IX_ThongTinNganChans_DonViId",
                table: "ThongTinNganChans",
                newName: "IX_ThongTinNganChans_DonViBanHanhId");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayApDungDung",
                table: "ThongTinNganChans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayQuyetDinhDung",
                table: "ThongTinNganChans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoQuyetDinhDung",
                table: "ThongTinNganChans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ThongTinNganChans_DanhMucDonVis_DonViBanHanhId",
                table: "ThongTinNganChans",
                column: "DonViBanHanhId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongTinNganChans_DanhMucDonVis_DonViBanHanhId",
                table: "ThongTinNganChans");

            migrationBuilder.DropColumn(
                name: "NgayApDungDung",
                table: "ThongTinNganChans");

            migrationBuilder.DropColumn(
                name: "NgayQuyetDinhDung",
                table: "ThongTinNganChans");

            migrationBuilder.DropColumn(
                name: "SoQuyetDinhDung",
                table: "ThongTinNganChans");

            migrationBuilder.RenameColumn(
                name: "NgayQuyetDinh",
                table: "ThongTinNganChans",
                newName: "NgayNhanCongVan");

            migrationBuilder.RenameColumn(
                name: "NgayApDung",
                table: "ThongTinNganChans",
                newName: "NgayDung");

            migrationBuilder.RenameColumn(
                name: "DonViBanHanhId",
                table: "ThongTinNganChans",
                newName: "DonViId");

            migrationBuilder.RenameIndex(
                name: "IX_ThongTinNganChans_DonViBanHanhId",
                table: "ThongTinNganChans",
                newName: "IX_ThongTinNganChans_DonViId");

            migrationBuilder.AddColumn<string>(
                name: "TenCoQuan",
                table: "ThongTinNganChans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenDonViNhap",
                table: "ThongTinNganChans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongTinNganChans_DanhMucDonVis_DonViId",
                table: "ThongTinNganChans",
                column: "DonViId",
                principalTable: "DanhMucDonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
