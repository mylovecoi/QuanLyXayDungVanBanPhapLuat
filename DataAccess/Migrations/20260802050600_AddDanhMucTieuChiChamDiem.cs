using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDanhMucTieuChiChamDiem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var systemUser = "11111111-1111-1111-1111-111111111111";

            migrationBuilder.AddColumn<decimal>(
                name: "DiemChatLuongVanBan",
                table: "HoSoVanBans",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiemTienDoXayDung",
                table: "HoSoVanBans",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayChamDiem",
                table: "HoSoVanBans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongDiemDanhGia",
                table: "HoSoVanBans",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TongThoiGianQuyDinhNgay",
                table: "HoSoVanBans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TongThoiGianXayDungNgay",
                table: "HoSoVanBans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TyLeThoiGianXayDung",
                table: "HoSoVanBans",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XepLoaiDanhGia",
                table: "HoSoVanBans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DanhMucTieuChiDiems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaTieuChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenTieuChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LoaiTieuChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KieuGiaTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DonViGiaTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    DiemToiDa = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucTieuChiDiems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucTieuChiDiemMucs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTieuChiDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuGiaTri = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DenGiaTri = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BaoGomTuGiaTri = table.Column<bool>(type: "bit", nullable: false),
                    BaoGomDenGiaTri = table.Column<bool>(type: "bit", nullable: false),
                    Diem = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    NhanHienThi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucTieuChiDiemMucs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucTieuChiDiemMucs_DanhMucTieuChiDiems_DanhMucTieuChiDiemId",
                        column: x => x.DanhMucTieuChiDiemId,
                        principalTable: "DanhMucTieuChiDiems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucTieuChiDiemMucs_DanhMucTieuChiDiemId",
                table: "DanhMucTieuChiDiemMucs",
                column: "DanhMucTieuChiDiemId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucTieuChiDiems_MaTieuChi",
                table: "DanhMucTieuChiDiems",
                column: "MaTieuChi",
                unique: true);

            migrationBuilder.Sql(
                $"""
                IF NOT EXISTS (SELECT 1 FROM DanhMucTieuChiDiems WHERE MaTieuChi = N'THOI_GIAN_XAY_DUNG')
                BEGIN
                    INSERT INTO DanhMucTieuChiDiems
                    (
                        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                        MaTieuChi, TenTieuChi, LoaiTieuChi, KieuGiaTri, DonViGiaTri,
                        ThuTuSapXep, DiemToiDa, TrangThai, MoTa, GhiChu
                    )
                    VALUES
                    (
                        '30000000-0000-0000-0000-000000000001', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(),
                        N'THOI_GIAN_XAY_DUNG', N'Thời gian xây dựng văn bản', N'THOI_GIAN', N'TY_LE', N'PERCENT',
                        1, 40, 1, N'Căn cứ theo tỷ lệ % thời gian thực tế / tổng thời gian quy định.', NULL
                    );
                END;

                IF NOT EXISTS (SELECT 1 FROM DanhMucTieuChiDiems WHERE MaTieuChi = N'CHAT_LUONG_XAY_DUNG')
                BEGIN
                    INSERT INTO DanhMucTieuChiDiems
                    (
                        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                        MaTieuChi, TenTieuChi, LoaiTieuChi, KieuGiaTri, DonViGiaTri,
                        ThuTuSapXep, DiemToiDa, TrangThai, MoTa, GhiChu
                    )
                    VALUES
                    (
                        '30000000-0000-0000-0000-000000000002', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(),
                        N'CHAT_LUONG_XAY_DUNG', N'Chất lượng văn bản xây dựng', N'CHAT_LUONG', N'SO_LAN', N'COUNT',
                        2, 60, 1, N'Căn cứ theo số lần trả lại ở bước đánh giá chất lượng.', NULL
                    );
                END;

                IF NOT EXISTS (SELECT 1 FROM DanhMucTieuChiDiemMucs WHERE Id = '30000000-0000-0000-0000-000000000101')
                BEGIN
                    INSERT INTO DanhMucTieuChiDiemMucs
                    (Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, DanhMucTieuChiDiemId, TuGiaTri, DenGiaTri, BaoGomTuGiaTri, BaoGomDenGiaTri, Diem, NhanHienThi, ThuTuSapXep, TrangThai, GhiChu)
                    VALUES
                    ('30000000-0000-0000-0000-000000000101', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000001', NULL, 80, 1, 1, 40, N'<= 80% thời gian chuẩn', 1, 1, NULL),
                    ('30000000-0000-0000-0000-000000000102', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000001', 80, 100, 0, 1, 35, N'> 80% đến 100%', 2, 1, NULL),
                    ('30000000-0000-0000-0000-000000000103', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000001', 100, 110, 0, 1, 28, N'> 100% đến 110%', 3, 1, NULL),
                    ('30000000-0000-0000-0000-000000000104', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000001', 110, 125, 0, 1, 20, N'> 110% đến 125%', 4, 1, NULL),
                    ('30000000-0000-0000-0000-000000000105', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000001', 125, 150, 0, 1, 10, N'> 125% đến 150%', 5, 1, NULL),
                    ('30000000-0000-0000-0000-000000000106', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000001', 150, NULL, 0, 1, 0, N'> 150%', 6, 1, NULL),
                    ('30000000-0000-0000-0000-000000000201', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000002', 0, 0, 1, 1, 60, N'Không bị trả lại lần nào', 1, 1, NULL),
                    ('30000000-0000-0000-0000-000000000202', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000002', 1, 1, 1, 1, 45, N'Trả lại 1 lần', 2, 1, NULL),
                    ('30000000-0000-0000-0000-000000000203', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000002', 2, 2, 1, 1, 30, N'Trả lại 2 lần', 3, 1, NULL),
                    ('30000000-0000-0000-0000-000000000204', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000002', 3, 3, 1, 1, 15, N'Trả lại 3 lần', 4, 1, NULL),
                    ('30000000-0000-0000-0000-000000000205', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(), '30000000-0000-0000-0000-000000000002', 4, NULL, 1, 1, 0, N'Trả lại từ 4 lần trở lên', 5, 1, NULL);
                END;

                IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000048')
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
                        '20000000-0000-0000-0000-000000000048', '{systemUser}', GETDATE(), '{systemUser}', GETDATE(),
                        6, 'Detail', 2, 'QuanLyDanhMuc.DanhMucTieuChiDiem', '20000000-0000-0000-0000-000000000011',
                        N'Danh mục tiêu chí chấm điểm', 'DanhMucTieuChiDiem', 'Index', NULL, 'DanhMucTieuChiDiems',
                        N'Kích hoạt', 'QuanTriHeThong', NULL
                    );
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000048';
                DELETE FROM DanhMucTieuChiDiemMucs WHERE DanhMucTieuChiDiemId IN ('30000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000002');
                DELETE FROM DanhMucTieuChiDiems WHERE Id IN ('30000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000002');
                """);

            migrationBuilder.DropTable(
                name: "DanhMucTieuChiDiemMucs");

            migrationBuilder.DropTable(
                name: "DanhMucTieuChiDiems");

            migrationBuilder.DropColumn(
                name: "DiemChatLuongVanBan",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "DiemTienDoXayDung",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "NgayChamDiem",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "TongDiemDanhGia",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "TongThoiGianQuyDinhNgay",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "TongThoiGianXayDungNgay",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "TyLeThoiGianXayDung",
                table: "HoSoVanBans");

            migrationBuilder.DropColumn(
                name: "XepLoaiDanhGia",
                table: "HoSoVanBans");
        }
    }
}
