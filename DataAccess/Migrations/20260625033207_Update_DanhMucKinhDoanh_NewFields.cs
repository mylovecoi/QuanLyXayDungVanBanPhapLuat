using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhMucKinhDoanh_NewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaDvDongChuyen",
                table: "DanhMucKinhDoanhs",
                newName: "DonViQuanLyId");

            migrationBuilder.RenameColumn(
                name: "MaDv",
                table: "DanhMucKinhDoanhs",
                newName: "DonViDongChuyenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DonViQuanLyId",
                table: "DanhMucKinhDoanhs",
                newName: "MaDvDongChuyen");

            migrationBuilder.RenameColumn(
                name: "DonViDongChuyenId",
                table: "DanhMucKinhDoanhs",
                newName: "MaDv");
        }
    }
}
