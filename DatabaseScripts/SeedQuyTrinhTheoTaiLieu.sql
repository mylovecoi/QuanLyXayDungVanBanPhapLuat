SET NOCOUNT ON;

DECLARE @Now DATETIME = GETDATE();
DECLARE @SeedUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';

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
