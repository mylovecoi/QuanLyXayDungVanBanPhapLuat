using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttachedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoVanBan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBanHanh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucDiaDanhs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDiaDanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    DiaDanhCapTrenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucDiaDanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucDonVis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDonVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    DonViChuQuanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaQHNS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucDanhQuanLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoVaTenNguoiQuanLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoaiDonVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhNangThanhToan = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucDonVis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupsPermision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsPermision", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Request = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViGui = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViTiepNhan = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViDongChuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControllerNameDanhSach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionNameDanhSach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterDanhSach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControllerNameXetDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionNameXetDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterXetDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViView = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupPermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Index = table.Column<bool>(type: "bit", nullable: false),
                    Create = table.Column<bool>(type: "bit", nullable: false),
                    Edit = table.Column<bool>(type: "bit", nullable: false),
                    Delete = table.Column<bool>(type: "bit", nullable: false),
                    Approve = table.Column<bool>(type: "bit", nullable: false),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Table = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Copyright = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MfgDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginLock = table.Column<int>(type: "int", nullable: false),
                    Train = table.Column<bool>(type: "bit", nullable: false),
                    IsChatBot = table.Column<bool>(type: "bit", nullable: false),
                    IsOPT = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemInfo", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SSA = table.Column<bool>(type: "bit", nullable: false),
                    DanhMucDonViId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoanhNghiepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OTPSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstLogin = table.Column<bool>(type: "bit", nullable: false),
                    LoginCount = table.Column<int>(type: "int", nullable: false),
                    TenDonViBaoCao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenDonViChuQuanBaoCao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaDanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucDanhKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTenNguoiKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KyHieuDonVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Menu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Theme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupPermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VNId = table.Column<bool>(type: "bit", nullable: false),
                    AgentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScanDeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScanDeviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucPhongBans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenPhongBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaPhongBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiPhongBan = table.Column<int>(type: "int", nullable: false),
                    DanhMucDonViId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucPhongBans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucPhongBans_DanhMucDonVis_DanhMucDonViId",
                        column: x => x.DanhMucDonViId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucCanBos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenCanBo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhongBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    TrinhDoChuyenMon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiLaoDong = table.Column<int>(type: "int", nullable: false),
                    SoTienBHXH = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoTienBHYT = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoQuyetDinhDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQuyetDinhDung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoQuyetDinhBoNhiem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQuyetDinhBoNhiem = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoQuyetDinhCapThe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQuyetDinhCapThe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoTheCongChungVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MucPhiBaoHiemTrachNhiem = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    ViTriViecLam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTuyenDung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoHopDongLaoDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKyHopDongLaoDong = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucCanBos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucCanBos_DanhMucDonVis_DonViQuanLyId",
                        column: x => x.DonViQuanLyId,
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhMucCanBos_DanhMucPhongBans_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "DanhMucPhongBans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucCanBos_DonViQuanLyId",
                table: "DanhMucCanBos",
                column: "DonViQuanLyId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucCanBos_PhongBanId",
                table: "DanhMucCanBos",
                column: "PhongBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPhongBans_DanhMucDonViId",
                table: "DanhMucPhongBans",
                column: "DanhMucDonViId");

            migrationBuilder.Sql(
                """
                DECLARE @Now DATETIME = GETDATE();

                INSERT INTO OptionDatas
                (
                    Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                    Code, DisplayName, Value, MoTa
                )
                VALUES
                (
                    '20000000-0000-0000-0000-000000000009',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    N'NhomQuyen',
                    N'Quản trị hệ thống',
                    N'QuanTriHeThong',
                    N'Nhóm quyền dùng cho các chức năng quản lý hệ thống'
                );

                INSERT INTO RoleActions
                (
                    Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                    STTSapXep, PhanLoai, Level, Role, RoleGroupId,
                    Title, Controller, Action, Parameter, [Table],
                    Status, UseGroup, Icon
                )
                VALUES
                (
                    '20000000-0000-0000-0000-000000000001',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    1, 'Group', 0, 'Systems', '00000000-0000-0000-0000-000000000000',
                    N'Chức năng hệ thống', '', '', NULL, '',
                    N'Kích hoạt', 'QuanTriHeThong', 'fas fa-cogs'
                ),
                (
                    '20000000-0000-0000-0000-000000000010',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    1, 'Group', 1, 'Systems.QuanTriHeThong', '20000000-0000-0000-0000-000000000001',
                    N'Quản trị hệ thống', '', '', NULL, '',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000011',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    2, 'Group', 1, 'Settings.QuanTriDanhMuc', '20000000-0000-0000-0000-000000000001',
                    N'Quản trị danh mục', '', '', NULL, '',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000004',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    1, 'Detail', 2, 'Systems.GroupPermission', '20000000-0000-0000-0000-000000000010',
                    N'Nhóm quyền truy cập', 'GroupPermission', 'Index', NULL, 'GroupsPermision',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000003',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    2, 'Detail', 2, 'Systems.User', '20000000-0000-0000-0000-000000000010',
                    N'Tài khoản truy cập', 'User', 'Index', NULL, 'Users',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000007',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    3, 'Detail', 2, 'Systems.Log', '20000000-0000-0000-0000-000000000010',
                    N'Nhật ký hệ thống', 'Log', 'Index', NULL, 'Logs',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000005',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    4, 'Detail', 2, 'Systems.RoleAction', '20000000-0000-0000-0000-000000000010',
                    N'Danh sách chức năng', 'RoleAction', 'Index', NULL, 'RoleActions',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000002',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    5, 'Detail', 2, 'Systems.SystemInfo', '20000000-0000-0000-0000-000000000010',
                    N'Cấu hình hệ thống', 'SystemInfo', 'Index', NULL, 'SystemInfo',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000012',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    1, 'Detail', 2, 'Settings.DanhMucDiaDanh', '20000000-0000-0000-0000-000000000011',
                    N'Danh sách địa danh', 'DanhMucDiaDanh', 'Index', NULL, 'DanhMucDiaDanhs',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000013',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    2, 'Detail', 2, 'Settings.DanhMucDonVi', '20000000-0000-0000-0000-000000000011',
                    N'Danh sách đơn vị', 'DanhMucDonVi', 'Index', NULL, 'DanhMucDonVis',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000014',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    3, 'Detail', 2, 'Manages.VanBanPhapLuat', '20000000-0000-0000-0000-000000000011',
                    N'Danh sách văn bản', 'VanBanPhapLuat', 'Index', NULL, 'AttachedFiles',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000015',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    4, 'Detail', 2, 'Settings.TrangThai', '20000000-0000-0000-0000-000000000011',
                    N'Danh sách trạng thái', 'DangPhatTrien', 'TrangThai', NULL, 'DangPhatTrien',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                ),
                (
                    '20000000-0000-0000-0000-000000000016',
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    '11111111-1111-1111-1111-111111111111',
                    @Now,
                    5, 'Detail', 2, 'Settings.QuyTrinhSoanThao', '20000000-0000-0000-0000-000000000011',
                    N'Danh sách quy trình soạn thảo', 'DangPhatTrien', 'QuyTrinhSoanThao', NULL, 'DangPhatTrien',
                    N'Kích hoạt', 'QuanTriHeThong', NULL
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachedFiles");

            migrationBuilder.DropTable(
                name: "DanhMucCanBos");

            migrationBuilder.DropTable(
                name: "DanhMucDiaDanhs");

            migrationBuilder.DropTable(
                name: "GroupsPermision");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OptionDatas");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "QuestionAnswers");

            migrationBuilder.DropTable(
                name: "RoleActions");

            migrationBuilder.DropTable(
                name: "SystemInfo");

            migrationBuilder.DropTable(
                name: "ThuTucHanhChinhs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DanhMucPhongBans");

            migrationBuilder.DropTable(
                name: "DanhMucDonVis");
        }
    }
}
