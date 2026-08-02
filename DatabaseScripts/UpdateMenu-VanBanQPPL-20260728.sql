DECLARE @Now DATETIME = GETDATE();
DECLARE @SystemUser UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000020')
BEGIN
    UPDATE RoleActions
    SET Title = N'Xây dựng văn bản QPPL',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000020';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000021')
BEGIN
    UPDATE RoleActions
    SET Title = N'Đăng ký văn bản',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000021';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000022')
BEGIN
    UPDATE RoleActions
    SET Title = N'Đăng ký văn bản',
        Controller = 'DangKyVanBan',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000022';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000023')
BEGIN
    UPDATE RoleActions
    SET Title = N'Xét duyệt đăng ký',
        Controller = 'XetDuyetDangKy',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000023';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000024')
BEGIN
    UPDATE RoleActions
    SET Title = N'Phê duyệt đăng ký',
        Controller = 'PheDuyetDangKy',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000024';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000025')
BEGIN
    UPDATE RoleActions
    SET Title = N'Xây dựng văn bản',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000025';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000026')
BEGIN
    UPDATE RoleActions
    SET Role = 'VanBanQPPL.XayDungVanBan.SoanThaoVanBan',
        Title = N'Soạn thảo văn bản',
        Controller = 'XetDuyetVanBan',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000026';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000027')
BEGIN
    UPDATE RoleActions
    SET Role = 'VanBanQPPL.XayDungVanBan.LayYKienUBND',
        Title = N'Lấy ý kiến UBND',
        Controller = 'LayYKienUBND',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000027';
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000051')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000051', @SystemUser, @Now, @SystemUser, @Now,
        4, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.TraCuuDangKyVanBan', '20000000-0000-0000-0000-000000000021',
        N'Tra cứu đăng ký văn bản', 'TraCuuDangKyVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kich hoat', NULL, NULL
    );
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000051')
BEGIN
    UPDATE RoleActions
    SET Title = N'Tra cứu đăng ký văn bản',
        Controller = 'TraCuuDangKyVanBan',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000051';
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000047')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STT, [Level], [Type], Role, ParentId,
        Title, Controller, Action, [Data], [Table],
        Status, Description, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000047', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.LayYKienHDND', '20000000-0000-0000-0000-000000000025',
        N'Lấy ý kiến HĐND', 'LayYKienHDND', 'Index', NULL, 'HoSoVanBans',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000049')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000049', @SystemUser, @Now, @SystemUser, @Now,
        5, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.ChamDiemXayDung', '20000000-0000-0000-0000-000000000025',
        N'Chấm điểm xây dựng', 'ChamDiemXayDung', 'Index', NULL, 'HoSoVanBanChamDiems',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000050')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000050', @SystemUser, @Now, @SystemUser, @Now,
        6, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.TheoDoiTienDoXayDung', '20000000-0000-0000-0000-000000000025',
        N'Theo dõi tiến độ xây dựng', 'TheoDoiTienDoXayDung', 'Index', NULL, 'HoSoVanBans',
        N'Kích hoạt', NULL, NULL
    );
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000028')
BEGIN
    UPDATE RoleActions
    SET Role = 'VanBanQPPL.XayDungVanBan.DuThaoVanBan',
        Title = N'Xét duyệt soạn thảo',
        Controller = 'DuThaoVanBan',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000028';
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000043')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000043', @SystemUser, @Now, @SystemUser, @Now,
        4, 'Group', 2, 'VanBanQPPL.XayDungVanBan.DuThaoVanBan', '20000000-0000-0000-0000-000000000025',
        N'Dự thảo văn bản', '', '', NULL, '',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000044')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000044', @SystemUser, @Now, @SystemUser, @Now,
        1, 'Detail', 3, 'VanBanQPPL.XayDungVanBan.DuThaoVanBan.DanhSachVanBanDuThao', '20000000-0000-0000-0000-000000000043',
        N'Danh sách văn bản dự thảo', 'DuThaoVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000045')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000045', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Detail', 3, 'VanBanQPPL.XayDungVanBan.DuThaoVanBan.XetDuyetDuThao', '20000000-0000-0000-0000-000000000043',
        N'Xét duyệt dự thảo', 'XetDuyetDuThao', 'Index', NULL, 'HoSoVanBans',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000046')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000046', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Detail', 3, 'VanBanQPPL.XayDungVanBan.DuThaoVanBan.PheDuyetDuThao', '20000000-0000-0000-0000-000000000043',
        N'Phê duyệt dự thảo', 'PheDuyetDuThao', 'Index', NULL, 'HoSoVanBans',
        N'Kích hoạt', NULL, NULL
    );
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000029')
BEGIN
    UPDATE RoleActions
    SET Role = 'VanBanQPPL.DanhGiaVanBan',
        PhanLoai = 'Group',
        Level = 1,
        STTSapXep = 3,
        RoleGroupId = '20000000-0000-0000-0000-000000000020',
        Title = N'Đánh giá văn bản',
        Controller = '',
        Action = '',
        [Table] = '',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000029';
