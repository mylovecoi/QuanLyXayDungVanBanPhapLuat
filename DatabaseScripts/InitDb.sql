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

CREATE TABLE [DanhMucVanBans] (
    [Id] uniqueidentifier NOT NULL,
    [TenLoaiVanBan] nvarchar(max) NOT NULL,
    [ChuTheBanHanh] nvarchar(max) NOT NULL,
    [CapChinhQuyen] nvarchar(max) NOT NULL,
    [KyHieuMau] nvarchar(max) NULL,
    [ThuTuSapXep] int NOT NULL,
    [TrangThai] bit NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucVanBans] PRIMARY KEY ([Id])
);

CREATE TABLE [DanhMucQuyTrinhSoanThaos] (
    [Id] uniqueidentifier NOT NULL,
    [MaQuyTrinh] nvarchar(max) NOT NULL,
    [TenQuyTrinh] nvarchar(max) NOT NULL,
    [DanhMucVanBanId] uniqueidentifier NULL,
    [CapApDung] nvarchar(max) NULL,
    [PhienBan] int NOT NULL,
    [TrangThai] bit NOT NULL,
    [NgayHieuLuc] datetime2 NULL,
    [NgayHetHieuLuc] datetime2 NULL,
    [MoTa] nvarchar(max) NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucQuyTrinhSoanThaos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DanhMucQuyTrinhSoanThaos_DanhMucVanBans_DanhMucVanBanId] FOREIGN KEY ([DanhMucVanBanId]) REFERENCES [DanhMucVanBans] ([Id])
);

