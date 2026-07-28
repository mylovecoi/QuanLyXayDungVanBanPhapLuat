SET NOCOUNT ON;

DECLARE @Now DATETIME = GETDATE();
DECLARE @SeedUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @WorkflowId UNIQUEIDENTIFIER = '40000000-0000-0000-0000-000000000001';

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucQuyTrinhSoanThaos
    WHERE Id = @WorkflowId
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
        @WorkflowId,
        N'QT_QPPL_7_BUOC',
        N'Quy trinh xay dung van ban QPPL 7 buoc tieu chuan',
        NULL,
        N'Tinh,Xa',
        1,
        1,
        @Now,
        NULL,
        N'Quy trinh mau cho theo doi xay dung van ban quy pham phap luat.',
        N'Seed workflow mac dinh.',
        @SeedUserId,
        @Now,
        @SeedUserId,
        @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = @WorkflowId
)
BEGIN
    INSERT INTO DanhMucBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, MaBuoc, TenBuoc, ThuTuSapXep, LoaiBuoc,
        BatBuoc, ChoPhepBoQua, ChoPhepQuayLui, CachHoanThanh, SoLuongPhanHoiToiThieu,
        YeuCauFileDinhKem, SoLanTraLaiToiDa, SoNgayXuLyTieuChuan, SoNgayCanhBaoSapHan, DonViTiepNhanMacDinhId,
        MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    (
        '41000000-0000-0000-0000-000000000001',
        @WorkflowId,
        N'BUOC_01_DANG_KY',
        N'Lap de nghi/Dang ky danh muc',
        1,
        N'DangKy',
        1, 0, 0,
        N'HoanThanhDangKy',
        NULL,
        0,
        0,
        3,
        1,
        NULL,
        N'Khoi tao de nghi xay dung van ban.',
        NULL,
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '41000000-0000-0000-0000-000000000002',
        @WorkflowId,
        N'BUOC_02_THONG_NHAT',
        N'Tiep nhan/Xet duyet dang ky',
        2,
        N'PheDuyet',
        1, 0, 1,
        N'DongYHoacKhongDongY',
        NULL,
        0,
        0,
        2,
        1,
        '40000000-0000-0000-0000-000000000013',
        N'VP UBND tiep nhan ho so do So Tai chinh gui len va cho y kien dong y/khong dong y.',
        NULL,
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '41000000-0000-0000-0000-000000000003',
        @WorkflowId,
        N'BUOC_03_SOAN_THAO',
        N'Soan thao van ban',
        3,
        N'SoanThao',
        1, 0, 1,
        N'HoanThanhDuThao',
        NULL,
        1,
        0,
        5,
        2,
        NULL,
        N'Don vi soan thao hoan thien du thao.',
        NULL,
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '41000000-0000-0000-0000-000000000004',
        @WorkflowId,
        N'BUOC_04_LAY_Y_KIEN',
        N'Lay y kien',
        4,
        N'LayYKien',
        1, 0, 0,
        N'GanKetQuaVaDinhKemFileDeHoanTat',
        1,
        1,
        0,
        7,
        2,
        NULL,
        N'Lay y kien va dinh kem file phan hoi.',
        NULL,
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '41000000-0000-0000-0000-000000000005',
        @WorkflowId,
        N'BUOC_05_THAM_DINH',
        N'Tham dinh/Danh gia',
        5,
        N'DanhGia',
        1, 0, 1,
        N'DanhGiaDatHoacTraLai',
        NULL,
        1,
        3,
        5,
        2,
        NULL,
        N'Cho phep tra lai toi da 3 lan.',
        NULL,
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '41000000-0000-0000-0000-000000000006',
        @WorkflowId,
        N'BUOC_06_TRINH_THAM_QUYEN',
        N'Trinh co quan co tham quyen',
        6,
        N'PheDuyet',
        1, 0, 0,
        N'TrinhPheDuyetHoSo',
        NULL,
        0,
        0,
        3,
        1,
        '40000000-0000-0000-0000-000000000002',
        N'So Tu phap tiep nhan tham muu ho so trinh co quan co tham quyen.',
        NULL,
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '41000000-0000-0000-0000-000000000007',
        @WorkflowId,
        N'BUOC_07_BAN_HANH',
        N'Ban hanh',
        7,
        N'BanHanh',
        1, 0, 0,
        N'CapNhatVanBanBanHanh',
        NULL,
        1,
        0,
        2,
        1,
        NULL,
        N'Hoan tat quy trinh va cap nhat van ban ban hanh.',
        NULL,
        @SeedUserId, @Now, @SeedUserId, @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucChuyenBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = @WorkflowId
)
BEGIN
    INSERT INTO DanhMucChuyenBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, TuBuocId, DenBuocId, DieuKienKetQua, LaNhanhMacDinh,
        MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    ('42000000-0000-0000-0000-000000000001', @WorkflowId, '41000000-0000-0000-0000-000000000001', '41000000-0000-0000-0000-000000000002', N'HOAN_THANH_DANG_KY', 1, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000002', @WorkflowId, '41000000-0000-0000-0000-000000000002', '41000000-0000-0000-0000-000000000003', N'DONG_Y', 1, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000003', @WorkflowId, '41000000-0000-0000-0000-000000000002', '41000000-0000-0000-0000-000000000001', N'KHONG_DONG_Y', 0, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000004', @WorkflowId, '41000000-0000-0000-0000-000000000003', '41000000-0000-0000-0000-000000000004', N'HOAN_THANH_DU_THAO', 1, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000005', @WorkflowId, '41000000-0000-0000-0000-000000000004', '41000000-0000-0000-0000-000000000005', N'DA_GAN_KET_QUA_Y_KIEN', 1, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000006', @WorkflowId, '41000000-0000-0000-0000-000000000005', '41000000-0000-0000-0000-000000000006', N'DAT', 1, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000007', @WorkflowId, '41000000-0000-0000-0000-000000000005', '41000000-0000-0000-0000-000000000003', N'KHONG_DAT_LAN_1', 0, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000008', @WorkflowId, '41000000-0000-0000-0000-000000000005', '41000000-0000-0000-0000-000000000003', N'KHONG_DAT_LAN_2', 0, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000009', @WorkflowId, '41000000-0000-0000-0000-000000000005', '41000000-0000-0000-0000-000000000003', N'KHONG_DAT_LAN_3', 0, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000010', @WorkflowId, '41000000-0000-0000-0000-000000000006', '41000000-0000-0000-0000-000000000007', N'TRINH_THANH_CONG', 1, NULL, NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('42000000-0000-0000-0000-000000000011', @WorkflowId, '41000000-0000-0000-0000-000000000006', '41000000-0000-0000-0000-000000000003', N'KHONG_DONG_Y', 0, N'Tra lai buoc soan thao de chinh sua, hoan thien truoc khi trinh lai.', NULL, @SeedUserId, @Now, @SeedUserId, @Now);
END;