END;

IF EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000030')
BEGIN
    UPDATE RoleActions
    SET Role = 'VanBanQPPL.DanhGiaVanBan.DanhSachVanBan',
        PhanLoai = 'Detail',
        Level = 2,
        STTSapXep = 1,
        RoleGroupId = '20000000-0000-0000-0000-000000000029',
        Title = N'Danh sách văn bản',
        Controller = 'DanhGiaVanBan',
        Action = 'Index',
        [Table] = 'HoSoVanBans',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000030';
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000035')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000035', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Detail', 2, 'VanBanQPPL.DanhGiaVanBan.XetDuyetVanBan', '20000000-0000-0000-0000-000000000029',
        N'Xét duyệt văn bản', 'XetDuyetVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000036')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000036', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Detail', 2, 'VanBanQPPL.DanhGiaVanBan.PheDuyetVanBan', '20000000-0000-0000-0000-000000000029',
        N'Phê duyệt văn bản', 'PheDuyetVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000037')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000037', @SystemUser, @Now, @SystemUser, @Now,
        4, 'Group', 1, 'VanBanQPPL.BanHanhVanBan', '20000000-0000-0000-0000-000000000020',
        N'Ban hành văn bản', '', '', NULL, '',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000038')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000038', @SystemUser, @Now, @SystemUser, @Now,
        1, 'Detail', 2, 'VanBanQPPL.BanHanhVanBan.DanhSachVanBan', '20000000-0000-0000-0000-000000000037',
        N'Danh sách văn bản', 'VanBanPhapLuat', 'Index', NULL, 'VanBanPhapLuat',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000039')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000039', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Detail', 2, 'VanBanQPPL.BanHanhVanBan.XetDuyetVanBan', '20000000-0000-0000-0000-000000000037',
        N'Xét duyệt văn bản', 'DangPhatTrien', 'XetDuyetBanHanhVanBan', NULL, 'DangPhatTrien',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000040')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000040', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Detail', 2, 'VanBanQPPL.BanHanhVanBan.BanHanhVanBan', '20000000-0000-0000-0000-000000000037',
        N'Ban hành văn bản', 'DangPhatTrien', 'BanHanhVanBan', NULL, 'DangPhatTrien',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000041')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000041', @SystemUser, @Now, @SystemUser, @Now,
        5, 'Group', 1, 'VanBanQPPL.GiaHanThoiGianXayDung', '20000000-0000-0000-0000-000000000020',
        N'Gia hạn thời gian xây dựng', '', '', NULL, '',
        N'Kích hoạt', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000042')
BEGIN
    INSERT INTO RoleActions
    (
        Id, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
        STTSapXep, PhanLoai, Level, Role, RoleGroupId,
        Title, Controller, Action, Parameter, [Table],
        Status, UseGroup, Icon
    )
    VALUES
    (
        '20000000-0000-0000-0000-000000000042', @SystemUser, @Now, @SystemUser, @Now,
        1, 'Detail', 2, 'VanBanQPPL.GiaHanThoiGianXayDung.DanhSachVanBan', '20000000-0000-0000-0000-000000000041',
        N'Danh sách văn bản', 'DangPhatTrien', 'DanhSachGiaHanXayDung', NULL, 'DangPhatTrien',
        N'Kích hoạt', NULL, NULL
    );
END;

UPDATE RoleActions
SET Controller = 'GiaHanXayDung',
    Action = 'Index',
    [Table] = 'HoSoVanBans',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000042';