CREATE TABLE [DanhMucBuocQuyTrinhs] (
    [Id] uniqueidentifier NOT NULL,
    [QuyTrinhSoanThaoId] uniqueidentifier NOT NULL,
    [MaBuoc] nvarchar(max) NOT NULL,
    [TenBuoc] nvarchar(max) NOT NULL,
    [ThuTuSapXep] int NOT NULL,
    [LoaiBuoc] nvarchar(max) NOT NULL,
    [BatBuoc] bit NOT NULL,
    [ChoPhepBoQua] bit NOT NULL,
    [ChoPhepQuayLui] bit NOT NULL,
    [CachHoanThanh] nvarchar(max) NULL,
    [SoLuongPhanHoiToiThieu] int NULL,
    [YeuCauFileDinhKem] bit NOT NULL,
    [SoLanTraLaiToiDa] int NOT NULL,
    [SoNgayXuLyTieuChuan] int NULL,
    [SoNgayCanhBaoSapHan] int NULL,
    [DonViTiepNhanMacDinhId] uniqueidentifier NULL,
    [MoTa] nvarchar(max) NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucBuocQuyTrinhs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DanhMucBuocQuyTrinhs_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId] FOREIGN KEY ([QuyTrinhSoanThaoId]) REFERENCES [DanhMucQuyTrinhSoanThaos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DanhMucChuyenBuocQuyTrinhs] (
    [Id] uniqueidentifier NOT NULL,
    [QuyTrinhSoanThaoId] uniqueidentifier NOT NULL,
    [TuBuocId] uniqueidentifier NOT NULL,
    [DenBuocId] uniqueidentifier NOT NULL,
    [DieuKienKetQua] nvarchar(max) NOT NULL,
    [LaNhanhMacDinh] bit NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_DanhMucChuyenBuocQuyTrinhs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DanhMucChuyenBuocQuyTrinhs_DanhMucBuocQuyTrinhs_DenBuocId] FOREIGN KEY ([DenBuocId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_DanhMucChuyenBuocQuyTrinhs_DanhMucBuocQuyTrinhs_TuBuocId] FOREIGN KEY ([TuBuocId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_DanhMucChuyenBuocQuyTrinhs_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId] FOREIGN KEY ([QuyTrinhSoanThaoId]) REFERENCES [DanhMucQuyTrinhSoanThaos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [HoSoVanBans] (
    [Id] uniqueidentifier NOT NULL,
    [MaHoSo] nvarchar(max) NOT NULL,
    [TenHoSo] nvarchar(max) NOT NULL,
    [DanhMucVanBanId] uniqueidentifier NOT NULL,
    [QuyTrinhSoanThaoId] uniqueidentifier NOT NULL,
    [BuocHienTaiId] uniqueidentifier NULL,
    [DanhMucTrangThaiId] uniqueidentifier NULL,
    [DonViSoanThaoId] uniqueidentifier NOT NULL,
    [NguoiTaoId] uniqueidentifier NOT NULL,
    [NgayTaoHoSo] datetime2 NOT NULL,
    [HanXuLy] datetime2 NULL,
    [NgayHoanThanh] datetime2 NULL,
    [SoLanTraLaiHienTai] int NOT NULL,
    [AttachedFileGroupId] uniqueidentifier NULL,
    [MoTa] nvarchar(max) NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_HoSoVanBans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HoSoVanBans_DanhMucBuocQuyTrinhs_BuocHienTaiId] FOREIGN KEY ([BuocHienTaiId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_HoSoVanBans_DanhMucDonVis_DonViSoanThaoId] FOREIGN KEY ([DonViSoanThaoId]) REFERENCES [DanhMucDonVis] ([Id]),
    CONSTRAINT [FK_HoSoVanBans_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId] FOREIGN KEY ([QuyTrinhSoanThaoId]) REFERENCES [DanhMucQuyTrinhSoanThaos] ([Id]),
    CONSTRAINT [FK_HoSoVanBans_DanhMucTrangThais_DanhMucTrangThaiId] FOREIGN KEY ([DanhMucTrangThaiId]) REFERENCES [DanhMucTrangThais] ([Id]),
    CONSTRAINT [FK_HoSoVanBans_DanhMucVanBans_DanhMucVanBanId] FOREIGN KEY ([DanhMucVanBanId]) REFERENCES [DanhMucVanBans] ([Id]),
    CONSTRAINT [FK_HoSoVanBans_Users_NguoiTaoId] FOREIGN KEY ([NguoiTaoId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [HoSoVanBanDanhGias] (
    [Id] uniqueidentifier NOT NULL,
    [HoSoVanBanId] uniqueidentifier NOT NULL,
    [BuocQuyTrinhId] uniqueidentifier NOT NULL,
    [LanDanhGia] int NOT NULL,
    [DonViDanhGiaId] uniqueidentifier NOT NULL,
    [NguoiDanhGiaId] uniqueidentifier NULL,
    [NgayDanhGia] datetime2 NOT NULL,
    [KetQuaDanhGia] nvarchar(max) NULL,
    [NoiDungDanhGia] nvarchar(max) NULL,
    [YeuCauChinhSua] nvarchar(max) NULL,
    [AttachedFileGroupId] uniqueidentifier NULL,
    [TraLaiBuocId] uniqueidentifier NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_HoSoVanBanDanhGias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HoSoVanBanDanhGias_DanhMucBuocQuyTrinhs_BuocQuyTrinhId] FOREIGN KEY ([BuocQuyTrinhId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_HoSoVanBanDanhGias_DanhMucBuocQuyTrinhs_TraLaiBuocId] FOREIGN KEY ([TraLaiBuocId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_HoSoVanBanDanhGias_DanhMucDonVis_DonViDanhGiaId] FOREIGN KEY ([DonViDanhGiaId]) REFERENCES [DanhMucDonVis] ([Id]),
    CONSTRAINT [FK_HoSoVanBanDanhGias_HoSoVanBans_HoSoVanBanId] FOREIGN KEY ([HoSoVanBanId]) REFERENCES [HoSoVanBans] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HoSoVanBanDanhGias_Users_NguoiDanhGiaId] FOREIGN KEY ([NguoiDanhGiaId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [HoSoVanBanLayYKiens] (
    [Id] uniqueidentifier NOT NULL,
    [HoSoVanBanId] uniqueidentifier NOT NULL,
    [BuocQuyTrinhId] uniqueidentifier NOT NULL,
    [NguoiDuocLayYKienId] uniqueidentifier NULL,
    [DonViDuocLayYKienId] uniqueidentifier NULL,
    [NoiDungYeuCau] nvarchar(max) NULL,
    [NoiDungPhanHoi] nvarchar(max) NULL,
    [NgayGui] datetime2 NOT NULL,
    [HanPhanHoi] datetime2 NULL,
    [NgayPhanHoi] datetime2 NULL,
    [TrangThaiPhanHoi] nvarchar(max) NULL,
    [AttachedFileGroupId] uniqueidentifier NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_HoSoVanBanLayYKiens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HoSoVanBanLayYKiens_DanhMucBuocQuyTrinhs_BuocQuyTrinhId] FOREIGN KEY ([BuocQuyTrinhId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_HoSoVanBanLayYKiens_DanhMucDonVis_DonViDuocLayYKienId] FOREIGN KEY ([DonViDuocLayYKienId]) REFERENCES [DanhMucDonVis] ([Id]),
    CONSTRAINT [FK_HoSoVanBanLayYKiens_HoSoVanBans_HoSoVanBanId] FOREIGN KEY ([HoSoVanBanId]) REFERENCES [HoSoVanBans] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HoSoVanBanLayYKiens_Users_NguoiDuocLayYKienId] FOREIGN KEY ([NguoiDuocLayYKienId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [HoSoVanBanXuLys] (
    [Id] uniqueidentifier NOT NULL,
    [HoSoVanBanId] uniqueidentifier NOT NULL,
    [BuocQuyTrinhId] uniqueidentifier NOT NULL,
    [LanXuLy] int NOT NULL,
    [DonViXuLyId] uniqueidentifier NOT NULL,
    [NguoiXuLyId] uniqueidentifier NULL,
    [NgayNhan] datetime2 NOT NULL,
    [HanXuLy] datetime2 NULL,
    [NgayXuLy] datetime2 NULL,
    [KetQuaXuLy] nvarchar(max) NULL,
    [NoiDungXuLy] nvarchar(max) NULL,
    [DanhMucTrangThaiId] uniqueidentifier NULL,
    [IsCurrent] bit NOT NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_HoSoVanBanXuLys] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HoSoVanBanXuLys_DanhMucBuocQuyTrinhs_BuocQuyTrinhId] FOREIGN KEY ([BuocQuyTrinhId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_HoSoVanBanXuLys_DanhMucDonVis_DonViXuLyId] FOREIGN KEY ([DonViXuLyId]) REFERENCES [DanhMucDonVis] ([Id]),
    CONSTRAINT [FK_HoSoVanBanXuLys_DanhMucTrangThais_DanhMucTrangThaiId] FOREIGN KEY ([DanhMucTrangThaiId]) REFERENCES [DanhMucTrangThais] ([Id]),
    CONSTRAINT [FK_HoSoVanBanXuLys_HoSoVanBans_HoSoVanBanId] FOREIGN KEY ([HoSoVanBanId]) REFERENCES [HoSoVanBans] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HoSoVanBanXuLys_Users_NguoiXuLyId] FOREIGN KEY ([NguoiXuLyId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [HoSoVanBanPhanHoiDanhGias] (
    [Id] uniqueidentifier NOT NULL,
    [HoSoVanBanDanhGiaId] uniqueidentifier NOT NULL,
    [HoSoVanBanId] uniqueidentifier NOT NULL,
    [LanDanhGia] int NOT NULL,
    [DonViSoanThaoId] uniqueidentifier NOT NULL,
    [NguoiPhanHoiId] uniqueidentifier NULL,
    [NgayPhanHoi] datetime2 NOT NULL,
    [NoiDungGiaiTrinh] nvarchar(max) NULL,
    [AttachedFileGroupId] uniqueidentifier NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_HoSoVanBanPhanHoiDanhGias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HoSoVanBanPhanHoiDanhGias_DanhMucDonVis_DonViSoanThaoId] FOREIGN KEY ([DonViSoanThaoId]) REFERENCES [DanhMucDonVis] ([Id]),
    CONSTRAINT [FK_HoSoVanBanPhanHoiDanhGias_HoSoVanBanDanhGias_HoSoVanBanDanhGiaId] FOREIGN KEY ([HoSoVanBanDanhGiaId]) REFERENCES [HoSoVanBanDanhGias] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HoSoVanBanPhanHoiDanhGias_HoSoVanBans_HoSoVanBanId] FOREIGN KEY ([HoSoVanBanId]) REFERENCES [HoSoVanBans] ([Id]),
    CONSTRAINT [FK_HoSoVanBanPhanHoiDanhGias_Users_NguoiPhanHoiId] FOREIGN KEY ([NguoiPhanHoiId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [HoSoVanBanBuocThoiHans] (
    [Id] uniqueidentifier NOT NULL,
    [HoSoVanBanId] uniqueidentifier NOT NULL,
    [BuocQuyTrinhId] uniqueidentifier NOT NULL,
    [ThuTuSapXep] int NOT NULL,
    [SoNgayXuLy] int NULL,
    [SoNgayCanhBaoSapHan] int NULL,
    [GhiChu] nvarchar(max) NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedBy] uniqueidentifier NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_HoSoVanBanBuocThoiHans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HoSoVanBanBuocThoiHans_DanhMucBuocQuyTrinhs_BuocQuyTrinhId] FOREIGN KEY ([BuocQuyTrinhId]) REFERENCES [DanhMucBuocQuyTrinhs] ([Id]),
    CONSTRAINT [FK_HoSoVanBanBuocThoiHans_HoSoVanBans_HoSoVanBanId] FOREIGN KEY ([HoSoVanBanId]) REFERENCES [HoSoVanBans] ([Id]) ON DELETE CASCADE
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

CREATE INDEX [IX_DanhMucBuocQuyTrinhs_QuyTrinhSoanThaoId] ON [DanhMucBuocQuyTrinhs] ([QuyTrinhSoanThaoId]);

CREATE INDEX [IX_DanhMucChuyenBuocQuyTrinhs_DenBuocId] ON [DanhMucChuyenBuocQuyTrinhs] ([DenBuocId]);

CREATE INDEX [IX_DanhMucChuyenBuocQuyTrinhs_QuyTrinhSoanThaoId] ON [DanhMucChuyenBuocQuyTrinhs] ([QuyTrinhSoanThaoId]);

CREATE INDEX [IX_DanhMucChuyenBuocQuyTrinhs_TuBuocId] ON [DanhMucChuyenBuocQuyTrinhs] ([TuBuocId]);

CREATE INDEX [IX_DanhMucPhongBans_DanhMucDonViId] ON [DanhMucPhongBans] ([DanhMucDonViId]);

CREATE INDEX [IX_DanhMucQuyTrinhSoanThaos_DanhMucVanBanId] ON [DanhMucQuyTrinhSoanThaos] ([DanhMucVanBanId]);

CREATE INDEX [IX_HoSoVanBanDanhGias_BuocQuyTrinhId] ON [HoSoVanBanDanhGias] ([BuocQuyTrinhId]);

CREATE INDEX [IX_HoSoVanBanDanhGias_DonViDanhGiaId] ON [HoSoVanBanDanhGias] ([DonViDanhGiaId]);

CREATE INDEX [IX_HoSoVanBanDanhGias_HoSoVanBanId] ON [HoSoVanBanDanhGias] ([HoSoVanBanId]);

CREATE INDEX [IX_HoSoVanBanDanhGias_NguoiDanhGiaId] ON [HoSoVanBanDanhGias] ([NguoiDanhGiaId]);

CREATE INDEX [IX_HoSoVanBanDanhGias_TraLaiBuocId] ON [HoSoVanBanDanhGias] ([TraLaiBuocId]);

CREATE INDEX [IX_HoSoVanBanLayYKiens_BuocQuyTrinhId] ON [HoSoVanBanLayYKiens] ([BuocQuyTrinhId]);

CREATE INDEX [IX_HoSoVanBanLayYKiens_DonViDuocLayYKienId] ON [HoSoVanBanLayYKiens] ([DonViDuocLayYKienId]);

CREATE INDEX [IX_HoSoVanBanLayYKiens_HoSoVanBanId] ON [HoSoVanBanLayYKiens] ([HoSoVanBanId]);

CREATE INDEX [IX_HoSoVanBanLayYKiens_NguoiDuocLayYKienId] ON [HoSoVanBanLayYKiens] ([NguoiDuocLayYKienId]);

CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_DonViSoanThaoId] ON [HoSoVanBanPhanHoiDanhGias] ([DonViSoanThaoId]);

CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanDanhGiaId] ON [HoSoVanBanPhanHoiDanhGias] ([HoSoVanBanDanhGiaId]);

CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanId] ON [HoSoVanBanPhanHoiDanhGias] ([HoSoVanBanId]);

CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_NguoiPhanHoiId] ON [HoSoVanBanPhanHoiDanhGias] ([NguoiPhanHoiId]);

CREATE INDEX [IX_HoSoVanBanBuocThoiHans_BuocQuyTrinhId] ON [HoSoVanBanBuocThoiHans] ([BuocQuyTrinhId]);

CREATE INDEX [IX_HoSoVanBanBuocThoiHans_HoSoVanBanId] ON [HoSoVanBanBuocThoiHans] ([HoSoVanBanId]);

CREATE INDEX [IX_HoSoVanBans_BuocHienTaiId] ON [HoSoVanBans] ([BuocHienTaiId]);

CREATE INDEX [IX_HoSoVanBans_DanhMucTrangThaiId] ON [HoSoVanBans] ([DanhMucTrangThaiId]);

CREATE INDEX [IX_HoSoVanBans_DanhMucVanBanId] ON [HoSoVanBans] ([DanhMucVanBanId]);

CREATE INDEX [IX_HoSoVanBans_DonViSoanThaoId] ON [HoSoVanBans] ([DonViSoanThaoId]);

CREATE INDEX [IX_HoSoVanBans_NguoiTaoId] ON [HoSoVanBans] ([NguoiTaoId]);

CREATE INDEX [IX_HoSoVanBans_QuyTrinhSoanThaoId] ON [HoSoVanBans] ([QuyTrinhSoanThaoId]);

CREATE INDEX [IX_HoSoVanBanXuLys_BuocQuyTrinhId] ON [HoSoVanBanXuLys] ([BuocQuyTrinhId]);

CREATE INDEX [IX_HoSoVanBanXuLys_DanhMucTrangThaiId] ON [HoSoVanBanXuLys] ([DanhMucTrangThaiId]);

CREATE INDEX [IX_HoSoVanBanXuLys_DonViXuLyId] ON [HoSoVanBanXuLys] ([DonViXuLyId]);

CREATE INDEX [IX_HoSoVanBanXuLys_HoSoVanBanId] ON [HoSoVanBanXuLys] ([HoSoVanBanId]);

CREATE INDEX [IX_HoSoVanBanXuLys_NguoiXuLyId] ON [HoSoVanBanXuLys] ([NguoiXuLyId]);

DECLARE @Now DATETIME = GETDATE();

INSERT INTO DanhMucDonVis
(
    Id, TenDonVi, [Level], STTSapXep, DonViChuQuanId,
    DiaChi, MaQHNS, SoDienThoai, ChucDanhQuanLy, HoVaTenNguoiQuanLy,
    PhanLoaiDonVi, TinhNangThanhToan, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
)
SELECT
    v.Id, v.TenDonVi, v.[Level], v.STTSapXep, v.DonViChuQuanId,
    NULL, NULL, NULL, NULL, NULL,
    v.PhanLoaiDonVi, 1, '11111111-1111-1111-1111-111111111111', @Now, '11111111-1111-1111-1111-111111111111', @Now
FROM
(
    VALUES
    ('40000000-0000-0000-0000-000000000001', N'Sở Nội vụ', 2, 1, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000002', N'Sở Tư pháp', 2, 2, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000003', N'Sở Tài chính', 2, 3, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000004', N'Sở Công Thương', 2, 4, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000005', N'Sở Nông nghiệp và Môi trường', 2, 5, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000006', N'Sở Xây dựng', 2, 6, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000007', N'Sở Khoa học và Công nghệ', 2, 7, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000008', N'Sở Văn hóa, Thể thao và Du lịch', 2, 8, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000009', N'Sở Giáo dục và Đào tạo', 2, 9, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000010', N'Sở Y tế', 2, 10, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000011', N'Sở Dân tộc và Tôn giáo', 2, 11, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000012', N'Thanh tra tỉnh', 2, 12, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000013', N'Văn phòng UBND tỉnh', 2, 13, '00000000-0000-0000-0000-000000000000', N'CQNN'),
    ('40000000-0000-0000-0000-000000000014', N'Văn phòng Đoàn đại biểu Quốc hội và HĐND tỉnh', 2, 14, '00000000-0000-0000-0000-000000000000', N'CQNN')
) AS v(Id, TenDonVi, [Level], STTSapXep, DonViChuQuanId, PhanLoaiDonVi)
WHERE NOT EXISTS
(
    SELECT 1
    FROM DanhMucDonVis d
    WHERE d.TenDonVi = v.TenDonVi
);

INSERT INTO Users
(
    Id, Username, Email, Name, Password, SSA, DanhMucDonViId, DoanhNghiepId,
    OTPSecretKey, Status, FirstLogin, LoginCount, TenDonViBaoCao, TenDonViChuQuanBaoCao,
    DiaDanh, ChucDanhKy, HoTenNguoiKy, KyHieuDonVi, Content, Menu, Theme,
    GroupPermissionId, VNId, AgentId, ScanDeviceId, ScanDeviceName, [Level],
    CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
)
SELECT
    v.Id, v.Username, v.Email, v.Name, v.Password, 0, v.DanhMucDonViId, '00000000-0000-0000-0000-000000000000',
    v.OTPSecretKey, N'Kích hoạt', 1, 0, NULL, NULL,
    NULL, NULL, NULL, NULL, 'Max', 'Fixed', 'Light',
    '00000000-0000-0000-0000-000000000000', 0, NULL, NULL, NULL, N'Nhà nước',
    '11111111-1111-1111-1111-111111111111', @Now, '11111111-1111-1111-1111-111111111111', @Now
FROM
(
    VALUES
    ('50000000-0000-0000-0000-000000000001', 'sonoivu', 'sonoivu@qlvb.local', N'Sở Nội vụ', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000001', 'JBSWY3DPEHPK3PXPAAAAAAAA'),
    ('50000000-0000-0000-0000-000000000002', 'sotuphap', 'sotuphap@qlvb.local', N'Sở Tư pháp', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000002', 'JBSWY3DPEHPK3PXPBBBBBBBB'),
    ('50000000-0000-0000-0000-000000000003', 'sotaichinh', 'sotaichinh@qlvb.local', N'Sở Tài chính', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000003', 'JBSWY3DPEHPK3PXPCCCCCCCC'),
    ('50000000-0000-0000-0000-000000000004', 'socongthuong', 'socongthuong@qlvb.local', N'Sở Công Thương', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000004', 'JBSWY3DPEHPK3PXPDDDDDDDD'),
    ('50000000-0000-0000-0000-000000000005', 'sonongnghiepmoitruong', 'sonongnghiepmoitruong@qlvb.local', N'Sở Nông nghiệp và Môi trường', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000005', 'JBSWY3DPEHPK3PXPEEEEEEEE'),
    ('50000000-0000-0000-0000-000000000006', 'soxaydung', 'soxaydung@qlvb.local', N'Sở Xây dựng', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000006', 'JBSWY3DPEHPK3PXPFFFFFFFF'),
    ('50000000-0000-0000-0000-000000000007', 'sokhoahoccongnghe', 'sokhoahoccongnghe@qlvb.local', N'Sở Khoa học và Công nghệ', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000007', 'JBSWY3DPEHPK3PXPGGGGGGGG'),
    ('50000000-0000-0000-0000-000000000008', 'sovanhoathethaodulich', 'sovanhoathethaodulich@qlvb.local', N'Sở Văn hóa, Thể thao và Du lịch', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000008', 'JBSWY3DPEHPK3PXPHHHHHHHH'),
    ('50000000-0000-0000-0000-000000000009', 'sogiaoducdaotao', 'sogiaoducdaotao@qlvb.local', N'Sở Giáo dục và Đào tạo', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000009', 'JBSWY3DPEHPK3PXPIIIIIIII'),
    ('50000000-0000-0000-0000-000000000010', 'soyte', 'soyte@qlvb.local', N'Sở Y tế', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000010', 'JBSWY3DPEHPK3PXPJJJJJJJJ'),
    ('50000000-0000-0000-0000-000000000011', 'sodantocvatongiao', 'sodantocvatongiao@qlvb.local', N'Sở Dân tộc và Tôn giáo', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000011', 'JBSWY3DPEHPK3PXPKKKKKKKK'),
    ('50000000-0000-0000-0000-000000000012', 'thanhtratinh', 'thanhtratinh@qlvb.local', N'Thanh tra tỉnh', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000012', 'JBSWY3DPEHPK3PXPLLLLLLLL'),
    ('50000000-0000-0000-0000-000000000013', 'vanphongubndtinh', 'vanphongubndtinh@qlvb.local', N'Văn phòng UBND tỉnh', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000013', 'JBSWY3DPEHPK3PXPMMMMMMMM'),
    ('50000000-0000-0000-0000-000000000014', 'vanphongdoandbqhvahdndtinh', 'vanphongdoandbqhvahdndtinh@qlvb.local', N'Văn phòng Đoàn đại biểu Quốc hội và HĐND tỉnh', '$2a$11$Tdtl7b.x6LciSZhWoPNIaenLEtf5y6WrR4KCFgmZGKAi0GaBwoaou', '40000000-0000-0000-0000-000000000014', 'JBSWY3DPEHPK3PXPNNNNNNNN')
) AS v(Id, Username, Email, Name, Password, DanhMucDonViId, OTPSecretKey)
WHERE NOT EXISTS
(
    SELECT 1
    FROM Users u
    WHERE u.Username = v.Username
);

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
    N'Danh sách quy trình soạn thảo', 'QuyTrinhSoanThao', 'Index', NULL, 'DanhMucQuyTrinhSoanThaos',
    N'Kích hoạt', 'QuanTriHeThong', NULL
),
(
    '20000000-0000-0000-0000-000000000020',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    2, 'Group', 0, 'VanBanQPPL', '00000000-0000-0000-0000-000000000000',
    N'Chức năng xây dựng văn bản QPPL', '', '', NULL, '',
    N'Kích hoạt', NULL, 'fas fa-landmark'
),
(
    '20000000-0000-0000-0000-000000000021',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    1, 'Group', 1, 'VanBanQPPL.DangKyXayDung', '20000000-0000-0000-0000-000000000020',
    N'Đăng ký xây dựng', '', '', NULL, '',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000022',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    1, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.DanhSachDangKy', '20000000-0000-0000-0000-000000000021',
    N'Danh sách đăng ký', 'DangPhatTrien', 'DangKyXayDung', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000023',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    2, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.XetDuyetDangKy', '20000000-0000-0000-0000-000000000021',
    N'Xét duyệt đăng ký', 'DangPhatTrien', 'XetDuyetDangKy', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000024',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    3, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.PheDuyetDangKy', '20000000-0000-0000-0000-000000000021',
    N'Phê duyệt đăng ký', 'DangPhatTrien', 'PheDuyetDangKy', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000025',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    2, 'Group', 1, 'VanBanQPPL.XayDungVanBan', '20000000-0000-0000-0000-000000000020',
    N'Xây dựng văn bản', '', '', NULL, '',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000026',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    1, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.DanhSachVanBan', '20000000-0000-0000-0000-000000000025',
    N'Danh sách văn bản', 'HoSoVanBan', 'Index', NULL, 'HoSoVanBans',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000027',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    2, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.XayDungVanBan', '20000000-0000-0000-0000-000000000025',
    N'Xây dựng văn bản', 'DangPhatTrien', 'XayDungVanBan', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000028',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    3, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.GiaHanXayDung', '20000000-0000-0000-0000-000000000025',
    N'Gia hạn thời gian xây dựng', 'DangPhatTrien', 'GiaHanXayDung', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000029',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    4, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.XetDuyetVanBan', '20000000-0000-0000-0000-000000000025',
    N'Xét duyệt văn bản', 'DangPhatTrien', 'XetDuyetVanBan', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000030',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    5, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.PheDuyetVanBan', '20000000-0000-0000-0000-000000000025',
    N'Phê duyệt văn bản', 'DangPhatTrien', 'PheDuyetVanBan', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000031',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    3, 'Group', 0, 'ThiHanhPhapLuat', '00000000-0000-0000-0000-000000000000',
    N'Thực hiện thi hành pháp luật', '', '', NULL, '',
    N'Kích hoạt', NULL, 'fas fa-balance-scale'
),
(
    '20000000-0000-0000-0000-000000000032',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    1, 'Detail', 1, 'ThiHanhPhapLuat.DanhSachKeHoach', '20000000-0000-0000-0000-000000000031',
    N'Danh sách kế hoạch', 'DangPhatTrien', 'DanhSachKeHoach', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000033',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    2, 'Detail', 1, 'ThiHanhPhapLuat.QuaTrinhToChucThucHien', '20000000-0000-0000-0000-000000000031',
    N'Danh sách quá trình tổ chức thực hiện', 'DangPhatTrien', 'QuaTrinhToChucThucHien', NULL, 'DangPhatTrien',
    N'Kích hoạt', NULL, NULL
),
(
    '20000000-0000-0000-0000-000000000034',
    '11111111-1111-1111-1111-111111111111',
    @Now,
    '11111111-1111-1111-1111-111111111111',
    @Now,
    3, 'Detail', 1, 'ThiHanhPhapLuat.DanhGiaKetQua', '20000000-0000-0000-0000-000000000031',
    N'Đánh giá kết quả', 'DangPhatTrien', 'DanhGiaKetQua', NULL, 'DangPhatTrien',
    N'Kích hoạt', 'QuanTriHeThong', NULL
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260723082931_InitDb', N'9.0.2');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724093000_AddDanhMucTrangThai', N'9.0.7');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724130000_AddWorkflowSoanThao', N'9.0.7');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724160000_AddWorkflowStepDeadlineTracking', N'9.0.7');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724173000_AddHoSoStepDeadlines', N'9.0.7');

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucQuyTrinhSoanThaos
    WHERE Id = '40000000-0000-0000-0000-000000000001'
)
BEGIN
    INSERT INTO DanhMucQuyTrinhSoanThaos
    (
        Id, MaQuyTrinh, TenQuyTrinh, DanhMucVanBanId, CapApDung, PhienBan, TrangThai,
        NgayHieuLuc, NgayHetHieuLuc, MoTa, GhiChu,
        CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    (
        '40000000-0000-0000-0000-000000000001',
        N'QT_QPPL_7_BUOC',
        N'Quy trình xây dựng văn bản QPPL 7 bước tiêu chuẩn',
        NULL,
        N'Tỉnh,Xã',
        1,
        1,
        @Now,
        NULL,
        N'Quy trình mẫu cấu hình 7 bước xây dựng văn bản quy phạm pháp luật, cho phép thay đổi linh hoạt theo danh mục.',
        N'Bản seed mặc định phục vụ khởi tạo hệ thống.',
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = '40000000-0000-0000-0000-000000000001'
)
BEGIN
    INSERT INTO DanhMucBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, MaBuoc, TenBuoc, ThuTuSapXep, LoaiBuoc,
        BatBuoc, ChoPhepBoQua, ChoPhepQuayLui, CachHoanThanh, SoLuongPhanHoiToiThieu,
        YeuCauFileDinhKem, SoLanTraLaiToiDa, SoNgayXuLyTieuChuan, SoNgayCanhBaoSapHan, DonViTiepNhanMacDinhId, MoTa, GhiChu,
        CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    (
        '41000000-0000-0000-0000-000000000001',
        '40000000-0000-0000-0000-000000000001',
        N'BUOC_01_DANG_KY',
        N'Lập đề nghị/Đăng ký danh mục',
        1,
        N'DangKy',
        1,
        0,
        0,
        N'HoanThanhDangKy',
        NULL,
        0,
        0,
        3,
        1,
        NULL,
        N'Đơn vị đề xuất khởi tạo đề nghị hoặc đăng ký danh mục xây dựng văn bản.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '41000000-0000-0000-0000-000000000002',
        '40000000-0000-0000-0000-000000000001',
        N'BUOC_02_THONG_NHAT',
        N'Tiep nhan/Xet duyet dang ky',
        2,
        N'PheDuyet',
        1,
        0,
        1,
        N'DongYHoacKhongDongY',
        NULL,
        0,
        0,
        2,
        1,
        '40000000-0000-0000-0000-000000000013',
        N'VP UBND tiep nhan ho so do So Tai chinh gui len va cho y kien dong y/khong dong y.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '41000000-0000-0000-0000-000000000003',
        '40000000-0000-0000-0000-000000000001',
        N'BUOC_03_SOAN_THAO',
        N'Soạn thảo VB',
        3,
        N'SoanThao',
        1,
        0,
        1,
        N'HoanThanhDuThao',
        NULL,
        1,
        0,
        5,
        2,
        NULL,
        N'Đơn vị soạn thảo cập nhật dự thảo và hoàn thiện hồ sơ trước khi lấy ý kiến.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '41000000-0000-0000-0000-000000000004',
        '40000000-0000-0000-0000-000000000001',
        N'BUOC_04_LAY_Y_KIEN',
        N'Lấy ý kiến',
        4,
        N'LayYKien',
        1,
        0,
        0,
        N'GanKetQuaVaDinhKemFileDeHoanTat',
        1,
        1,
        0,
        7,
        2,
        NULL,
        N'Gửi lấy ý kiến, gán kết quả phản hồi và đính kèm file trước khi hoàn thành bước.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '41000000-0000-0000-0000-000000000005',
        '40000000-0000-0000-0000-000000000001',
        N'BUOC_05_THAM_DINH',
        N'Thẩm định/Đánh giá',
        5,
        N'DanhGia',
        1,
        0,
        1,
        N'DanhGiaDatHoacTraLai',
        NULL,
        1,
        3,
        5,
        2,
        NULL,
        N'Đơn vị đánh giá có thể kết luận đạt hoặc trả lại đơn vị soạn thảo tối đa 3 lần, mỗi lần đều lưu lịch sử và file.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '41000000-0000-0000-0000-000000000006',
        '40000000-0000-0000-0000-000000000001',
        N'BUOC_06_TRINH_THAM_QUYEN',
        N'Trình cơ quan có thẩm quyền',
        6,
        N'PheDuyet',
        1,
        0,
        0,
        N'TrinhPheDuyetHoSo',
        NULL,
        0,
        0,
        3,
        1,
        '40000000-0000-0000-0000-000000000002',
        N'Sở Tư pháp tiếp nhận tham mưu hồ sơ trình cơ quan có thẩm quyền.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '41000000-0000-0000-0000-000000000007',
        '40000000-0000-0000-0000-000000000001',
        N'BUOC_07_BAN_HANH',
        N'Ban hành',
        7,
        N'BanHanh',
        1,
        0,
        0,
        N'CapNhatVanBanBanHanh',
        NULL,
        1,
        0,
        2,
        1,
        NULL,
        N'Hoàn tất quy trình bằng việc cập nhật văn bản đã ban hành và file chính thức.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucChuyenBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = '40000000-0000-0000-0000-000000000001'
)
BEGIN
    INSERT INTO DanhMucChuyenBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, TuBuocId, DenBuocId, DieuKienKetQua, LaNhanhMacDinh,
        MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    (
        '42000000-0000-0000-0000-000000000001',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000002',
        N'HOAN_THANH_DANG_KY',
        1,
        N'Hoàn thành đăng ký để chuyển sang bước văn bản thống nhất/đồng ý.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000002',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000002',
        '41000000-0000-0000-0000-000000000003',
        N'DONG_Y',
        1,
        N'VP UBND đồng ý thì chuyển sang bước soạn thảo.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000003',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000002',
        '41000000-0000-0000-0000-000000000001',
        N'KHONG_DONG_Y',
        0,
        N'Không đồng ý thì quay lại bước đăng ký để điều chỉnh đề nghị.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000004',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000003',
        '41000000-0000-0000-0000-000000000004',
        N'HOAN_THANH_DU_THAO',
        1,
        N'Hoàn thành dự thảo để chuyển sang lấy ý kiến.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000005',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000004',
        '41000000-0000-0000-0000-000000000005',
        N'DA_GAN_KET_QUA_Y_KIEN',
        1,
        N'Bước lấy ý kiến hoàn tất sau khi gán kết quả và đính kèm file phản hồi.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000006',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000005',
        '41000000-0000-0000-0000-000000000006',
        N'DAT',
        1,
        N'Kết quả thẩm định đạt thì chuyển sang trình cơ quan có thẩm quyền.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000007',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000005',
        '41000000-0000-0000-0000-000000000003',
        N'KHONG_DAT_LAN_1',
        0,
        N'Thẩm định không đạt lần 1, trả lại bước soạn thảo để chỉnh sửa và giải trình.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000008',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000005',
        '41000000-0000-0000-0000-000000000003',
        N'KHONG_DAT_LAN_2',
        0,
        N'Thẩm định không đạt lần 2, tiếp tục quay lại bước soạn thảo.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000009',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000005',
        '41000000-0000-0000-0000-000000000003',
        N'KHONG_DAT_LAN_3',
        0,
        N'Thẩm định không đạt lần 3, là lần trả lại tối đa theo cấu hình.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000010',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000006',
        '41000000-0000-0000-0000-000000000007',
        N'TRINH_THANH_CONG',
        1,
        N'Trình thành công thì chuyển sang bước ban hành.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    ),
    (
        '42000000-0000-0000-0000-000000000011',
        '40000000-0000-0000-0000-000000000001',
        '41000000-0000-0000-0000-000000000006',
        '41000000-0000-0000-0000-000000000003',
        N'KHONG_DONG_Y',
        0,
        N'Trình chưa được chấp thuận, trả lại bước soạn thảo để chỉnh sửa và hoàn thiện hồ sơ.',
        NULL,
        '11111111-1111-1111-1111-111111111111',
        @Now,
        '11111111-1111-1111-1111-111111111111',
        @Now
    );
END;

COMMIT;
GO

