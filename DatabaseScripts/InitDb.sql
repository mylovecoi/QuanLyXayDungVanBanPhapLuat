IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [AttachedFiles] (
    [Id] uniqueidentifier NOT NULL,
    [GroupId] uniqueidentifier NOT NULL,
    [TableName] nvarchar(max) NULL,
    [SoVanBan] nvarchar(max) NULL,
    [NgayBanHanh] datetime2 NOT NULL,
    [NgayApDung] datetime2 NOT NULL,
    [Url] nvarchar(250) NULL,
    [FileContent] varbinary(max) NULL,
    [FileName] nvarchar(100) NULL,
    [ContentType] nvarchar(50) NULL,
    [MoTa] nvarchar(500) NULL,
    [Status] nvarchar(max) NULL,
    [Public] bit NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_AttachedFiles] PRIMARY KEY ([Id])
);

CREATE TABLE [DanhMucDiaDanhs] (
    [Id] uniqueidentifier NOT NULL,
    [TenDiaDanh] nvarchar(max) NOT NULL,
    [Level] int NOT NULL,
    [STTSapXep] int NOT NULL,
    [DiaDanhCapTrenId] uniqueidentifier NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucDiaDanhs] PRIMARY KEY ([Id])
);

CREATE TABLE [DanhMucDonVis] (
    [Id] uniqueidentifier NOT NULL,
    [TenDonVi] nvarchar(max) NOT NULL,
    [Level] int NOT NULL,
    [STTSapXep] int NOT NULL,
    [DonViChuQuanId] uniqueidentifier NOT NULL,
    [DiaChi] nvarchar(max) NULL,
    [MaQHNS] nvarchar(max) NULL,
    [SoDienThoai] nvarchar(max) NULL,
    [ChucDanhQuanLy] nvarchar(max) NULL,
    [HoVaTenNguoiQuanLy] nvarchar(max) NULL,
    [PhanLoaiDonVi] nvarchar(max) NULL,
    [TinhNangThanhToan] bit NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucDonVis] PRIMARY KEY ([Id])
);

CREATE TABLE [GroupsPermision] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_GroupsPermision] PRIMARY KEY ([Id])
);

CREATE TABLE [Logs] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(max) NULL,
    [IpAddress] nvarchar(max) NULL,
    [Url] nvarchar(max) NULL,
    [Method] nvarchar(max) NULL,
    [Request] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY ([Id])
);

CREATE TABLE [Notifications] (
    [Id] uniqueidentifier NOT NULL,
    [DonViGui] uniqueidentifier NOT NULL,
    [DonViTiepNhan] uniqueidentifier NOT NULL,
    [DonViDongChuyen] nvarchar(max) NULL,
    [NoiDung] nvarchar(max) NULL,
    [ControllerNameDanhSach] nvarchar(max) NULL,
    [ActionNameDanhSach] nvarchar(max) NULL,
    [ParameterDanhSach] nvarchar(max) NULL,
    [ControllerNameXetDuyet] nvarchar(max) NULL,
    [ActionNameXetDuyet] nvarchar(max) NULL,
    [ParameterXetDuyet] nvarchar(max) NULL,
    [DonViView] nvarchar(max) NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
);

CREATE TABLE [OptionDatas] (
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(max) NULL,
    [DisplayName] nvarchar(max) NULL,
    [Value] nvarchar(max) NULL,
    [MoTa] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_OptionDatas] PRIMARY KEY ([Id])
);

CREATE TABLE [Permission] (
    [Id] uniqueidentifier NOT NULL,
    [GroupPermissionId] uniqueidentifier NOT NULL,
    [RoleActionId] uniqueidentifier NOT NULL,
    [Status] nvarchar(max) NULL,
    [Index] bit NOT NULL,
    [Create] bit NOT NULL,
    [Edit] bit NOT NULL,
    [Delete] bit NOT NULL,
    [Approve] bit NOT NULL,
    [Public] bit NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY ([Id])
);

CREATE TABLE [QuestionAnswers] (
    [Id] uniqueidentifier NOT NULL,
    [Question] nvarchar(max) NULL,
    [Answer] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_QuestionAnswers] PRIMARY KEY ([Id])
);

