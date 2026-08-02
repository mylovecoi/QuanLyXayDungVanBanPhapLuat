using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class AddTraCuuDangKyVanBanMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var systemUser = "11111111-1111-1111-1111-111111111111";

            migrationBuilder.Sql(
                $"""
                IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000051')
                BEGIN
                    INSERT INTO RoleActions
                    (
                        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
                        Title, Controller, Action, Parameter, [Table],
                        Status, UseGroup, Icon
                    )
                    VALUES
                    (
                        '20000000-0000-0000-0000-000000000051', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(),
                        4, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.TraCuuDangKyVanBan', '20000000-0000-0000-0000-000000000021',
                        N'Tra cứu đăng ký văn bản', 'TraCuuDangKyVanBan', 'Index', NULL, 'HoSoVanBans',
                        N'Kích hoạt', NULL, NULL
                    )
                END
                ELSE
                BEGIN
                    UPDATE RoleActions
                    SET Title = N'Tra cứu đăng ký văn bản',
                        Controller = 'TraCuuDangKyVanBan',
                        Action = 'Index',
                        [Table] = 'HoSoVanBans',
                        UpdatedBy = '{systemUser}',
                        UpdatedDate = GETDATE()
                    WHERE Id = '20000000-0000-0000-0000-000000000051'
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM RoleActions
                WHERE Id = '20000000-0000-0000-0000-000000000051'
                """);
        }
    }
}
