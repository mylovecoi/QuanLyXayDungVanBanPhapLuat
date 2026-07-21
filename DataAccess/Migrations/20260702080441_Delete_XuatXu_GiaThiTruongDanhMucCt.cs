using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Delete_XuatXu_GiaThiTruongDanhMucCt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XuatXu",
                table: "GiaThiTruongDanhMucCts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "XuatXu",
                table: "GiaThiTruongDanhMucCts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
