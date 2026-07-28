using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddWorkflowStepDeadlineTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoNgayCanhBaoSapHan",
                table: "DanhMucBuocQuyTrinhs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoNgayXuLyTieuChuan",
                table: "DanhMucBuocQuyTrinhs",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoNgayCanhBaoSapHan",
                table: "DanhMucBuocQuyTrinhs");

            migrationBuilder.DropColumn(
                name: "SoNgayXuLyTieuChuan",
                table: "DanhMucBuocQuyTrinhs");
        }
    }
}
