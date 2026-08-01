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

DECLARE @WorkflowUbnd6BuocId UNIQUEIDENTIFIER = '52000000-0000-0000-0000-000000000001';
DECLARE @WorkflowHdnd7BuocId UNIQUEIDENTIFIER = '53000000-0000-0000-0000-000000000001';
DECLARE @LoaiVanBanQuyetDinhUbndId UNIQUEIDENTIFIER =
(
    SELECT TOP 1 Id
    FROM DanhMucVanBans
    WHERE CapChinhQuyen = N'Tỉnh'
      AND ChuTheBanHanh = N'UBND'
    ORDER BY ThuTuSapXep, TenLoaiVanBan
);
DECLARE @LoaiVanBanNghiQuyetHdndId UNIQUEIDENTIFIER =
(
    SELECT TOP 1 Id
    FROM DanhMucVanBans
    WHERE CapChinhQuyen = N'Tỉnh'
      AND ChuTheBanHanh = N'HĐND'
    ORDER BY ThuTuSapXep, TenLoaiVanBan
);

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucQuyTrinhSoanThaos
    WHERE Id = @WorkflowUbnd6BuocId
)
BEGIN
    INSERT INTO DanhMucQuyTrinhSoanThaos
    (
        Id, MaQuyTrinh, TenQuyTrinh, DanhMucVanBanId, DanhMucVanBanIds, CapApDung, PhienBan, TrangThai,
        MoTa, GhiChu,
        CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    (
        @WorkflowUbnd6BuocId,
        N'QT_QPPL_TINH_QD_UBND_6_BUOC',
        N'Quy trình xây dựng văn bản QPPL cấp tỉnh (Quyết định UBND)',
        @LoaiVanBanQuyetDinhUbndId,
        CASE WHEN @LoaiVanBanQuyetDinhUbndId IS NULL THEN NULL ELSE CONVERT(nvarchar(36), @LoaiVanBanQuyetDinhUbndId) END,
        N'Tỉnh',
        1,
        1,
        N'Quy trình 6 bước theo tài liệu Quy trinh.docx cho nhóm văn bản Quyết định UBND cấp tỉnh.',
        N'Thêm từ tài liệu nghiệp vụ ngày 30/07/2026.',
        @SeedUserId,
        @Now,
        @SeedUserId,
        @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = @WorkflowUbnd6BuocId
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
        '52100000-0000-0000-0000-000000000001',
        @WorkflowUbnd6BuocId,
        N'BUOC_01_SOAN_THAO_GUI_GOP_Y',
        N'Tổ chức soạn thảo và gửi lấy ý kiến góp ý',
        1,
        N'SoanThao',
        1, 0, 0,
        N'CapNhatHoSoDuThaoVaGuiLayYKien',
        NULL,
        1,
        0,
        NULL,
        NULL,
        NULL,
        N'Cập nhật hồ sơ dự thảo văn bản QPPL gửi lấy ý kiến góp ý.',
        N'Đính kèm công văn và hồ sơ gửi lấy ý kiến trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '52100000-0000-0000-0000-000000000002',
        @WorkflowUbnd6BuocId,
        N'BUOC_02_GUI_THAM_DINH',
        N'Gửi thẩm định',
        2,
        N'XuLy',
        1, 0, 0,
        N'GuiThamDinh',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000002',
        N'Cập nhật hồ sơ dự thảo văn bản QPPL gửi thẩm định.',
        N'Đính kèm công văn và hồ sơ gửi thẩm định trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '52100000-0000-0000-0000-000000000003',
        @WorkflowUbnd6BuocId,
        N'BUOC_03_THAM_DINH_VAN_BAN',
        N'Thẩm định văn bản',
        3,
        N'DanhGia',
        1, 0, 0,
        N'CapNhatVanBanThamDinh',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000002',
        N'Sở Tư pháp cập nhật văn bản thẩm định.',
        N'Đính kèm văn bản thẩm định trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '52100000-0000-0000-0000-000000000004',
        @WorkflowUbnd6BuocId,
        N'BUOC_04_TRINH_HO_SO_XAY_DUNG',
        N'Trình hồ sơ xây dựng văn bản',
        4,
        N'XuLy',
        1, 0, 0,
        N'TrinhHoSoXayDungVanBan',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000013',
        N'Cập nhật hồ sơ dự thảo văn bản QPPL đã hoàn thiện để trình UBND tỉnh.',
        N'Đính kèm văn bản và hồ sơ dự thảo hoàn thiện để trình trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '52100000-0000-0000-0000-000000000005',
        @WorkflowUbnd6BuocId,
        N'BUOC_05_LAY_Y_KIEN_THANH_VIEN_UBND',
        N'Lấy ý kiến thành viên UBND tỉnh',
        5,
        N'LayYKien',
        1, 0, 0,
        N'LayYKienThanhVienUbnd',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000013',
        N'Cập nhật hồ sơ dự thảo văn bản QPPL được gửi lấy ý kiến thành viên UBND tỉnh.',
        N'Đính kèm văn bản và hồ sơ lấy ý kiến thành viên UBND tỉnh trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '52100000-0000-0000-0000-000000000006',
        @WorkflowUbnd6BuocId,
        N'BUOC_06_THONG_QUA_BAN_HANH',
        N'Dự thảo được thông qua và ban hành',
        6,
        N'BanHanh',
        1, 0, 0,
        N'CapNhatVanBanDaKyBanHanh',
        NULL,
        1,
        0,
        NULL,
        NULL,
        NULL,
        N'Cập nhật văn bản đã được ký ban hành.',
        N'Đính kèm văn bản đã được ký ban hành trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucChuyenBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = @WorkflowUbnd6BuocId
)
BEGIN
    INSERT INTO DanhMucChuyenBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, TuBuocId, DenBuocId, DieuKienKetQua, LaNhanhMacDinh,
        MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    ('52200000-0000-0000-0000-000000000001', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000001', '52100000-0000-0000-0000-000000000002', N'GUI_LAY_Y_KIEN', 1, N'Hoàn thành soạn thảo và gửi lấy ý kiến góp ý.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000002', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000002', '52100000-0000-0000-0000-000000000003', N'GUI_THAM_DINH', 1, N'Chuyển hồ sơ sang bước thẩm định.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000003', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000003', '52100000-0000-0000-0000-000000000004', N'THAM_DINH_XONG', 1, N'Hoàn thành thẩm định để trình hồ sơ xây dựng văn bản.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000004', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000004', '52100000-0000-0000-0000-000000000005', N'TRINH_HO_SO_XONG', 1, N'Trình hồ sơ xong để lấy ý kiến thành viên UBND tỉnh.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000005', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000005', '52100000-0000-0000-0000-000000000006', N'LAY_Y_KIEN_UBND_XONG', 1, N'Hoàn tất lấy ý kiến thành viên UBND tỉnh để cập nhật văn bản ban hành.', NULL, @SeedUserId, @Now, @SeedUserId, @Now);
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucQuyTrinhSoanThaos
    WHERE Id = @WorkflowHdnd7BuocId
)
BEGIN
    INSERT INTO DanhMucQuyTrinhSoanThaos
    (
        Id, MaQuyTrinh, TenQuyTrinh, DanhMucVanBanId, DanhMucVanBanIds, CapApDung, PhienBan, TrangThai,
        MoTa, GhiChu,
        CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    (
        @WorkflowHdnd7BuocId,
        N'QT_QPPL_TINH_NQ_HDND_7_BUOC',
        N'Quy trình xây dựng văn bản QPPL cấp tỉnh (Nghị quyết HĐND)',
        @LoaiVanBanNghiQuyetHdndId,
        CASE WHEN @LoaiVanBanNghiQuyetHdndId IS NULL THEN NULL ELSE CONVERT(nvarchar(36), @LoaiVanBanNghiQuyetHdndId) END,
        N'Tỉnh',
        1,
        1,
        N'Quy trình 7 bước theo tài liệu Quy trinh.docx cho nhóm văn bản Nghị quyết HĐND cấp tỉnh.',
        N'Thêm từ tài liệu nghiệp vụ ngày 30/07/2026.',
        @SeedUserId,
        @Now,
        @SeedUserId,
        @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = @WorkflowHdnd7BuocId
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
        '53100000-0000-0000-0000-000000000001',
        @WorkflowHdnd7BuocId,
        N'BUOC_01_SOAN_THAO_GUI_GOP_Y',
        N'Tổ chức soạn thảo và gửi lấy ý kiến góp ý',
        1,
        N'SoanThao',
        1, 0, 0,
        N'CapNhatHoSoDuThaoVaGuiLayYKien',
        NULL,
        1,
        0,
        NULL,
        NULL,
        NULL,
        N'Cập nhật hồ sơ dự thảo văn bản QPPL gửi lấy ý kiến góp ý.',
        N'Đính kèm công văn và hồ sơ gửi lấy ý kiến trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '53100000-0000-0000-0000-000000000002',
        @WorkflowHdnd7BuocId,
        N'BUOC_02_GUI_THAM_DINH',
        N'Gửi thẩm định',
        2,
        N'XuLy',
        1, 0, 0,
        N'GuiThamDinh',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000002',
        N'Cập nhật hồ sơ dự thảo văn bản QPPL gửi thẩm định.',
        N'Đính kèm công văn và hồ sơ gửi thẩm định trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '53100000-0000-0000-0000-000000000003',
        @WorkflowHdnd7BuocId,
        N'BUOC_03_THAM_DINH_VAN_BAN',
        N'Thẩm định văn bản',
        3,
        N'DanhGia',
        1, 0, 0,
        N'CapNhatVanBanThamDinh',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000002',
        N'Sở Tư pháp cập nhật văn bản thẩm định.',
        N'Đính kèm văn bản thẩm định trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '53100000-0000-0000-0000-000000000004',
        @WorkflowHdnd7BuocId,
        N'BUOC_04_TRINH_HO_SO_XAY_DUNG',
        N'Trình hồ sơ xây dựng văn bản',
        4,
        N'XuLy',
        1, 0, 0,
        N'TrinhHoSoXayDungVanBan',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000014',
        N'Cập nhật hồ sơ dự thảo văn bản QPPL đã hoàn thiện để trình HĐND tỉnh.',
        N'Đính kèm văn bản và hồ sơ dự thảo hoàn thiện để trình trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '53100000-0000-0000-0000-000000000005',
        @WorkflowHdnd7BuocId,
        N'BUOC_05_LAY_Y_KIEN_THANH_VIEN_UBND',
        N'Lấy ý kiến thành viên UBND tỉnh',
        5,
        N'LayYKien',
        1, 0, 0,
        N'LayYKienThanhVienUbnd',
        NULL,
        1,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000013',
        N'Cập nhật hồ sơ dự thảo văn bản QPPL được gửi lấy ý kiến thành viên UBND tỉnh.',
        N'Đính kèm văn bản và hồ sơ lấy ý kiến thành viên UBND tỉnh trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '53100000-0000-0000-0000-000000000006',
        @WorkflowHdnd7BuocId,
        N'BUOC_06_TRINH_HDND_HOP',
        N'Hồ sơ dự thảo trình HĐND họp',
        6,
        N'PheDuyet',
        1, 0, 0,
        N'CapNhatYKienDaiBieuHopHdnd',
        NULL,
        0,
        0,
        NULL,
        NULL,
        '40000000-0000-0000-0000-000000000014',
        N'Cập nhật ý kiến của các đại biểu trong cuộc họp HĐND.',
        N'Bước đặc thù áp dụng cho hồ sơ trình HĐND họp.',
        @SeedUserId, @Now, @SeedUserId, @Now
    ),
    (
        '53100000-0000-0000-0000-000000000007',
        @WorkflowHdnd7BuocId,
        N'BUOC_07_THONG_QUA_BAN_HANH',
        N'Dự thảo được thông qua và ban hành',
        7,
        N'BanHanh',
        1, 0, 0,
        N'CapNhatVanBanDaKyBanHanh',
        NULL,
        1,
        0,
        NULL,
        NULL,
        NULL,
        N'Cập nhật văn bản đã được ký ban hành.',
        N'Đính kèm văn bản đã được ký ban hành trên phần mềm.',
        @SeedUserId, @Now, @SeedUserId, @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM DanhMucChuyenBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = @WorkflowHdnd7BuocId
)
BEGIN
    INSERT INTO DanhMucChuyenBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, TuBuocId, DenBuocId, DieuKienKetQua, LaNhanhMacDinh,
        MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    ('53200000-0000-0000-0000-000000000001', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000001', '53100000-0000-0000-0000-000000000002', N'GUI_LAY_Y_KIEN', 1, N'Hoàn thành soạn thảo và gửi lấy ý kiến góp ý.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000002', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000002', '53100000-0000-0000-0000-000000000003', N'GUI_THAM_DINH', 1, N'Chuyển hồ sơ sang bước thẩm định.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000003', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000003', '53100000-0000-0000-0000-000000000004', N'THAM_DINH_XONG', 1, N'Hoàn thành thẩm định để trình hồ sơ xây dựng văn bản.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000004', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000004', '53100000-0000-0000-0000-000000000005', N'TRINH_HO_SO_XONG', 1, N'Trình hồ sơ xong để lấy ý kiến thành viên UBND tỉnh.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000005', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000005', '53100000-0000-0000-0000-000000000006', N'LAY_Y_KIEN_UBND_XONG', 1, N'Hoàn tất lấy ý kiến thành viên UBND tỉnh để chuyển trình HĐND họp.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000006', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000006', '53100000-0000-0000-0000-000000000007', N'TRINH_HDND_XONG', 1, N'Hoàn tất bước họp HĐND để cập nhật văn bản đã thông qua và ban hành.', NULL, @SeedUserId, @Now, @SeedUserId, @Now);
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
