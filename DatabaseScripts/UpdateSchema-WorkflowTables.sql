IF OBJECT_ID(N'[DanhMucQuyTrinhSoanThaos]') IS NULL
BEGIN
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
END;

IF OBJECT_ID(N'[DanhMucBuocQuyTrinhs]') IS NULL
BEGIN
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
        CONSTRAINT [FK_DanhMucBuocQuyTrinhs_DanhMucDonVis_DonViTiepNhanMacDinhId] FOREIGN KEY ([DonViTiepNhanMacDinhId]) REFERENCES [DanhMucDonVis] ([Id]),
        CONSTRAINT [FK_DanhMucBuocQuyTrinhs_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId] FOREIGN KEY ([QuyTrinhSoanThaoId]) REFERENCES [DanhMucQuyTrinhSoanThaos] ([Id]) ON DELETE CASCADE
    );
END;

IF COL_LENGTH('DanhMucBuocQuyTrinhs', 'SoNgayXuLyTieuChuan') IS NULL
BEGIN
    ALTER TABLE [DanhMucBuocQuyTrinhs] ADD [SoNgayXuLyTieuChuan] int NULL;
END;

IF COL_LENGTH('DanhMucBuocQuyTrinhs', 'SoNgayCanhBaoSapHan') IS NULL
BEGIN
    ALTER TABLE [DanhMucBuocQuyTrinhs] ADD [SoNgayCanhBaoSapHan] int NULL;
END;

IF COL_LENGTH('DanhMucBuocQuyTrinhs', 'DonViTiepNhanMacDinhId') IS NULL
BEGIN
    ALTER TABLE [DanhMucBuocQuyTrinhs] ADD [DonViTiepNhanMacDinhId] uniqueidentifier NULL;
END;

IF COL_LENGTH('DanhMucBuocQuyTrinhs', 'DonViTiepNhanMacDinhId') IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM sys.foreign_keys
        WHERE [name] = 'FK_DanhMucBuocQuyTrinhs_DanhMucDonVis_DonViTiepNhanMacDinhId'
    )
BEGIN
    ALTER TABLE [DanhMucBuocQuyTrinhs]
    WITH NOCHECK
    ADD CONSTRAINT [FK_DanhMucBuocQuyTrinhs_DanhMucDonVis_DonViTiepNhanMacDinhId]
        FOREIGN KEY ([DonViTiepNhanMacDinhId]) REFERENCES [DanhMucDonVis] ([Id]);
END;

EXEC sp_executesql N'
UPDATE [DanhMucBuocQuyTrinhs]
SET
    [SoNgayXuLyTieuChuan] = CASE [MaBuoc]
        WHEN N''BUOC_01_DANG_KY'' THEN 3
        WHEN N''BUOC_02_THONG_NHAT'' THEN 2
        WHEN N''BUOC_03_SOAN_THAO'' THEN 5
        WHEN N''BUOC_04_LAY_Y_KIEN'' THEN 7
        WHEN N''BUOC_05_THAM_DINH'' THEN 5
        WHEN N''BUOC_06_TRINH_THAM_QUYEN'' THEN 3
        WHEN N''BUOC_07_BAN_HANH'' THEN 2
        ELSE [SoNgayXuLyTieuChuan]
    END,
    [SoNgayCanhBaoSapHan] = CASE [MaBuoc]
        WHEN N''BUOC_01_DANG_KY'' THEN 1
        WHEN N''BUOC_02_THONG_NHAT'' THEN 1
        WHEN N''BUOC_03_SOAN_THAO'' THEN 2
        WHEN N''BUOC_04_LAY_Y_KIEN'' THEN 2
        WHEN N''BUOC_05_THAM_DINH'' THEN 2
        WHEN N''BUOC_06_TRINH_THAM_QUYEN'' THEN 1
        WHEN N''BUOC_07_BAN_HANH'' THEN 1
        ELSE [SoNgayCanhBaoSapHan]
    END,
    [DonViTiepNhanMacDinhId] = CASE [MaBuoc]
        WHEN N''BUOC_02_THONG_NHAT'' THEN ''40000000-0000-0000-0000-000000000013''
        WHEN N''BUOC_06_TRINH_THAM_QUYEN'' THEN ''40000000-0000-0000-0000-000000000002''
        ELSE [DonViTiepNhanMacDinhId]
    END
