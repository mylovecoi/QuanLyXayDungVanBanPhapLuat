DECLARE @Now DATETIME = GETDATE();
DECLARE @SystemUser UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';

IF EXISTS (
    SELECT 1
    FROM RoleActions
    WHERE Id = '20000000-0000-0000-0000-000000000016'
)
BEGIN
    UPDATE RoleActions
    SET Controller = 'QuyTrinhSoanThao',
        Action = 'Index',
        [Table] = 'DanhMucQuyTrinhSoanThaos',
        UpdatedBy = @SystemUser,
        UpdatedDate = @Now
    WHERE Id = '20000000-0000-0000-0000-000000000016';
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000020')
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
        '20000000-0000-0000-0000-000000000020', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Group', 0, 'VanBanQPPL', '00000000-0000-0000-0000-000000000000',
        N'Xây dựng văn bản QPPL', '', '', NULL, '',
        N'Kich hoat', NULL, 'fas fa-landmark'
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000021')
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
        '20000000-0000-0000-0000-000000000021', @SystemUser, @Now, @SystemUser, @Now,
        1, 'Group', 1, 'VanBanQPPL.DangKyXayDung', '20000000-0000-0000-0000-000000000020',
        N'Đăng ký văn bản', '', '', NULL, '',
        N'Kich hoat', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000022')
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
        '20000000-0000-0000-0000-000000000022', @SystemUser, @Now, @SystemUser, @Now,
        1, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.DanhSachDangKy', '20000000-0000-0000-0000-000000000021',
        N'Đăng ký văn bản', 'DangKyVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kich hoat', NULL, NULL
    );
END;

UPDATE RoleActions
SET Title = N'Đăng ký văn bản',
    Controller = 'DangKyVanBan',
    Action = 'Index',
    [Table] = 'HoSoVanBans',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000022';

UPDATE RoleActions
SET Title = N'Xây dựng văn bản QPPL',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000020';

UPDATE RoleActions
SET Title = N'Đăng ký văn bản',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000021';

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000023')
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
        '20000000-0000-0000-0000-000000000023', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.XetDuyetDangKy', '20000000-0000-0000-0000-000000000021',
        N'Xet duyet dang ky', 'XetDuyetDangKy', 'Index', NULL, 'HoSoVanBans',
        N'Kich hoat', NULL, NULL
    );
END;

UPDATE RoleActions
SET Title = N'Xet duyet dang ky',
    Controller = 'XetDuyetDangKy',
    Action = 'Index',
    [Table] = 'HoSoVanBans',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000023';

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000024')
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
        '20000000-0000-0000-0000-000000000024', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Detail', 2, 'VanBanQPPL.DangKyXayDung.PheDuyetDangKy', '20000000-0000-0000-0000-000000000021',
        N'Phe duyet dang ky', 'PheDuyetDangKy', 'Index', NULL, 'HoSoVanBans',
        N'Kich hoat', NULL, NULL
    );
END;

UPDATE RoleActions
SET Title = N'Phe duyet dang ky',
    Controller = 'PheDuyetDangKy',
    Action = 'Index',
    [Table] = 'HoSoVanBans',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000024';

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000025')
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
        '20000000-0000-0000-0000-000000000025', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Group', 1, 'VanBanQPPL.XayDungVanBan', '20000000-0000-0000-0000-000000000020',
        N'Xay dung van ban', '', '', NULL, '',
        N'Kich hoat', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000026')
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
        '20000000-0000-0000-0000-000000000026', @SystemUser, @Now, @SystemUser, @Now,
        1, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.DanhSachVanBan', '20000000-0000-0000-0000-000000000025',
        N'Danh sach van ban', 'HoSoVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kich hoat', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000027')
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
        '20000000-0000-0000-0000-000000000027', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.XayDungVanBan', '20000000-0000-0000-0000-000000000025',
        N'Xay dung van ban', 'DangPhatTrien', 'XayDungVanBan', NULL, 'DangPhatTrien',
        N'Kich hoat', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000028')
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
        '20000000-0000-0000-0000-000000000028', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.GiaHanXayDung', '20000000-0000-0000-0000-000000000025',
        N'Gia han thoi gian xay dung', 'DangPhatTrien', 'GiaHanXayDung', NULL, 'DangPhatTrien',
        N'Kich hoat', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000029')
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
        '20000000-0000-0000-0000-000000000029', @SystemUser, @Now, @SystemUser, @Now,
        4, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.XetDuyetVanBan', '20000000-0000-0000-0000-000000000025',
        N'Xet duyet van ban', 'XetDuyetVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kich hoat', NULL, NULL
    );
END;

UPDATE RoleActions
SET Title = N'Xet duyet van ban',
    Controller = 'XetDuyetVanBan',
    Action = 'Index',
    [Table] = 'HoSoVanBans',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000029';

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000030')
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
        '20000000-0000-0000-0000-000000000030', @SystemUser, @Now, @SystemUser, @Now,
        5, 'Detail', 2, 'VanBanQPPL.XayDungVanBan.PheDuyetVanBan', '20000000-0000-0000-0000-000000000025',
        N'Phe duyet van ban', 'PheDuyetVanBan', 'Index', NULL, 'HoSoVanBans',
        N'Kich hoat', NULL, NULL
    );
END;

UPDATE RoleActions
SET Title = N'Phe duyet van ban',
    Controller = 'PheDuyetVanBan',
    Action = 'Index',
    [Table] = 'HoSoVanBans',
    UpdatedBy = @SystemUser,
    UpdatedDate = @Now
WHERE Id = '20000000-0000-0000-0000-000000000030';

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000031')
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
        '20000000-0000-0000-0000-000000000031', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Group', 0, 'ThiHanhPhapLuat', '00000000-0000-0000-0000-000000000000',
        N'Thuc hien thi hanh phap luat', '', '', NULL, '',
        N'Kich hoat', NULL, 'fas fa-balance-scale'
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000032')
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
        '20000000-0000-0000-0000-000000000032', @SystemUser, @Now, @SystemUser, @Now,
        1, 'Detail', 1, 'ThiHanhPhapLuat.DanhSachKeHoach', '20000000-0000-0000-0000-000000000031',
        N'Danh sach ke hoach', 'DangPhatTrien', 'DanhSachKeHoach', NULL, 'DangPhatTrien',
        N'Kich hoat', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000033')
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
        '20000000-0000-0000-0000-000000000033', @SystemUser, @Now, @SystemUser, @Now,
        2, 'Detail', 1, 'ThiHanhPhapLuat.QuaTrinhToChucThucHien', '20000000-0000-0000-0000-000000000031',
        N'Danh sach qua trinh to chuc thuc hien', 'DangPhatTrien', 'QuaTrinhToChucThucHien', NULL, 'DangPhatTrien',
        N'Kich hoat', NULL, NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM RoleActions WHERE Id = '20000000-0000-0000-0000-000000000034')
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
        '20000000-0000-0000-0000-000000000034', @SystemUser, @Now, @SystemUser, @Now,
        3, 'Detail', 1, 'ThiHanhPhapLuat.DanhGiaKetQua', '20000000-0000-0000-0000-000000000031',
        N'Danh gia ket qua', 'DangPhatTrien', 'DanhGiaKetQua', NULL, 'DangPhatTrien',
        N'Kich hoat', NULL, NULL
    );
END;
