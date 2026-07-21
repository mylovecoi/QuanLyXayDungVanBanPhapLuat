using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Remove_DinhGiaCts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DinhGiaCts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DinhGiaCts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gia1 = table.Column<double>(type: "float", nullable: false),
                    Gia2 = table.Column<double>(type: "float", nullable: false),
                    Gia3 = table.Column<double>(type: "float", nullable: false),
                    Gia4 = table.Column<double>(type: "float", nullable: false),
                    GiaKeKhai = table.Column<double>(type: "float", nullable: false),
                    GiaLienKe = table.Column<double>(type: "float", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTHienThi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    Style = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenCt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Trangthai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhGiaCts", x => x.Id);
                });
        }
    }
}
