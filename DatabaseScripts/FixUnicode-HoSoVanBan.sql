UPDATE HoSoVanBans SET TenHoSo = N'Hồ sơ mẫu hoàn chỉnh quy trình 7 bước' WHERE MaHoSo = N'HSVB-2026-002';
UPDATE HoSoVanBans SET TenHoSo = N'Hồ sơ mẫu đang dừng ở bước 2' WHERE MaHoSo = N'HSVB-2026-003';
UPDATE HoSoVanBans SET TenHoSo = N'Hồ sơ mẫu bị trả lại 1 lần và đang ở bước 5' WHERE MaHoSo = N'HSVB-2026-004';
UPDATE HoSoVanBans SET TenHoSo = N'Hồ sơ mẫu đang quá hạn ở bước lấy ý kiến' WHERE MaHoSo = N'HSVB-2026-005';
UPDATE HoSoVanBans SET TenHoSo = N'Hồ sơ mẫu đang chờ VP UBND tiếp nhận/xét duyệt đăng ký' WHERE MaHoSo = N'HSVB-2026-006';
UPDATE HoSoVanBans SET TenHoSo = N'Hồ sơ mẫu đang ở bước trình cơ quan có thẩm quyền' WHERE MaHoSo = N'HSVB-2026-007';

SELECT MaHoSo, TenHoSo
FROM HoSoVanBans
WHERE MaHoSo IN (
    N'HSVB-2026-002',
    N'HSVB-2026-003',
    N'HSVB-2026-004',
    N'HSVB-2026-005',
    N'HSVB-2026-006',
    N'HSVB-2026-007'
)
ORDER BY MaHoSo;
