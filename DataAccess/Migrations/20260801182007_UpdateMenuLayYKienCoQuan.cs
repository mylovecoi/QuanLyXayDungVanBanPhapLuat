using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMenuLayYKienCoQuan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE RoleActions
                SET Title = N'Lấy ý kiến UBND',
                    Controller = N'LayYKienUBND',
                    Action = N'Index',
                    [Table] = N'HoSoVanBans',
                    UpdatedDate = GETDATE()
                WHERE Role = N'VanBanQPPL.XayDungVanBan.GopYDanhGia';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE RoleActions
                SET Title = N'Góp ý đánh giá',
                    Controller = N'DangPhatTrien',
                    Action = N'GopYDanhGia',
                    [Table] = N'DangPhatTrien',
                    UpdatedDate = GETDATE()
                WHERE Role = N'VanBanQPPL.XayDungVanBan.GopYDanhGia';
                """);
        }
    }
}
