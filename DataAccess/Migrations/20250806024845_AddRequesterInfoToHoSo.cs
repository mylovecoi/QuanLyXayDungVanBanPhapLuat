using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRequesterInfoToHoSo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PhuongThucCongChung",
                table: "HoSoCCCTs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThongTinDonVi",
                table: "HoSoCCCTs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhuongThucCongChung",
                table: "HoSoCCCTs");

            migrationBuilder.DropColumn(
                name: "ThongTinDonVi",
                table: "HoSoCCCTs");
        }
    }
}
