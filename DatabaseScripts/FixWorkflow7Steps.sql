SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @WorkflowId uniqueidentifier = (
        SELECT TOP 1 Id
        FROM DanhMucQuyTrinhSoanThaos
        WHERE MaQuyTrinh = N'QT_QPPL_7_BUOC'
    );

    IF @WorkflowId IS NULL
    BEGIN
        RAISERROR(N'Không tìm thấy quy trình QT_QPPL_7_BUOC.', 16, 1);
    END;

    DECLARE @Step01 uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_01_DANG_KY'
    );
    DECLARE @Step02 uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_02_THONG_NHAT'
    );
    DECLARE @Step03Extra uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_03_PHE_DUYET_DANG_KY'
    );
    DECLARE @Step03 uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_03_SOAN_THAO'
    );
    DECLARE @Step04 uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_04_LAY_Y_KIEN'
    );
    DECLARE @Step05 uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_05_THAM_DINH'
    );
    DECLARE @Step06 uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_06_TRINH_THAM_QUYEN'
    );
    DECLARE @Step08Extra uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_08_PHE_DUYET_VAN_BAN'
    );
    DECLARE @Step07 uniqueidentifier = (
        SELECT TOP 1 Id FROM DanhMucBuocQuyTrinhs
        WHERE QuyTrinhSoanThaoId = @WorkflowId AND MaBuoc = N'BUOC_07_BAN_HANH'
    );

    IF @Step01 IS NULL OR @Step02 IS NULL OR @Step03 IS NULL OR @Step04 IS NULL OR @Step05 IS NULL OR @Step06 IS NULL OR @Step07 IS NULL
    BEGIN
        RAISERROR(N'Quy trình chưa đủ các bước chuẩn để gộp về 7 bước.', 16, 1);
    END;

    UPDATE DanhMucBuocQuyTrinhs
    SET
        TenBuoc = CASE MaBuoc
            WHEN N'BUOC_01_DANG_KY' THEN N'Lập đề nghị/Đăng ký danh mục'
            WHEN N'BUOC_02_THONG_NHAT' THEN N'Văn bản thống nhất/đồng ý'
            WHEN N'BUOC_03_SOAN_THAO' THEN N'Soạn thảo văn bản'
            WHEN N'BUOC_04_LAY_Y_KIEN' THEN N'Lấy ý kiến'
            WHEN N'BUOC_05_THAM_DINH' THEN N'Thẩm định/Đánh giá'
            WHEN N'BUOC_06_TRINH_THAM_QUYEN' THEN N'Trình cơ quan có thẩm quyền'
            WHEN N'BUOC_07_BAN_HANH' THEN N'Ban hành'
            ELSE TenBuoc
        END,
        ThuTuSapXep = CASE MaBuoc
            WHEN N'BUOC_01_DANG_KY' THEN 1
            WHEN N'BUOC_02_THONG_NHAT' THEN 2
            WHEN N'BUOC_03_SOAN_THAO' THEN 3
            WHEN N'BUOC_04_LAY_Y_KIEN' THEN 4
            WHEN N'BUOC_05_THAM_DINH' THEN 5
            WHEN N'BUOC_06_TRINH_THAM_QUYEN' THEN 6
            WHEN N'BUOC_07_BAN_HANH' THEN 7
            ELSE ThuTuSapXep
        END
    WHERE QuyTrinhSoanThaoId = @WorkflowId;

    IF @Step03Extra IS NOT NULL
    BEGIN
        UPDATE HoSoVanBans
        SET BuocHienTaiId = @Step02
        WHERE BuocHienTaiId = @Step03Extra;

        UPDATE HoSoVanBanXuLys
        SET BuocQuyTrinhId = @Step02
        WHERE BuocQuyTrinhId = @Step03Extra;

        UPDATE HoSoVanBanDanhGias
        SET BuocQuyTrinhId = @Step02
        WHERE BuocQuyTrinhId = @Step03Extra;

        UPDATE HoSoVanBanDanhGias
        SET TraLaiBuocId = @Step02
        WHERE TraLaiBuocId = @Step03Extra;

        UPDATE HoSoVanBanLayYKiens
        SET BuocQuyTrinhId = @Step02
        WHERE BuocQuyTrinhId = @Step03Extra;

        UPDATE target
        SET
            target.SoNgayXuLy = COALESCE(target.SoNgayXuLy, source.SoNgayXuLy),
            target.SoNgayCanhBaoSapHan = COALESCE(target.SoNgayCanhBaoSapHan, source.SoNgayCanhBaoSapHan),
            target.GhiChu = COALESCE(NULLIF(target.GhiChu, N''), source.GhiChu)
        FROM HoSoVanBanBuocThoiHans target
        INNER JOIN HoSoVanBanBuocThoiHans source
            ON source.HoSoVanBanId = target.HoSoVanBanId
           AND source.BuocQuyTrinhId = @Step03Extra
           AND target.BuocQuyTrinhId = @Step02;

        UPDATE HoSoVanBanBuocThoiHans
        SET BuocQuyTrinhId = @Step02
        WHERE BuocQuyTrinhId = @Step03Extra
          AND HoSoVanBanId NOT IN (
              SELECT HoSoVanBanId
              FROM HoSoVanBanBuocThoiHans
              WHERE BuocQuyTrinhId = @Step02
          );

        DELETE source
        FROM HoSoVanBanBuocThoiHans source
        INNER JOIN HoSoVanBanBuocThoiHans target
            ON target.HoSoVanBanId = source.HoSoVanBanId
           AND target.BuocQuyTrinhId = @Step02
        WHERE source.BuocQuyTrinhId = @Step03Extra;
    END;

    IF @Step08Extra IS NOT NULL
    BEGIN
        UPDATE HoSoVanBans
        SET BuocHienTaiId = @Step06
        WHERE BuocHienTaiId = @Step08Extra;

        UPDATE HoSoVanBanXuLys
        SET BuocQuyTrinhId = @Step06
        WHERE BuocQuyTrinhId = @Step08Extra;

        UPDATE HoSoVanBanDanhGias
        SET BuocQuyTrinhId = @Step06
        WHERE BuocQuyTrinhId = @Step08Extra;

        UPDATE HoSoVanBanDanhGias
        SET TraLaiBuocId = @Step06
        WHERE TraLaiBuocId = @Step08Extra;

        UPDATE HoSoVanBanLayYKiens
        SET BuocQuyTrinhId = @Step06
        WHERE BuocQuyTrinhId = @Step08Extra;

        UPDATE target
        SET
            target.SoNgayXuLy = COALESCE(target.SoNgayXuLy, source.SoNgayXuLy),
            target.SoNgayCanhBaoSapHan = COALESCE(target.SoNgayCanhBaoSapHan, source.SoNgayCanhBaoSapHan),
            target.GhiChu = COALESCE(NULLIF(target.GhiChu, N''), source.GhiChu)
        FROM HoSoVanBanBuocThoiHans target
        INNER JOIN HoSoVanBanBuocThoiHans source
            ON source.HoSoVanBanId = target.HoSoVanBanId
           AND source.BuocQuyTrinhId = @Step08Extra
           AND target.BuocQuyTrinhId = @Step06;

        UPDATE HoSoVanBanBuocThoiHans
        SET BuocQuyTrinhId = @Step06
        WHERE BuocQuyTrinhId = @Step08Extra
          AND HoSoVanBanId NOT IN (
              SELECT HoSoVanBanId
              FROM HoSoVanBanBuocThoiHans
              WHERE BuocQuyTrinhId = @Step06
          );

        DELETE source
        FROM HoSoVanBanBuocThoiHans source
        INNER JOIN HoSoVanBanBuocThoiHans target
            ON target.HoSoVanBanId = source.HoSoVanBanId
           AND target.BuocQuyTrinhId = @Step06
        WHERE source.BuocQuyTrinhId = @Step08Extra;
    END;

    DELETE FROM DanhMucChuyenBuocQuyTrinhs
    WHERE QuyTrinhSoanThaoId = @WorkflowId;

    INSERT INTO DanhMucChuyenBuocQuyTrinhs
    (
        Id, QuyTrinhSoanThaoId, TuBuocId, DenBuocId, DieuKienKetQua, LaNhanhMacDinh,
        MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    )
    VALUES
    (NEWID(), @WorkflowId, @Step01, @Step02, N'HOAN_THANH_DANG_KY', 1, N'Chuyển hồ sơ đăng ký sang bước thống nhất/đồng ý.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step02, @Step03, N'DONG_Y', 1, N'Đồng ý để đơn vị soạn thảo tiếp tục xây dựng văn bản.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step02, @Step01, N'KHONG_DONG_Y', 0, N'Trả lại hồ sơ đăng ký cho đơn vị khởi tạo.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step03, @Step04, N'HOAN_THANH_DU_THAO', 1, N'Hoàn thành dự thảo và chuyển sang bước lấy ý kiến.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step04, @Step05, N'DA_GAN_KET_QUA_Y_KIEN', 1, N'Đã gắn kết quả lấy ý kiến và chuyển sang thẩm định.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step05, @Step06, N'DAT', 1, N'Thẩm định đạt và chuyển sang trình cơ quan có thẩm quyền.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step05, @Step03, N'KHONG_DAT_LAN_1', 0, N'Trả lại đơn vị soạn thảo lần 1.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step05, @Step03, N'KHONG_DAT_LAN_2', 0, N'Trả lại đơn vị soạn thảo lần 2.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step05, @Step03, N'KHONG_DAT_LAN_3', 0, N'Trả lại đơn vị soạn thảo lần 3.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE()),
    (NEWID(), @WorkflowId, @Step06, @Step07, N'TRINH_THANH_CONG', 1, N'Trình thành công và chuyển sang ban hành.', NULL, '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE(), '1FC5A8A6-6390-4788-BE90-A82098ED0134', GETDATE());

    IF @Step03Extra IS NOT NULL
    BEGIN
        DELETE FROM DanhMucBuocQuyTrinhs WHERE Id = @Step03Extra;
    END;

    IF @Step08Extra IS NOT NULL
    BEGIN
        DELETE FROM DanhMucBuocQuyTrinhs WHERE Id = @Step08Extra;
    END;

    COMMIT TRANSACTION;
    PRINT N'Đã chuẩn hóa quy trình QT_QPPL_7_BUOC về 7 bước.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
