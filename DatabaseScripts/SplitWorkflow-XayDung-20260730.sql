SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Now DATETIME = GETDATE();
    DECLARE @SeedUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';

    DECLARE @WorkflowLegacyId UNIQUEIDENTIFIER = (
        SELECT TOP 1 Id
        FROM DanhMucQuyTrinhSoanThaos
        WHERE MaQuyTrinh = N'QT_QPPL_7_BUOC'
    );

    IF @WorkflowLegacyId IS NOT NULL
    BEGIN
        UPDATE DanhMucQuyTrinhSoanThaos
        SET TrangThai = 0,
            GhiChu = N'Workflow cũ đã ngừng áp dụng cho nghiệp vụ xây dựng từ ngày 30/07/2026.'
        WHERE Id = @WorkflowLegacyId;
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

    MERGE DanhMucQuyTrinhSoanThaos AS target
    USING
    (
        SELECT
            @WorkflowUbnd6BuocId AS Id,
            N'QT_QPPL_TINH_QD_UBND_6_BUOC' AS MaQuyTrinh,
            N'Quy trình xây dựng văn bản QPPL cấp tỉnh (Quyết định UBND)' AS TenQuyTrinh,
            N'XayDung' AS LoaiQuyTrinh,
            @LoaiVanBanQuyetDinhUbndId AS DanhMucVanBanId,
            CASE WHEN @LoaiVanBanQuyetDinhUbndId IS NULL THEN NULL ELSE CONVERT(nvarchar(36), @LoaiVanBanQuyetDinhUbndId) END AS DanhMucVanBanIds,
            N'Tỉnh' AS CapApDung,
            1 AS PhienBan,
            CAST(1 AS bit) AS TrangThai,
            N'Workflow xây dựng riêng cho nghiệp vụ hồ sơ soạn thảo cấp tỉnh đối với Quyết định UBND.' AS MoTa,
            N'Chuẩn hóa thành workflow xây dựng riêng ngày 30/07/2026.' AS GhiChu

        UNION ALL

        SELECT
            @WorkflowHdnd7BuocId,
            N'QT_QPPL_TINH_NQ_HDND_7_BUOC',
            N'Quy trình xây dựng văn bản QPPL cấp tỉnh (Nghị quyết HĐND)',
            N'XayDung',
            @LoaiVanBanNghiQuyetHdndId,
            CASE WHEN @LoaiVanBanNghiQuyetHdndId IS NULL THEN NULL ELSE CONVERT(nvarchar(36), @LoaiVanBanNghiQuyetHdndId) END,
            N'Tỉnh',
            1,
            CAST(1 AS bit),
            N'Workflow xây dựng riêng cho nghiệp vụ hồ sơ soạn thảo cấp tỉnh đối với Nghị quyết HĐND.',
            N'Chuẩn hóa thành workflow xây dựng riêng ngày 30/07/2026.'
    ) AS source
    ON target.Id = source.Id
    WHEN MATCHED THEN
        UPDATE SET
            target.MaQuyTrinh = source.MaQuyTrinh,
            target.TenQuyTrinh = source.TenQuyTrinh,
            target.LoaiQuyTrinh = source.LoaiQuyTrinh,
            target.DanhMucVanBanId = source.DanhMucVanBanId,
            target.DanhMucVanBanIds = source.DanhMucVanBanIds,
            target.CapApDung = source.CapApDung,
            target.PhienBan = source.PhienBan,
            target.TrangThai = source.TrangThai,
            target.MoTa = source.MoTa,
            target.GhiChu = source.GhiChu,
            target.UpdatedBy = @SeedUserId,
            target.UpdatedDate = @Now
    WHEN NOT MATCHED THEN
        INSERT
        (
            Id, MaQuyTrinh, TenQuyTrinh, LoaiQuyTrinh, DanhMucVanBanId, DanhMucVanBanIds,
            CapApDung, PhienBan, TrangThai, MoTa, GhiChu,
            CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
        )
        VALUES
        (
            source.Id, source.MaQuyTrinh, source.TenQuyTrinh, source.LoaiQuyTrinh, source.DanhMucVanBanId, source.DanhMucVanBanIds,
            source.CapApDung, source.PhienBan, source.TrangThai, source.MoTa, source.GhiChu,
            @SeedUserId, @Now, @SeedUserId, @Now
        );

    MERGE DanhMucBuocQuyTrinhs AS target
    USING
    (
        SELECT * FROM
        (
            VALUES
            (CAST('52100000-0000-0000-0000-000000000001' AS uniqueidentifier), @WorkflowUbnd6BuocId, N'BUOC_01_SOAN_THAO_GUI_GOP_Y', N'Tổ chức soạn thảo và gửi lấy ý kiến góp ý', 1, N'SoanThao', 1, 0, 0, N'CapNhatHoSoDuThaoVaGuiLayYKien', NULL, 1, 0, NULL, NULL, NULL, N'Cập nhật hồ sơ dự thảo văn bản QPPL và chuyển lấy góp ý.', N'Bước khởi tạo riêng của workflow xây dựng.'),
            (CAST('52100000-0000-0000-0000-000000000002' AS uniqueidentifier), @WorkflowUbnd6BuocId, N'BUOC_02_GUI_THAM_DINH', N'Gửi thẩm định', 2, N'XuLy', 1, 0, 0, N'GuiThamDinh', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000002' AS uniqueidentifier), N'Đơn vị soạn thảo gửi hồ sơ đến Sở Tư pháp thẩm định.', N'Đính kèm công văn và hồ sơ gửi thẩm định.'),
            (CAST('52100000-0000-0000-0000-000000000003' AS uniqueidentifier), @WorkflowUbnd6BuocId, N'BUOC_03_THAM_DINH_VAN_BAN', N'Thẩm định văn bản', 3, N'DanhGia', 1, 0, 0, N'CapNhatVanBanThamDinh', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000002' AS uniqueidentifier), N'Sở Tư pháp cập nhật kết quả thẩm định văn bản.', N'Cho phép đính kèm văn bản thẩm định.'),
            (CAST('52100000-0000-0000-0000-000000000004' AS uniqueidentifier), @WorkflowUbnd6BuocId, N'BUOC_04_TRINH_HO_SO_XAY_DUNG', N'Trình hồ sơ xây dựng văn bản', 4, N'XuLy', 1, 0, 0, N'TrinhHoSoXayDungVanBan', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000013' AS uniqueidentifier), N'Cập nhật hồ sơ hoàn thiện để trình UBND tỉnh.', N'Đính kèm bộ hồ sơ trình.'),
            (CAST('52100000-0000-0000-0000-000000000005' AS uniqueidentifier), @WorkflowUbnd6BuocId, N'BUOC_05_LAY_Y_KIEN_THANH_VIEN_UBND', N'Lấy ý kiến thành viên UBND tỉnh', 5, N'LayYKien', 1, 0, 0, N'LayYKienThanhVienUbnd', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000013' AS uniqueidentifier), N'Theo dõi việc lấy ý kiến thành viên UBND tỉnh.', N'Có thể tổng hợp và cập nhật kết quả trên phần mềm.'),
            (CAST('52100000-0000-0000-0000-000000000006' AS uniqueidentifier), @WorkflowUbnd6BuocId, N'BUOC_06_THONG_QUA_BAN_HANH', N'Dự thảo được thông qua và ban hành', 6, N'BanHanh', 1, 0, 0, N'CapNhatVanBanDaKyBanHanh', NULL, 1, 0, NULL, NULL, NULL, N'Cập nhật văn bản đã được ký ban hành.', N'Bước kết thúc của workflow xây dựng UBND.'),

            (CAST('53100000-0000-0000-0000-000000000001' AS uniqueidentifier), @WorkflowHdnd7BuocId, N'BUOC_01_SOAN_THAO_GUI_GOP_Y', N'Tổ chức soạn thảo và gửi lấy ý kiến góp ý', 1, N'SoanThao', 1, 0, 0, N'CapNhatHoSoDuThaoVaGuiLayYKien', NULL, 1, 0, NULL, NULL, NULL, N'Cập nhật hồ sơ dự thảo văn bản QPPL và chuyển lấy góp ý.', N'Bước khởi tạo riêng của workflow xây dựng.'),
            (CAST('53100000-0000-0000-0000-000000000002' AS uniqueidentifier), @WorkflowHdnd7BuocId, N'BUOC_02_GUI_THAM_DINH', N'Gửi thẩm định', 2, N'XuLy', 1, 0, 0, N'GuiThamDinh', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000002' AS uniqueidentifier), N'Đơn vị soạn thảo gửi hồ sơ đến Sở Tư pháp thẩm định.', N'Đính kèm công văn và hồ sơ gửi thẩm định.'),
            (CAST('53100000-0000-0000-0000-000000000003' AS uniqueidentifier), @WorkflowHdnd7BuocId, N'BUOC_03_THAM_DINH_VAN_BAN', N'Thẩm định văn bản', 3, N'DanhGia', 1, 0, 0, N'CapNhatVanBanThamDinh', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000002' AS uniqueidentifier), N'Sở Tư pháp cập nhật kết quả thẩm định văn bản.', N'Cho phép đính kèm văn bản thẩm định.'),
            (CAST('53100000-0000-0000-0000-000000000004' AS uniqueidentifier), @WorkflowHdnd7BuocId, N'BUOC_04_TRINH_HO_SO_XAY_DUNG', N'Trình hồ sơ xây dựng văn bản', 4, N'XuLy', 1, 0, 0, N'TrinhHoSoXayDungVanBan', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000014' AS uniqueidentifier), N'Cập nhật hồ sơ hoàn thiện để trình HĐND tỉnh.', N'Đính kèm bộ hồ sơ trình.'),
            (CAST('53100000-0000-0000-0000-000000000005' AS uniqueidentifier), @WorkflowHdnd7BuocId, N'BUOC_05_LAY_Y_KIEN_THANH_VIEN_UBND', N'Lấy ý kiến thành viên UBND tỉnh', 5, N'LayYKien', 1, 0, 0, N'LayYKienThanhVienUbnd', NULL, 1, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000013' AS uniqueidentifier), N'Theo dõi việc lấy ý kiến thành viên UBND tỉnh.', N'Có thể tổng hợp và cập nhật kết quả trên phần mềm.'),
            (CAST('53100000-0000-0000-0000-000000000006' AS uniqueidentifier), @WorkflowHdnd7BuocId, N'BUOC_06_TRINH_HDND_HOP', N'Hồ sơ dự thảo trình HĐND họp', 6, N'PheDuyet', 1, 0, 0, N'CapNhatYKienDaiBieuHopHdnd', NULL, 0, 0, NULL, NULL, CAST('40000000-0000-0000-0000-000000000014' AS uniqueidentifier), N'Cập nhật ý kiến của đại biểu tại kỳ họp HĐND.', N'Bước đặc thù áp dụng cho hồ sơ trình HĐND họp.'),
            (CAST('53100000-0000-0000-0000-000000000007' AS uniqueidentifier), @WorkflowHdnd7BuocId, N'BUOC_07_THONG_QUA_BAN_HANH', N'Dự thảo được thông qua và ban hành', 7, N'BanHanh', 1, 0, 0, N'CapNhatVanBanDaKyBanHanh', NULL, 1, 0, NULL, NULL, NULL, N'Cập nhật văn bản đã được ký ban hành.', N'Bước kết thúc của workflow xây dựng HĐND.')
        ) AS S(Id, QuyTrinhSoanThaoId, MaBuoc, TenBuoc, ThuTuSapXep, LoaiBuoc, BatBuoc, ChoPhepBoQua, ChoPhepQuayLui, CachHoanThanh, SoLuongPhanHoiToiThieu, YeuCauFileDinhKem, SoLanTraLaiToiDa, SoNgayXuLyTieuChuan, SoNgayCanhBaoSapHan, DonViTiepNhanMacDinhId, MoTa, GhiChu)
    ) AS source
    ON target.Id = source.Id
    WHEN MATCHED THEN
        UPDATE SET
            target.QuyTrinhSoanThaoId = source.QuyTrinhSoanThaoId,
            target.MaBuoc = source.MaBuoc,
            target.TenBuoc = source.TenBuoc,
            target.ThuTuSapXep = source.ThuTuSapXep,
            target.LoaiBuoc = source.LoaiBuoc,
            target.BatBuoc = source.BatBuoc,
            target.ChoPhepBoQua = source.ChoPhepBoQua,
            target.ChoPhepQuayLui = source.ChoPhepQuayLui,
            target.CachHoanThanh = source.CachHoanThanh,
            target.SoLuongPhanHoiToiThieu = source.SoLuongPhanHoiToiThieu,
            target.YeuCauFileDinhKem = source.YeuCauFileDinhKem,
            target.SoLanTraLaiToiDa = source.SoLanTraLaiToiDa,
            target.SoNgayXuLyTieuChuan = source.SoNgayXuLyTieuChuan,
            target.SoNgayCanhBaoSapHan = source.SoNgayCanhBaoSapHan,
            target.DonViTiepNhanMacDinhId = source.DonViTiepNhanMacDinhId,
            target.MoTa = source.MoTa,
            target.GhiChu = source.GhiChu,
            target.UpdatedBy = @SeedUserId,
            target.UpdatedDate = @Now
    WHEN NOT MATCHED THEN
        INSERT
        (
            Id, QuyTrinhSoanThaoId, MaBuoc, TenBuoc, ThuTuSapXep, LoaiBuoc,
            BatBuoc, ChoPhepBoQua, ChoPhepQuayLui, CachHoanThanh, SoLuongPhanHoiToiThieu,
            YeuCauFileDinhKem, SoLanTraLaiToiDa, SoNgayXuLyTieuChuan, SoNgayCanhBaoSapHan, DonViTiepNhanMacDinhId,
            MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
        )
        VALUES
        (
            source.Id, source.QuyTrinhSoanThaoId, source.MaBuoc, source.TenBuoc, source.ThuTuSapXep, source.LoaiBuoc,
            source.BatBuoc, source.ChoPhepBoQua, source.ChoPhepQuayLui, source.CachHoanThanh, source.SoLuongPhanHoiToiThieu,
            source.YeuCauFileDinhKem, source.SoLanTraLaiToiDa, source.SoNgayXuLyTieuChuan, source.SoNgayCanhBaoSapHan, source.DonViTiepNhanMacDinhId,
            source.MoTa, source.GhiChu, @SeedUserId, @Now, @SeedUserId, @Now
        );

    DELETE FROM DanhMucChuyenBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId IN (@WorkflowUbnd6BuocId, @WorkflowHdnd7BuocId);

    INSERT INTO DanhMucChuyenBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, TuBuocId, DenBuocId, DieuKienKetQua, LaNhanhMacDinh,
        MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    ('52200000-0000-0000-0000-000000000001', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000001', '52100000-0000-0000-0000-000000000002', N'GUI_LAY_Y_KIEN', 1, N'Hoàn thành bước soạn thảo và chuyển thẩm định.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000002', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000002', '52100000-0000-0000-0000-000000000003', N'GUI_THAM_DINH', 1, N'Chuyển hồ sơ sang bước thẩm định.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000003', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000003', '52100000-0000-0000-0000-000000000004', N'THAM_DINH_XONG', 1, N'Hoàn thành thẩm định để trình hồ sơ xây dựng.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000004', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000004', '52100000-0000-0000-0000-000000000005', N'TRINH_HO_SO_XONG', 1, N'Trình hồ sơ xong để lấy ý kiến thành viên UBND tỉnh.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('52200000-0000-0000-0000-000000000005', @WorkflowUbnd6BuocId, '52100000-0000-0000-0000-000000000005', '52100000-0000-0000-0000-000000000006', N'LAY_Y_KIEN_UBND_XONG', 1, N'Hoàn tất lấy ý kiến để cập nhật văn bản ban hành.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),

    ('53200000-0000-0000-0000-000000000001', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000001', '53100000-0000-0000-0000-000000000002', N'GUI_LAY_Y_KIEN', 1, N'Hoàn thành bước soạn thảo và chuyển thẩm định.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000002', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000002', '53100000-0000-0000-0000-000000000003', N'GUI_THAM_DINH', 1, N'Chuyển hồ sơ sang bước thẩm định.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000003', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000003', '53100000-0000-0000-0000-000000000004', N'THAM_DINH_XONG', 1, N'Hoàn thành thẩm định để trình hồ sơ xây dựng.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000004', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000004', '53100000-0000-0000-0000-000000000005', N'TRINH_HO_SO_XONG', 1, N'Trình hồ sơ xong để lấy ý kiến thành viên UBND tỉnh.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000005', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000005', '53100000-0000-0000-0000-000000000006', N'LAY_Y_KIEN_UBND_XONG', 1, N'Hoàn tất lấy ý kiến để chuyển bước họp HĐND.', NULL, @SeedUserId, @Now, @SeedUserId, @Now),
    ('53200000-0000-0000-0000-000000000006', @WorkflowHdnd7BuocId, '53100000-0000-0000-0000-000000000006', '53100000-0000-0000-0000-000000000007', N'TRINH_HDND_XONG', 1, N'Hoàn tất bước họp HĐND để cập nhật văn bản ban hành.', NULL, @SeedUserId, @Now, @SeedUserId, @Now);

    COMMIT TRANSACTION;
    PRINT N'Đã tách và chuẩn hóa workflow Xây dựng riêng cho nghiệp vụ xây dựng văn bản.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
