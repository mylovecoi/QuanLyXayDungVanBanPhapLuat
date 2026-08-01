using DataAccess;
using DataAccess.Entities.Manages;
using DataAccess.Entities.QuanLyDanhMuc;
using DataAccess.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.QuanLyDanhMuc
{
    public interface IQuyTrinhSoanThaoService
    {
        Task<CommonResponse> GetDanhSachAsync(string search, int pageSize = 5, int pageCurrent = 1);
        Task<QuyTrinhSoanThaoEditModel> GetCreateModelAsync();
        Task<CommonResponse> GetEditAsync(Guid id);
        Task<CommonResponse> StoreAsync(QuyTrinhSoanThaoEditModel request);
        Task<CommonResponse> UpdateAsync(QuyTrinhSoanThaoEditModel request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<List<DanhMucVanBan>> GetDanhMucVanBanOptionsAsync();
        Task<List<DanhMucDonVi>> GetDanhMucDonViOptionsAsync();
    }

    public class QuyTrinhSoanThaoService(ApplicationDbContext dbContext) : IQuyTrinhSoanThaoService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetDanhSachAsync(string search, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var danhMucVanBanMap = await _dbContext.DanhMucVanBans
                    .AsNoTracking()
                    .ToDictionaryAsync(x => x.Id, x => x.TenLoaiVanBan);

                var query = _dbContext.DanhMucQuyTrinhSoanThaos
                    .AsNoTracking()
                    .Select(quyTrinh => new QuyTrinhSoanThaoListItemModel
                    {
                        Id = quyTrinh.Id,
                        MaQuyTrinh = quyTrinh.MaQuyTrinh,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        LoaiQuyTrinh = quyTrinh.LoaiQuyTrinh,
                        TenLoaiVanBan = quyTrinh.DanhMucVanBanIds,
                        CapApDung = quyTrinh.CapApDung,
                        PhienBan = quyTrinh.PhienBan,
                        TrangThai = quyTrinh.TrangThai,
                        SoBuoc = _dbContext.DanhMucBuocQuyTrinhs.Count(x => x.QuyTrinhSoanThaoId == quyTrinh.Id),
                        SoNhanhChuyen = _dbContext.DanhMucChuyenBuocQuyTrinhs.Count(x => x.QuyTrinhSoanThaoId == quyTrinh.Id)
                    });

                var rawItems = await query.OrderBy(x => x.CapApDung).ThenBy(x => x.MaQuyTrinh).ToListAsync();

                foreach (var item in rawItems)
                {
                    item.TenLoaiQuyTrinh = FormatLoaiQuyTrinhDisplay(item.LoaiQuyTrinh);
                    item.TenLoaiVanBan = ResolveDanhMucVanBanNames(item.TenLoaiVanBan, danhMucVanBanMap);
                    item.CapApDung = FormatCapApDungDisplay(item.CapApDung);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    rawItems = rawItems.Where(x =>
                            x.MaQuyTrinh.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            x.TenQuyTrinh.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            (!string.IsNullOrWhiteSpace(x.TenLoaiQuyTrinh) && x.TenLoaiQuyTrinh.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(x.TenLoaiVanBan) && x.TenLoaiVanBan.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(x.CapApDung) && x.CapApDung.Contains(search, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                var totalRecord = rawItems.Count;
                var data = rawItems.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToList();
                return new CommonResponse("success", "Thành công", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public Task<QuyTrinhSoanThaoEditModel> GetCreateModelAsync()
        {
            return Task.FromResult(QuyTrinhSoanThaoDefaultFactory.CreateDefault());
        }

        public async Task<CommonResponse> GetEditAsync(Guid id)
        {
            try
            {
                var quyTrinh = await _dbContext.DanhMucQuyTrinhSoanThaos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (quyTrinh == null)
                {
                    return new CommonResponse("error", "Không tìm thấy quy trình soạn thảo!");
                }

                var buocs = await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == id)
                    .OrderBy(x => x.ThuTuSapXep)
                    .ThenBy(x => x.MaBuoc)
                    .Select(x => new QuyTrinhSoanThaoBuocModel
                    {
                        Id = x.Id,
                        MaBuoc = x.MaBuoc,
                        TenBuoc = x.TenBuoc,
                        ThuTuSapXep = x.ThuTuSapXep,
                        LoaiBuoc = x.LoaiBuoc,
                        BatBuoc = x.BatBuoc,
                        ChoPhepBoQua = x.ChoPhepBoQua,
                        ChoPhepQuayLui = x.ChoPhepQuayLui,
                        CachHoanThanh = x.CachHoanThanh,
                        SoLuongPhanHoiToiThieu = x.SoLuongPhanHoiToiThieu,
                        YeuCauFileDinhKem = x.YeuCauFileDinhKem,
                        SoLanTraLaiToiDa = x.SoLanTraLaiToiDa,
                        SoNgayXuLyTieuChuan = x.SoNgayXuLyTieuChuan,
                        SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan,
                        DonViTiepNhanMacDinhId = x.DonViTiepNhanMacDinhId,
                        MoTa = x.MoTa,
                        GhiChu = x.GhiChu
                    })
                    .ToListAsync();

                var buocMap = buocs.ToDictionary(x => x.Id, x => x.MaBuoc);

                var chuyenBuocs = await _dbContext.DanhMucChuyenBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == id)
                    .OrderBy(x => x.CreatedDate)
                    .Select(x => new
                    {
                        x.Id,
                        x.TuBuocId,
                        x.DenBuocId,
                        x.DieuKienKetQua,
                        x.LaNhanhMacDinh,
                        x.MoTa,
                        x.GhiChu
                    })
                    .ToListAsync();

                var model = new QuyTrinhSoanThaoEditModel
                {
                    Id = quyTrinh.Id,
                    MaQuyTrinh = quyTrinh.MaQuyTrinh,
                    TenQuyTrinh = quyTrinh.TenQuyTrinh,
                    LoaiQuyTrinh = string.IsNullOrWhiteSpace(quyTrinh.LoaiQuyTrinh) ? "XayDung" : quyTrinh.LoaiQuyTrinh,
                    DanhMucVanBanId = quyTrinh.DanhMucVanBanId,
                    DanhMucVanBanIds = ParseGuidList(quyTrinh.DanhMucVanBanIds, quyTrinh.DanhMucVanBanId),
                    CapApDung = quyTrinh.CapApDung,
                    CapApDungs = ParseCapApDungList(quyTrinh.CapApDung),
                    PhienBan = quyTrinh.PhienBan,
                    TrangThai = quyTrinh.TrangThai,
                    MoTa = quyTrinh.MoTa,
                    GhiChu = quyTrinh.GhiChu,
                    BuocQuyTrinhs = buocs,
                    ChuyenBuocs = chuyenBuocs.Select(x => new QuyTrinhSoanThaoChuyenBuocModel
                    {
                        Id = x.Id,
                        TuBuocMa = buocMap.TryGetValue(x.TuBuocId, out var tuBuocMa) ? tuBuocMa : string.Empty,
                        DenBuocMa = buocMap.TryGetValue(x.DenBuocId, out var denBuocMa) ? denBuocMa : string.Empty,
                        DieuKienKetQua = x.DieuKienKetQua,
                        LaNhanhMacDinh = x.LaNhanhMacDinh,
                        MoTa = x.MoTa,
                        GhiChu = x.GhiChu
                    }).ToList()
                };

                return new CommonResponse("success", "Thành công", model);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> StoreAsync(QuyTrinhSoanThaoEditModel request)
        {
            var sanitizeResult = SanitizeAndValidate(request, Guid.Empty);
            if (sanitizeResult != null)
            {
                return sanitizeResult;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = new DanhMucQuyTrinhSoanThao
                {
                    MaQuyTrinh = request.MaQuyTrinh.Trim(),
                    TenQuyTrinh = request.TenQuyTrinh.Trim(),
                    LoaiQuyTrinh = request.LoaiQuyTrinh,
                    DanhMucVanBanId = request.DanhMucVanBanId,
                    DanhMucVanBanIds = ConvertGuidListToString(request.DanhMucVanBanIds),
                    CapApDung = string.Join(",", request.CapApDungs),
                    PhienBan = request.PhienBan > 0 ? request.PhienBan : 1,
                    TrangThai = request.TrangThai,
                    MoTa = request.MoTa?.Trim(),
                    GhiChu = request.GhiChu?.Trim()
                };

                _dbContext.DanhMucQuyTrinhSoanThaos.Add(entity);
                await _dbContext.SaveChangesAsync();

                var stepMap = await UpsertStepsAsync(entity.Id, request.BuocQuyTrinhs, new List<DanhMucBuocQuyTrinh>());
                await ReplaceTransitionsAsync(entity.Id, request.ChuyenBuocs, stepMap);

                await transaction.CommitAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> UpdateAsync(QuyTrinhSoanThaoEditModel request)
        {
            var sanitizeResult = SanitizeAndValidate(request, request.Id);
            if (sanitizeResult != null)
            {
                return sanitizeResult;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = await _dbContext.DanhMucQuyTrinhSoanThaos.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null)
                {
                    return new CommonResponse("error", "Không tìm thấy quy trình soạn thảo!");
                }

                var existingSteps = await _dbContext.DanhMucBuocQuyTrinhs
                    .Where(x => x.QuyTrinhSoanThaoId == request.Id)
                    .ToListAsync();

                var requestedStepIds = request.BuocQuyTrinhs.Where(x => x.Id != Guid.Empty).Select(x => x.Id).ToHashSet();
                var deletedStepIds = existingSteps.Where(x => !requestedStepIds.Contains(x.Id)).Select(x => x.Id).ToList();

                if (deletedStepIds.Count > 0 && await IsAnyStepInUseAsync(deletedStepIds))
                {
                    return new CommonResponse("error", "Không thể xóa bước đã được sử dụng trong hồ sơ văn bản!");
                }

                entity.MaQuyTrinh = request.MaQuyTrinh.Trim();
                entity.TenQuyTrinh = request.TenQuyTrinh.Trim();
                entity.LoaiQuyTrinh = request.LoaiQuyTrinh;
                entity.DanhMucVanBanId = request.DanhMucVanBanId;
                entity.DanhMucVanBanIds = ConvertGuidListToString(request.DanhMucVanBanIds);
                entity.CapApDung = string.Join(",", request.CapApDungs);
                entity.TrangThai = request.TrangThai;
                entity.MoTa = request.MoTa?.Trim();
                entity.GhiChu = request.GhiChu?.Trim();

                _dbContext.DanhMucQuyTrinhSoanThaos.Update(entity);
                await _dbContext.SaveChangesAsync();

                if (deletedStepIds.Count > 0)
                {
                    var transitionsToDelete = await _dbContext.DanhMucChuyenBuocQuyTrinhs
                        .Where(x => x.QuyTrinhSoanThaoId == request.Id &&
                               (deletedStepIds.Contains(x.TuBuocId) || deletedStepIds.Contains(x.DenBuocId)))
                        .ToListAsync();

                    if (transitionsToDelete.Count > 0)
                    {
                        _dbContext.DanhMucChuyenBuocQuyTrinhs.RemoveRange(transitionsToDelete);
                    }

                    var stepsToDelete = existingSteps.Where(x => deletedStepIds.Contains(x.Id)).ToList();
                    _dbContext.DanhMucBuocQuyTrinhs.RemoveRange(stepsToDelete);
                    await _dbContext.SaveChangesAsync();
                }

                var stepMap = await UpsertStepsAsync(entity.Id, request.BuocQuyTrinhs, existingSteps);
                await ReplaceTransitionsAsync(entity.Id, request.ChuyenBuocs, stepMap);

                await transaction.CommitAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.DanhMucQuyTrinhSoanThaos.FindAsync(id);
                if (entity == null)
                {
                    return new CommonResponse("error", "Không tìm thấy quy trình soạn thảo!");
                }

                var isUsed = await _dbContext.HoSoVanBans.AnyAsync(x => x.QuyTrinhSoanThaoId == id);
                if (isUsed)
                {
                    return new CommonResponse("error", "Quy trình đã phát sinh hồ sơ văn bản, không thể xóa!");
                }

                _dbContext.DanhMucQuyTrinhSoanThaos.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<List<DanhMucVanBan>> GetDanhMucVanBanOptionsAsync()
        {
            return await _dbContext.DanhMucVanBans
                .AsNoTracking()
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.TenLoaiVanBan)
                .ToListAsync();
        }

        public async Task<List<DanhMucDonVi>> GetDanhMucDonViOptionsAsync()
        {
            return await _dbContext.DanhMucDonVis
                .AsNoTracking()
                .OrderBy(x => x.STTSapXep)
                .ThenBy(x => x.TenDonVi)
                .ToListAsync();
        }

        private CommonResponse? SanitizeAndValidate(QuyTrinhSoanThaoEditModel request, Guid id)
        {
            request.MaQuyTrinh = request.MaQuyTrinh?.Trim() ?? string.Empty;
            request.TenQuyTrinh = request.TenQuyTrinh?.Trim() ?? string.Empty;
            request.LoaiQuyTrinh = NormalizeLoaiQuyTrinhValue(request.LoaiQuyTrinh);
            request.CapApDungs = request.CapApDungs
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(NormalizeCapApDungValue)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (request.CapApDungs.Count == 0)
            {
                request.CapApDungs = new List<string> { "Tinh" };
            }

            request.CapApDung = string.Join(",", request.CapApDungs);
            request.DanhMucVanBanIds = request.DanhMucVanBanIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();
            request.DanhMucVanBanId = request.DanhMucVanBanIds.FirstOrDefault();
            request.BuocQuyTrinhs = request.BuocQuyTrinhs
                .Where(x => !string.IsNullOrWhiteSpace(x.MaBuoc) ||
                            !string.IsNullOrWhiteSpace(x.TenBuoc) ||
                            !string.IsNullOrWhiteSpace(x.LoaiBuoc))
                .Select(x =>
                {
                    x.MaBuoc = x.MaBuoc?.Trim() ?? string.Empty;
                    x.TenBuoc = x.TenBuoc?.Trim() ?? string.Empty;
                    x.LoaiBuoc = string.IsNullOrWhiteSpace(x.LoaiBuoc) ? "XuLy" : x.LoaiBuoc.Trim();
                    x.CachHoanThanh = x.CachHoanThanh?.Trim();
                    x.SoNgayXuLyTieuChuan = x.SoNgayXuLyTieuChuan.HasValue && x.SoNgayXuLyTieuChuan.Value <= 0 ? null : x.SoNgayXuLyTieuChuan;
                    x.SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan.HasValue && x.SoNgayCanhBaoSapHan.Value < 0 ? 0 : x.SoNgayCanhBaoSapHan;
                    x.DonViTiepNhanMacDinhId = x.DonViTiepNhanMacDinhId == Guid.Empty ? null : x.DonViTiepNhanMacDinhId;
                    x.MoTa = x.MoTa?.Trim();
                    x.GhiChu = x.GhiChu?.Trim();
                    return x;
                })
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .ToList();

            request.ChuyenBuocs = request.ChuyenBuocs
                .Where(x => !string.IsNullOrWhiteSpace(x.TuBuocMa) ||
                            !string.IsNullOrWhiteSpace(x.DenBuocMa) ||
                            !string.IsNullOrWhiteSpace(x.DieuKienKetQua))
                .Select(x =>
                {
                    x.TuBuocMa = x.TuBuocMa?.Trim() ?? string.Empty;
                    x.DenBuocMa = x.DenBuocMa?.Trim() ?? string.Empty;
                    x.DieuKienKetQua = x.DieuKienKetQua?.Trim() ?? string.Empty;
                    x.MoTa = x.MoTa?.Trim();
                    x.GhiChu = x.GhiChu?.Trim();
                    return x;
                })
                .ToList();

            if (string.IsNullOrWhiteSpace(request.MaQuyTrinh))
            {
                return new CommonResponse("error", "Ma quy trinh khong duoc de trong!");
            }

            if (string.IsNullOrWhiteSpace(request.TenQuyTrinh))
            {
                return new CommonResponse("error", "Ten quy trinh khong duoc de trong!");
            }

            if (string.IsNullOrWhiteSpace(request.LoaiQuyTrinh))
            {
                return new CommonResponse("error", "Loai quy trinh khong duoc de trong!");
            }

            if (_dbContext.DanhMucQuyTrinhSoanThaos.Any(x => x.MaQuyTrinh == request.MaQuyTrinh && x.Id != id))
            {
                return new CommonResponse("error", "Mã quy trình đã tồn tại!");
            }

            if (request.BuocQuyTrinhs.Count == 0)
            {
                return new CommonResponse("error", "Phai co it nhat 1 buoc quy trinh!");
            }

            if (request.BuocQuyTrinhs.Any(x => string.IsNullOrWhiteSpace(x.MaBuoc) || string.IsNullOrWhiteSpace(x.TenBuoc)))
            {
                return new CommonResponse("error", "Tat ca cac buoc phai co ma buoc va ten buoc!");
            }

            var duplicateStepCodes = request.BuocQuyTrinhs
                .GroupBy(x => x.MaBuoc, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateStepCodes.Count > 0)
            {
                return new CommonResponse("error", $"Mã bước bị trùng: {string.Join(", ", duplicateStepCodes)}");
            }

            var stepCodes = request.BuocQuyTrinhs
                .Select(x => x.MaBuoc)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var item in request.ChuyenBuocs)
            {
                if (!stepCodes.Contains(item.TuBuocMa) || !stepCodes.Contains(item.DenBuocMa))
                {
                    return new CommonResponse("error", $"Nhanh chuyen buoc '{item.TuBuocMa} -> {item.DenBuocMa}' khong hop le vi ma buoc khong ton tai.");
                }

                if (string.IsNullOrWhiteSpace(item.DieuKienKetQua))
                {
                    return new CommonResponse("error", "Nhanh chuyen buoc phai co dieu kien ket qua!");
                }
            }

            return null;
        }

        private static List<Guid> ParseGuidList(string? value, Guid? fallbackValue)
        {
            var result = (value ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(item => Guid.TryParse(item, out var parsed) ? parsed : Guid.Empty)
                .Where(item => item != Guid.Empty)
                .Distinct()
                .ToList();

            if (result.Count == 0 && fallbackValue.HasValue && fallbackValue.Value != Guid.Empty)
            {
                result.Add(fallbackValue.Value);
            }

            return result;
        }

        private static string ConvertGuidListToString(List<Guid> values)
        {
            return string.Join(",", values.Where(x => x != Guid.Empty).Distinct());
        }

        private static List<string> ParseCapApDungList(string? value)
        {
            var result = (value ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(NormalizeCapApDungValue)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return result.Count > 0 ? result : new List<string> { "Tinh" };
        }

        private static string NormalizeCapApDungValue(string? value)
        {
            var normalized = (value ?? string.Empty).Trim();
            return normalized.ToUpperInvariant() switch
            {
                "TỈNH" => "Tinh",
                "TINH" => "Tinh",
                "XÃ" => "Xa",
                "XA" => "Xa",
                _ => normalized
            };
        }

        private static string NormalizeLoaiQuyTrinhValue(string? value)
        {
            var normalized = (value ?? string.Empty).Trim();
            return normalized.ToUpperInvariant() switch
            {
                "DANGKY" => "DangKy",
                "ĐĂNGKÝ" => "DangKy",
                "ĐĂNG KÝ" => "DangKy",
                "DANG_KY" => "DangKy",
                "XAYDUNG" => "XayDung",
                "XÂYDỰNG" => "XayDung",
                "XÂY DỰNG" => "XayDung",
                "XAY_DUNG" => "XayDung",
                _ => string.IsNullOrWhiteSpace(normalized) ? "XayDung" : normalized
            };
        }

        private static string FormatLoaiQuyTrinhDisplay(string? loaiQuyTrinh)
        {
            return NormalizeLoaiQuyTrinhValue(loaiQuyTrinh) switch
            {
                "DangKy" => "Đăng ký",
                "XayDung" => "Xây dựng",
                _ => loaiQuyTrinh ?? "Xây dựng"
            };
        }

        private static string? ResolveDanhMucVanBanNames(string? csvIds, Dictionary<Guid, string> danhMucVanBanMap)
        {
            if (string.IsNullOrWhiteSpace(csvIds))
            {
                return null;
            }

            var names = csvIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(item => Guid.TryParse(item, out var parsed) ? parsed : Guid.Empty)
                .Where(item => item != Guid.Empty && danhMucVanBanMap.ContainsKey(item))
                .Select(item => danhMucVanBanMap[item])
                .Distinct()
                .ToList();

            return names.Count > 0 ? string.Join(", ", names) : null;
        }

        private static string? FormatCapApDungDisplay(string? capApDung)
        {
            if (string.IsNullOrWhiteSpace(capApDung))
            {
                return capApDung;
            }

            return string.Join(", ",
                capApDung
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(item => item.Equals("Tinh", StringComparison.OrdinalIgnoreCase) ? "Tỉnh" :
                                    item.Equals("Xa", StringComparison.OrdinalIgnoreCase) ? "Xã" :
                                    item)
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private async Task<Dictionary<string, Guid>> UpsertStepsAsync(
            Guid quyTrinhId,
            List<QuyTrinhSoanThaoBuocModel> requestSteps,
            List<DanhMucBuocQuyTrinh> existingSteps)
        {
            var existingMap = existingSteps.ToDictionary(x => x.Id, x => x);
            var codeMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in requestSteps)
            {
                DanhMucBuocQuyTrinh entity;
                if (item.Id != Guid.Empty && existingMap.TryGetValue(item.Id, out var existing))
                {
                    entity = existing;
                }
                else
                {
                    entity = new DanhMucBuocQuyTrinh
                    {
                        QuyTrinhSoanThaoId = quyTrinhId
                    };
                    _dbContext.DanhMucBuocQuyTrinhs.Add(entity);
                }

                entity.QuyTrinhSoanThaoId = quyTrinhId;
                entity.MaBuoc = item.MaBuoc;
                entity.TenBuoc = item.TenBuoc;
                entity.ThuTuSapXep = item.ThuTuSapXep <= 0 ? 1 : item.ThuTuSapXep;
                entity.LoaiBuoc = item.LoaiBuoc;
                entity.BatBuoc = item.BatBuoc;
                entity.ChoPhepBoQua = item.ChoPhepBoQua;
                entity.ChoPhepQuayLui = item.ChoPhepQuayLui;
                entity.CachHoanThanh = item.CachHoanThanh;
                entity.SoLuongPhanHoiToiThieu = item.SoLuongPhanHoiToiThieu;
                entity.YeuCauFileDinhKem = item.YeuCauFileDinhKem;
                entity.SoLanTraLaiToiDa = item.SoLanTraLaiToiDa < 0 ? 0 : item.SoLanTraLaiToiDa;
                entity.SoNgayXuLyTieuChuan = item.SoNgayXuLyTieuChuan.HasValue && item.SoNgayXuLyTieuChuan.Value > 0 ? item.SoNgayXuLyTieuChuan : null;
                entity.SoNgayCanhBaoSapHan = item.SoNgayCanhBaoSapHan.HasValue && item.SoNgayCanhBaoSapHan.Value >= 0 ? item.SoNgayCanhBaoSapHan : null;
                entity.DonViTiepNhanMacDinhId = item.DonViTiepNhanMacDinhId;
                entity.MoTa = item.MoTa;
                entity.GhiChu = item.GhiChu;
            }

            await _dbContext.SaveChangesAsync();

            var refreshedSteps = await _dbContext.DanhMucBuocQuyTrinhs
                .Where(x => x.QuyTrinhSoanThaoId == quyTrinhId)
                .ToListAsync();

            foreach (var item in refreshedSteps)
            {
                codeMap[item.MaBuoc] = item.Id;
            }

            return codeMap;
        }

        private async Task ReplaceTransitionsAsync(
            Guid quyTrinhId,
            List<QuyTrinhSoanThaoChuyenBuocModel> requestTransitions,
            Dictionary<string, Guid> stepMap)
        {
            var oldTransitions = await _dbContext.DanhMucChuyenBuocQuyTrinhs
                .Where(x => x.QuyTrinhSoanThaoId == quyTrinhId)
                .ToListAsync();

            if (oldTransitions.Count > 0)
            {
                _dbContext.DanhMucChuyenBuocQuyTrinhs.RemoveRange(oldTransitions);
                await _dbContext.SaveChangesAsync();
            }

            var newTransitions = requestTransitions.Select(x => new DanhMucChuyenBuocQuyTrinh
            {
                QuyTrinhSoanThaoId = quyTrinhId,
                TuBuocId = stepMap[x.TuBuocMa],
                DenBuocId = stepMap[x.DenBuocMa],
                DieuKienKetQua = x.DieuKienKetQua,
                LaNhanhMacDinh = x.LaNhanhMacDinh,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            }).ToList();

            if (newTransitions.Count > 0)
            {
                _dbContext.DanhMucChuyenBuocQuyTrinhs.AddRange(newTransitions);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<bool> IsAnyStepInUseAsync(List<Guid> stepIds)
        {
            return await _dbContext.HoSoVanBans.AnyAsync(x => x.BuocHienTaiId != null && stepIds.Contains(x.BuocHienTaiId.Value)) ||
                   await _dbContext.HoSoVanBanXuLys.AnyAsync(x => stepIds.Contains(x.BuocQuyTrinhId)) ||
                   await _dbContext.HoSoVanBanLayYKiens.AnyAsync(x => stepIds.Contains(x.BuocQuyTrinhId)) ||
                   await _dbContext.HoSoVanBanDanhGias.AnyAsync(x => stepIds.Contains(x.BuocQuyTrinhId) ||
                                                                     (x.TraLaiBuocId != null && stepIds.Contains(x.TraLaiBuocId.Value)));
        }
    }
}