CREATE TABLE [DanhMucTrangThais] (
    [Id] uniqueidentifier NOT NULL,
    [MaTrangThai] nvarchar(max) NOT NULL,
    [TenTrangThai] nvarchar(max) NOT NULL,
    [MaMauHex] nvarchar(max) NOT NULL,
    [ThuTuSapXep] int NOT NULL,
    [TrangThai] bit NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucTrangThais] PRIMARY KEY ([Id])
);

CREATE TABLE [RoleActions] (
    [Id] uniqueidentifier NOT NULL,
    [STTSapXep] int NOT NULL,
    [PhanLoai] nvarchar(max) NOT NULL,
    [Level] int NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [RoleGroupId] uniqueidentifier NOT NULL,
    [Title] nvarchar(max) NULL,
    [Controller] nvarchar(max) NULL,
    [Action] nvarchar(max) NULL,
    [Parameter] nvarchar(max) NULL,
    [Table] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [UseGroup] nvarchar(max) NULL,
    [Icon] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_RoleActions] PRIMARY KEY ([Id])
);

CREATE TABLE [SystemInfo] (
    [Id] uniqueidentifier NOT NULL,
    [AppName] nvarchar(max) NULL,
    [Copyright] nvarchar(max) NULL,
    [MfgDate] datetime2 NOT NULL,
    [ExpDate] datetime2 NOT NULL,
    [LoginLock] int NOT NULL,
    [Train] bit NOT NULL,
    [IsChatBot] bit NOT NULL,
    [IsOPT] bit NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_SystemInfo] PRIMARY KEY ([Id])
);

CREATE TABLE [ThuTucHanhChinhs] (
    [Id] uniqueidentifier NOT NULL,
    [MaThuTuc] nvarchar(max) NOT NULL,
    [TenThuTuc] nvarchar(max) NOT NULL,
    [TenQuyetDinh] nvarchar(max) NOT NULL,
    [NgayQuyetDinh] datetime2 NOT NULL,
    [CoQuanThucHien] nvarchar(max) NOT NULL,
    [CachThucThucHien] nvarchar(max) NOT NULL,
    [DoiTuongThucHien] nvarchar(max) NOT NULL,
    [TrinhTuThucHien] nvarchar(max) NOT NULL,
    [ThoiHanGiaiQuyet] nvarchar(max) NOT NULL,
    [Phi] float NOT NULL,
    [LePhi] float NOT NULL,
    [ThanhPhanHoSo] nvarchar(max) NOT NULL,
    [YeuCauDieuKien] nvarchar(max) NOT NULL,
    [CanCuPhapLy] nvarchar(max) NOT NULL,
    [KetQuaThucHien] nvarchar(max) NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_ThuTucHanhChinhs] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [SSA] bit NOT NULL,
    [DanhMucDonViId] uniqueidentifier NOT NULL,
    [DoanhNghiepId] uniqueidentifier NOT NULL,
    [OTPSecretKey] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FirstLogin] bit NOT NULL,
    [LoginCount] int NOT NULL,
    [TenDonViBaoCao] nvarchar(max) NULL,
    [TenDonViChuQuanBaoCao] nvarchar(max) NULL,
    [DiaDanh] nvarchar(max) NULL,
    [ChucDanhKy] nvarchar(max) NULL,
    [HoTenNguoiKy] nvarchar(max) NULL,
    [KyHieuDonVi] nvarchar(max) NULL,
    [Content] nvarchar(max) NULL,
    [Menu] nvarchar(max) NULL,
    [Theme] nvarchar(max) NULL,
    [GroupPermissionId] uniqueidentifier NOT NULL,
    [VNId] bit NOT NULL,
    [AgentId] nvarchar(max) NULL,
    [ScanDeviceId] nvarchar(max) NULL,
    [ScanDeviceName] nvarchar(max) NULL,
    [Level] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [DanhMucPhongBans] (
    [Id] uniqueidentifier NOT NULL,
    [TenPhongBan] nvarchar(max) NOT NULL,
    [MaPhongBan] nvarchar(max) NOT NULL,
    [LoaiPhongBan] int NOT NULL,
    [DanhMucDonViId] uniqueidentifier NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucPhongBans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DanhMucPhongBans_DanhMucDonVis_DanhMucDonViId] FOREIGN KEY ([DanhMucDonViId]) REFERENCES [DanhMucDonVis] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DanhMucCanBos] (
    [Id] uniqueidentifier NOT NULL,
    [DonViQuanLyId] uniqueidentifier NOT NULL,
    [TenCanBo] nvarchar(max) NOT NULL,
    [NgaySinh] datetime2 NULL,
    [UserId] uniqueidentifier NOT NULL,
    [PhongBanId] uniqueidentifier NOT NULL,
    [GioiTinh] bit NOT NULL,
    [TrinhDoChuyenMon] nvarchar(max) NOT NULL,
    [LoaiLaoDong] int NOT NULL,
    [SoTienBHXH] decimal(18,0) NOT NULL,
    [SoTienBHYT] decimal(18,0) NOT NULL,
    [SoQuyetDinhDung] nvarchar(max) NULL,
    [NgayQuyetDinhDung] datetime2 NULL,
    [GhiChu] nvarchar(max) NULL,
    [SoQuyetDinhBoNhiem] nvarchar(max) NULL,
    [NgayQuyetDinhBoNhiem] datetime2 NULL,
    [SoQuyetDinhCapThe] nvarchar(max) NULL,
    [NgayQuyetDinhCapThe] datetime2 NULL,
    [SoTheCongChungVien] nvarchar(max) NULL,
    [ChucVu] nvarchar(max) NULL,
    [MucPhiBaoHiemTrachNhiem] decimal(18,0) NULL,
    [ViTriViecLam] nvarchar(max) NULL,
    [NgayTuyenDung] datetime2 NULL,
    [SoHopDongLaoDong] nvarchar(max) NULL,
    [NgayKyHopDongLaoDong] datetime2 NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucCanBos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DanhMucCanBos_DanhMucDonVis_DonViQuanLyId] FOREIGN KEY ([DonViQuanLyId]) REFERENCES [DanhMucDonVis] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DanhMucCanBos_DanhMucPhongBans_PhongBanId] FOREIGN KEY ([PhongBanId]) REFERENCES [DanhMucPhongBans] ([Id])
);

