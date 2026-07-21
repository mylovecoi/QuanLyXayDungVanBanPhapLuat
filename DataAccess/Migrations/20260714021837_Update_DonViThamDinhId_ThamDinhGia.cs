using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_DonViThamDinhId_ThamDinhGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DvThamDinh",
                table: "ThamDinhGias");

            migrationBuilder.AddColumn<Guid>(
                name: "DonViThamDinhId",
                table: "ThamDinhGias",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonViThamDinhId",
                table: "ThamDinhGias");

            migrationBuilder.AddColumn<string>(
                name: "DvThamDinh",
                table: "ThamDinhGias",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
