using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_GiaThiTruongTh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ipf_Excel",
                table: "GiaThiTruongTongHops");

            migrationBuilder.DropColumn(
                name: "Ipf_Pdf",
                table: "GiaThiTruongTongHops");

            migrationBuilder.DropColumn(
                name: "Ipf_Pdf_Base64",
                table: "GiaThiTruongTongHops");

            migrationBuilder.DropColumn(
                name: "Ipf_Word",
                table: "GiaThiTruongTongHops");

            migrationBuilder.DropColumn(
                name: "Ipf_Word_Base64",
                table: "GiaThiTruongTongHops");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ipf_Excel",
                table: "GiaThiTruongTongHops",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ipf_Pdf",
                table: "GiaThiTruongTongHops",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ipf_Pdf_Base64",
                table: "GiaThiTruongTongHops",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ipf_Word",
                table: "GiaThiTruongTongHops",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ipf_Word_Base64",
                table: "GiaThiTruongTongHops",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