CREATE INDEX [IX_DanhMucCanBos_DonViQuanLyId] ON [DanhMucCanBos] ([DonViQuanLyId]);

CREATE INDEX [IX_DanhMucCanBos_PhongBanId] ON [DanhMucCanBos] ([PhongBanId]);

CREATE INDEX [IX_DanhMucPhongBans_DanhMucDonViId] ON [DanhMucPhongBans] ([DanhMucDonViId]);

DECLARE @Now DATETIME = GETDATE();

INSERT INTO DanhMucTrangThais
(
    Id, MaTrangThai, TenTrangThai, MaMauHex, ThuTuSapXep,
    TrangThai, MoTa, GhiChu,
    CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
)
VALUES
(
    '30000000-0000-0000-0000-000000000001',
    N'DANG_XU_LY',
    N'Đang xử lý',
    N'#28A745',
    1,
    1,
    N'Trạng thái đang được xử lý và hiển thị màu xanh trên màn hình theo dõi.',
    NULL,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now
),
(
    '30000000-0000-0000-0000-000000000002',
    N'QUA_HAN',
    N'Quá hạn',
    N'#DC3545',
    2,
    1,
    N'Trạng thái đã quá hạn xử lý và hiển thị màu đỏ trên màn hình theo dõi.',
    NULL,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now
),
(
    '30000000-0000-0000-0000-000000000003',
    N'SAP_DEN_HAN',
    N'Sắp đến hạn',
    N'#FFC107',
    3,
    1,
    N'Trạng thái sắp đến hạn xử lý và hiển thị màu vàng trên màn hình theo dõi.',
    NULL,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now
);

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
    3, 'Detail', 2, 'QuanLyDanhMuc.DanhMucVanBan', '20000000-0000-0000-0000-000000000011',
    N'Danh sách văn bản', 'DanhMucVanBan', 'Index', NULL, 'DanhMucVanBans',
    N'Kích hoạt', 'QuanTriHeThong', NULL
),
(
    '20000000-0000-0000-0000-000000000015',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    4, 'Detail', 2, 'QuanLyDanhMuc.DanhMucTrangThai', '20000000-0000-0000-0000-000000000011',
    N'Danh sách trạng thái', 'DanhMucTrangThai', 'Index', NULL, 'DanhMucTrangThais',
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

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260723082931_InitDb', N'9.0.2');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724093000_AddDanhMucTrangThai', N'9.0.7');

COMMIT;
GO

