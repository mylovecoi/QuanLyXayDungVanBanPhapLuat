using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    [Migration("20260802070000_AddTheoDoiTienDoXayDungMenu")]
    public partial class AddTheoDoiTienDoXayDungMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var systemUser = "11111111-1111-1111-1111-111111111111";

            migrationBuilder.Sql(
                $"""
                IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000050')
                BEGIN
                    INSERT INTO RoleActions
                    (
                        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                        STTSapXep, PhanLoai, [Level], Role, RoleGroupId,
                        Title, Controller, Action, Parameter, [Table],
                        Status, UseGroup, Icon
                    )
                    VALUES
                    (
                        '20000000-0000-0000-0000-000000000050', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(),
                        6, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.TheoDoiTienDoXayDung', '20000000-0000-0000-0000-000000000025',
                        N'Theo dõi tiến độ xây dựng', 'TheoDoiTienDoXayDung', 'Index', NULL, 'HoSoVanBans',
                        N'Kích hoạt', NULL, NULL
                    );
                END;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000050';
                """);
        }
    }
}