WHERE [MaBuoc] IN (
    N''BUOC_01_DANG_KY'',
    N''BUOC_02_THONG_NHAT'',
    N''BUOC_03_SOAN_THAO'',
    N''BUOC_04_LAY_Y_KIEN'',
    N''BUOC_05_THAM_DINH'',
    N''BUOC_06_TRINH_THAM_QUYEN'',
    N''BUOC_07_BAN_HANH''
);';

IF OBJECT_ID(N'[DanhMucChuyenBuocQuyTrinhs]') IS NULL
BEGIN
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
END;

IF OBJECT_ID(N'[HoSoVanBans]') IS NULL
BEGIN
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
END;

IF OBJECT_ID(N'[HoSoVanBanDanhGias]') IS NULL
BEGIN
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
END;

IF OBJECT_ID(N'[HoSoVanBanLayYKiens]') IS NULL
BEGIN
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
END;

IF OBJECT_ID(N'[HoSoVanBanXuLys]') IS NULL
BEGIN
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
END;

IF OBJECT_ID(N'[HoSoVanBanPhanHoiDanhGias]') IS NULL
BEGIN
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
END;

IF OBJECT_ID(N'[HoSoVanBanBuocThoiHans]') IS NULL
BEGIN
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
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DanhMucQuyTrinhSoanThaos_DanhMucVanBanId')
    CREATE INDEX [IX_DanhMucQuyTrinhSoanThaos_DanhMucVanBanId] ON [DanhMucQuyTrinhSoanThaos] ([DanhMucVanBanId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DanhMucBuocQuyTrinhs_QuyTrinhSoanThaoId')
    CREATE INDEX [IX_DanhMucBuocQuyTrinhs_QuyTrinhSoanThaoId] ON [DanhMucBuocQuyTrinhs] ([QuyTrinhSoanThaoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DanhMucChuyenBuocQuyTrinhs_DenBuocId')
    CREATE INDEX [IX_DanhMucChuyenBuocQuyTrinhs_DenBuocId] ON [DanhMucChuyenBuocQuyTrinhs] ([DenBuocId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DanhMucChuyenBuocQuyTrinhs_QuyTrinhSoanThaoId')
    CREATE INDEX [IX_DanhMucChuyenBuocQuyTrinhs_QuyTrinhSoanThaoId] ON [DanhMucChuyenBuocQuyTrinhs] ([QuyTrinhSoanThaoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DanhMucChuyenBuocQuyTrinhs_TuBuocId')
    CREATE INDEX [IX_DanhMucChuyenBuocQuyTrinhs_TuBuocId] ON [DanhMucChuyenBuocQuyTrinhs] ([TuBuocId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBans_BuocHienTaiId')
    CREATE INDEX [IX_HoSoVanBans_BuocHienTaiId] ON [HoSoVanBans] ([BuocHienTaiId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBans_DanhMucTrangThaiId')
    CREATE INDEX [IX_HoSoVanBans_DanhMucTrangThaiId] ON [HoSoVanBans] ([DanhMucTrangThaiId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBans_DanhMucVanBanId')
    CREATE INDEX [IX_HoSoVanBans_DanhMucVanBanId] ON [HoSoVanBans] ([DanhMucVanBanId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBans_DonViSoanThaoId')
    CREATE INDEX [IX_HoSoVanBans_DonViSoanThaoId] ON [HoSoVanBans] ([DonViSoanThaoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBans_NguoiTaoId')
    CREATE INDEX [IX_HoSoVanBans_NguoiTaoId] ON [HoSoVanBans] ([NguoiTaoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBans_QuyTrinhSoanThaoId')
    CREATE INDEX [IX_HoSoVanBans_QuyTrinhSoanThaoId] ON [HoSoVanBans] ([QuyTrinhSoanThaoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanDanhGias_BuocQuyTrinhId')
    CREATE INDEX [IX_HoSoVanBanDanhGias_BuocQuyTrinhId] ON [HoSoVanBanDanhGias] ([BuocQuyTrinhId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanDanhGias_DonViDanhGiaId')
    CREATE INDEX [IX_HoSoVanBanDanhGias_DonViDanhGiaId] ON [HoSoVanBanDanhGias] ([DonViDanhGiaId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanDanhGias_HoSoVanBanId')
    CREATE INDEX [IX_HoSoVanBanDanhGias_HoSoVanBanId] ON [HoSoVanBanDanhGias] ([HoSoVanBanId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanDanhGias_NguoiDanhGiaId')
    CREATE INDEX [IX_HoSoVanBanDanhGias_NguoiDanhGiaId] ON [HoSoVanBanDanhGias] ([NguoiDanhGiaId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanDanhGias_TraLaiBuocId')
    CREATE INDEX [IX_HoSoVanBanDanhGias_TraLaiBuocId] ON [HoSoVanBanDanhGias] ([TraLaiBuocId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanLayYKiens_BuocQuyTrinhId')
    CREATE INDEX [IX_HoSoVanBanLayYKiens_BuocQuyTrinhId] ON [HoSoVanBanLayYKiens] ([BuocQuyTrinhId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanLayYKiens_DonViDuocLayYKienId')
    CREATE INDEX [IX_HoSoVanBanLayYKiens_DonViDuocLayYKienId] ON [HoSoVanBanLayYKiens] ([DonViDuocLayYKienId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanLayYKiens_HoSoVanBanId')
    CREATE INDEX [IX_HoSoVanBanLayYKiens_HoSoVanBanId] ON [HoSoVanBanLayYKiens] ([HoSoVanBanId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanLayYKiens_NguoiDuocLayYKienId')
    CREATE INDEX [IX_HoSoVanBanLayYKiens_NguoiDuocLayYKienId] ON [HoSoVanBanLayYKiens] ([NguoiDuocLayYKienId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanXuLys_BuocQuyTrinhId')
    CREATE INDEX [IX_HoSoVanBanXuLys_BuocQuyTrinhId] ON [HoSoVanBanXuLys] ([BuocQuyTrinhId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanXuLys_DanhMucTrangThaiId')
    CREATE INDEX [IX_HoSoVanBanXuLys_DanhMucTrangThaiId] ON [HoSoVanBanXuLys] ([DanhMucTrangThaiId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanXuLys_DonViXuLyId')
    CREATE INDEX [IX_HoSoVanBanXuLys_DonViXuLyId] ON [HoSoVanBanXuLys] ([DonViXuLyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanXuLys_HoSoVanBanId')
    CREATE INDEX [IX_HoSoVanBanXuLys_HoSoVanBanId] ON [HoSoVanBanXuLys] ([HoSoVanBanId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanXuLys_NguoiXuLyId')
    CREATE INDEX [IX_HoSoVanBanXuLys_NguoiXuLyId] ON [HoSoVanBanXuLys] ([NguoiXuLyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanPhanHoiDanhGias_DonViSoanThaoId')
    CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_DonViSoanThaoId] ON [HoSoVanBanPhanHoiDanhGias] ([DonViSoanThaoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanDanhGiaId')
    CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanDanhGiaId] ON [HoSoVanBanPhanHoiDanhGias] ([HoSoVanBanDanhGiaId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanId')
    CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_HoSoVanBanId] ON [HoSoVanBanPhanHoiDanhGias] ([HoSoVanBanId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanPhanHoiDanhGias_NguoiPhanHoiId')
    CREATE INDEX [IX_HoSoVanBanPhanHoiDanhGias_NguoiPhanHoiId] ON [HoSoVanBanPhanHoiDanhGias] ([NguoiPhanHoiId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanBuocThoiHans_BuocQuyTrinhId')
    CREATE INDEX [IX_HoSoVanBanBuocThoiHans_BuocQuyTrinhId] ON [HoSoVanBanBuocThoiHans] ([BuocQuyTrinhId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HoSoVanBanBuocThoiHans_HoSoVanBanId')
    CREATE INDEX [IX_HoSoVanBanBuocThoiHans_HoSoVanBanId] ON [HoSoVanBanBuocThoiHans] ([HoSoVanBanId]);

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260724130000_AddWorkflowSoanThao')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260724130000_AddWorkflowSoanThao', N'9.0.7');
END;

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260724160000_AddWorkflowStepDeadlineTracking')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260724160000_AddWorkflowStepDeadlineTracking', N'9.0.7');
END;

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260724173000_AddHoSoStepDeadlines')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260724173000_AddHoSoStepDeadlines', N'9.0.7');
END;
