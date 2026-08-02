using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateThiHanhPhapLuatMenuRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE RoleActions
SET Controller = 'DanhSachKeHoachThiHanhPhapLuat',
    Action = 'Index',
    [Table] = 'ThiHanhPhapLuatKeHoach'
WHERE Id = '20000000-0000-0000-0000-000000000032';

UPDATE RoleActions
SET Controller = 'QuaTrinhToChucThucHien',
    Action = 'Index',
    [Table] = 'ThiHanhPhapLuatKeHoach'
WHERE Id = '20000000-0000-0000-0000-000000000033';

UPDATE RoleActions
SET Controller = 'DanhGiaKetQuaThiHanhPhapLuat',
    Action = 'Index',
    [Table] = 'ThiHanhPhapLuatKeHoach'
WHERE Id = '20000000-0000-0000-0000-000000000034';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE RoleActions
SET Controller = 'DangPhatTrien',
    Action = 'DanhSachKeHoach',
    [Table] = 'DangPhatTrien'
WHERE Id = '20000000-0000-0000-0000-000000000032';

UPDATE RoleActions
SET Controller = 'DangPhatTrien',
    Action = 'QuaTrinhToChucThucHien',
    [Table] = 'DangPhatTrien'
WHERE Id = '20000000-0000-0000-0000-000000000033';

UPDATE RoleActions
SET Controller = 'DangPhatTrien',
    Action = 'DanhGiaKetQua',
    [Table] = 'DangPhatTrien'
WHERE Id = '20000000-0000-0000-0000-000000000034';
");
        }
    }
}
