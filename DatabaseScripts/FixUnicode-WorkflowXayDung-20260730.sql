SET NOCOUNT ON;

UPDATE DanhMucQuyTrinhSoanThaos
SET
    TenQuyTrinh = N'Quy trình xây dựng văn bản QPPL cấp tỉnh (Quyết định UBND)',
    MoTa = N'Workflow xây dựng riêng cho nghiệp vụ hồ sơ soạn thảo cấp tỉnh đối với Quyết định UBND.',
    GhiChu = N'Chuẩn hóa thành workflow xây dựng riêng ngày 30/07/2026.',
    UpdatedDate = GETDATE()
WHERE MaQuyTrinh = N'QT_QPPL_TINH_QD_UBND_6_BUOC';

UPDATE DanhMucQuyTrinhSoanThaos
SET
    TenQuyTrinh = N'Quy trình xây dựng văn bản QPPL cấp tỉnh (Nghị quyết HĐND)',
    MoTa = N'Workflow xây dựng riêng cho nghiệp vụ hồ sơ soạn thảo cấp tỉnh đối với Nghị quyết HĐND.',
    GhiChu = N'Chuẩn hóa thành workflow xây dựng riêng ngày 30/07/2026.',
    UpdatedDate = GETDATE()
WHERE MaQuyTrinh = N'QT_QPPL_TINH_NQ_HDND_7_BUOC';
