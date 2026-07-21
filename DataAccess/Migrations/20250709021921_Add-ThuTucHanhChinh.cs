using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddThuTucHanhChinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThuTucHanhChinhs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaThuTuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenThuTuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenQuyetDinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayQuyetDinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoQuanThucHien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CachThucThucHien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoiTuongThucHien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrinhTuThucHien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiHanGiaiQuyet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phi = table.Column<double>(type: "float", nullable: false),
                    LePhi = table.Column<double>(type: "float", nullable: false),
                    ThanhPhanHoSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YeuCauDieuKien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanCuPhapLy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KetQuaThucHien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuTucHanhChinhs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThuTucHanhChinhs");
        }
    }
}
