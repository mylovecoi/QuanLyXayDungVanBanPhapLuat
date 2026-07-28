using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class UpdateQuyTrinhSoanThaoMultiSelect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DanhMucVanBanIds",
                table: "DanhMucQuyTrinhSoanThaos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "NgayHetHieuLuc",
                table: "DanhMucQuyTrinhSoanThaos");

            migrationBuilder.DropColumn(
                name: "NgayHieuLuc",
                table: "DanhMucQuyTrinhSoanThaos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayHetHieuLuc",
                table: "DanhMucQuyTrinhSoanThaos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayHieuLuc",
                table: "DanhMucQuyTrinhSoanThaos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "DanhMucVanBanIds",
                table: "DanhMucQuyTrinhSoanThaos");
        }
    }
}
