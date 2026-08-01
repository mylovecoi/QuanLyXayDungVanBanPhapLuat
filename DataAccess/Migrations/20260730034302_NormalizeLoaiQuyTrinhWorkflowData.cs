using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeLoaiQuyTrinhWorkflowData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LoaiQuyTrinh",
                table: "DanhMucQuyTrinhSoanThaos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "XayDung",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.Sql(@"
                UPDATE DanhMucQuyTrinhSoanThaos
                SET LoaiQuyTrinh = N'XayDung'
                WHERE LoaiQuyTrinh IS NULL OR LTRIM(RTRIM(LoaiQuyTrinh)) = N'';
            ");

            migrationBuilder.Sql(@"
                UPDATE qt
                SET qt.LoaiQuyTrinh = N'DangKy'
                FROM DanhMucQuyTrinhSoanThaos qt
                WHERE EXISTS (
                    SELECT 1
                    FROM DanhMucBuocQuyTrinhs b
                    WHERE b.QuyTrinhSoanThaoId = qt.Id
                      AND b.MaBuoc = N'BUOC_01_DANG_KY'
                );
            ");

            migrationBuilder.Sql(@"
                UPDATE qt
                SET qt.LoaiQuyTrinh = N'XayDung'
                FROM DanhMucQuyTrinhSoanThaos qt
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM DanhMucBuocQuyTrinhs b
                    WHERE b.QuyTrinhSoanThaoId = qt.Id
                      AND b.MaBuoc = N'BUOC_01_DANG_KY'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE DanhMucQuyTrinhSoanThaos
                SET LoaiQuyTrinh = N'XayDung';
            ");

            migrationBuilder.AlterColumn<string>(
                name: "LoaiQuyTrinh",
                table: "DanhMucQuyTrinhSoanThaos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "XayDung");
        }
    }
}
