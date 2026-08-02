using DataAccess;
using DataAccess.Entities.QuanLyDanhMuc;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.QuanLyDanhMuc
{
    public interface IDanhMucTieuChiDiemService
    {
        Task<CommonResponse> GetDanhSachAsync(string search, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> StoreAsync(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc>? mucs = null);
        Task<CommonResponse> UpdateAsync(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc>? mucs = null);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<bool> CheckDuplicateAsync(string maTieuChi, Guid id);
        Task<int> GetNextThuTuSapXepAsync();
        Task EnsureDefaultDataAsync();
    }

    public class DanhMucTieuChiDiemService(ApplicationDbContext dbContext) : IDanhMucTieuChiDiemService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetDanhSachAsync(string search, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var query = _dbContext.DanhMucTieuChiDiems.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.MaTieuChi.Contains(search) ||
                        x.TenTieuChi.Contains(search) ||
                        x.LoaiTieuChi.Contains(search) ||
                        x.KieuGiaTri.Contains(search) ||
                        x.DonViGiaTri.Contains(search) ||
                        (x.MoTa != null && x.MoTa.Contains(search)) ||
                        (x.GhiChu != null && x.GhiChu.Contains(search)));
                }

                query = query.OrderBy(x => x.ThuTuSapXep).ThenBy(x => x.TenTieuChi);

                var totalRecord = await query.CountAsync();
                var data = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

                return new CommonResponse("success", "Thành công", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var tieuChi = await _dbContext.DanhMucTieuChiDiems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (tieuChi == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin!");
                }

                var mucs = await _dbContext.DanhMucTieuChiDiemMucs.AsNoTracking()
                    .Where(x => x.DanhMucTieuChiDiemId == id)
                    .OrderBy(x => x.ThuTuSapXep)
                    .ThenBy(x => x.TuGiaTri)
                    .ToListAsync();

                return new CommonResponse("success", "Thành công", new { TieuChi = tieuChi, Mucs = mucs });
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> StoreAsync(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc>? mucs = null)
        {
            try
            {
                if (await CheckDuplicateAsync(request.MaTieuChi, Guid.Empty))
                {
                    return new CommonResponse("error", "Mã tiêu chí đã tồn tại!");
                }

                NormalizeRequest(request, mucs);
                var validateMessage = ValidateRequest(request, mucs);
                if (!string.IsNullOrWhiteSpace(validateMessage))
                {
                    return new CommonResponse("error", validateMessage);
                }

                if (mucs != null && mucs.Count > 0)
                {
                    foreach (var muc in mucs)
                    {
                        muc.DanhMucTieuChiDiemId = request.Id;
                    }
                }

                _dbContext.DanhMucTieuChiDiems.Add(request);
                if (mucs != null && mucs.Count > 0)
                {
                    _dbContext.DanhMucTieuChiDiemMucs.AddRange(mucs);
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc>? mucs = null)
        {
            try
            {
                if (await CheckDuplicateAsync(request.MaTieuChi, request.Id))
                {
                    return new CommonResponse("error", "Mã tiêu chí đã tồn tại!");
                }

                var data = await _dbContext.DanhMucTieuChiDiems.FindAsync(request.Id);
                if (data == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin!");
                }

                NormalizeRequest(request, mucs);
                var validateMessage = ValidateRequest(request, mucs);
                if (!string.IsNullOrWhiteSpace(validateMessage))
                {
                    return new CommonResponse("error", validateMessage);
                }

                data.MaTieuChi = request.MaTieuChi;
                data.TenTieuChi = request.TenTieuChi;
                data.LoaiTieuChi = request.LoaiTieuChi;
                data.KieuGiaTri = request.KieuGiaTri;
                data.DonViGiaTri = request.DonViGiaTri;
                data.ThuTuSapXep = request.ThuTuSapXep;
                data.DiemToiDa = request.DiemToiDa;
                data.TrangThai = request.TrangThai;
                data.MoTa = request.MoTa;
                data.GhiChu = request.GhiChu;

                _dbContext.DanhMucTieuChiDiems.Update(data);

                var oldMucs = await _dbContext.DanhMucTieuChiDiemMucs
                    .Where(x => x.DanhMucTieuChiDiemId == data.Id)
                    .ToListAsync();
                if (oldMucs.Count > 0)
                {
                    _dbContext.DanhMucTieuChiDiemMucs.RemoveRange(oldMucs);
                }

                if (mucs != null && mucs.Count > 0)
                {
                    foreach (var muc in mucs)
                    {
                        muc.DanhMucTieuChiDiemId = data.Id;
                    }

                    _dbContext.DanhMucTieuChiDiemMucs.AddRange(mucs);
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucTieuChiDiems.FindAsync(id);
                if (data == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin!");
                }

                _dbContext.DanhMucTieuChiDiems.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<bool> CheckDuplicateAsync(string maTieuChi, Guid id)
        {
            return await _dbContext.DanhMucTieuChiDiems.AnyAsync(x => x.MaTieuChi == maTieuChi && x.Id != id);
        }

        public async Task<int> GetNextThuTuSapXepAsync()
        {
            var maxThuTu = await _dbContext.DanhMucTieuChiDiems
                .AsNoTracking()
                .Select(x => (int?)x.ThuTuSapXep)
                .MaxAsync();

            return (maxThuTu ?? 0) + 1;
        }

        public async Task EnsureDefaultDataAsync()
        {
            if (await _dbContext.DanhMucTieuChiDiems.AnyAsync())
            {
                return;
            }

            var thoiGianId = Guid.NewGuid();
            var chatLuongId = Guid.NewGuid();

            var tieuChis = new List<DanhMucTieuChiDiem>
            {
                new()
                {
                    Id = thoiGianId,
                    MaTieuChi = "THOI_GIAN_XAY_DUNG",
                    TenTieuChi = "Thời gian xây dựng văn bản",
                    LoaiTieuChi = "THOI_GIAN",
                    KieuGiaTri = "TY_LE",
                    DonViGiaTri = "PERCENT",
                    ThuTuSapXep = 1,
                    DiemToiDa = 40,
                    MoTa = "Căn cứ theo tỷ lệ % thời gian thực tế / tổng thời gian quy định."
                },
                new()
                {
                    Id = chatLuongId,
                    MaTieuChi = "CHAT_LUONG_XAY_DUNG",
                    TenTieuChi = "Chất lượng văn bản xây dựng",
                    LoaiTieuChi = "CHAT_LUONG",
                    KieuGiaTri = "SO_LAN",
                    DonViGiaTri = "COUNT",
                    ThuTuSapXep = 2,
                    DiemToiDa = 60,
                    MoTa = "Căn cứ theo số lần trả lại ở bước đánh giá chất lượng."
                }
            };

            var mucs = new List<DanhMucTieuChiDiemMuc>
            {
                new() { DanhMucTieuChiDiemId = thoiGianId, TuGiaTri = null, DenGiaTri = 80, BaoGomDenGiaTri = true, Diem = 40, NhanHienThi = "<= 80% thời gian chuẩn", ThuTuSapXep = 1 },
                new() { DanhMucTieuChiDiemId = thoiGianId, TuGiaTri = 80, DenGiaTri = 100, BaoGomTuGiaTri = false, BaoGomDenGiaTri = true, Diem = 35, NhanHienThi = "> 80% đến 100%", ThuTuSapXep = 2 },
                new() { DanhMucTieuChiDiemId = thoiGianId, TuGiaTri = 100, DenGiaTri = 110, BaoGomTuGiaTri = false, BaoGomDenGiaTri = true, Diem = 28, NhanHienThi = "> 100% đến 110%", ThuTuSapXep = 3 },
                new() { DanhMucTieuChiDiemId = thoiGianId, TuGiaTri = 110, DenGiaTri = 125, BaoGomTuGiaTri = false, BaoGomDenGiaTri = true, Diem = 20, NhanHienThi = "> 110% đến 125%", ThuTuSapXep = 4 },
                new() { DanhMucTieuChiDiemId = thoiGianId, TuGiaTri = 125, DenGiaTri = 150, BaoGomTuGiaTri = false, BaoGomDenGiaTri = true, Diem = 10, NhanHienThi = "> 125% đến 150%", ThuTuSapXep = 5 },
                new() { DanhMucTieuChiDiemId = thoiGianId, TuGiaTri = 150, DenGiaTri = null, BaoGomTuGiaTri = false, Diem = 0, NhanHienThi = "> 150%", ThuTuSapXep = 6 },

                new() { DanhMucTieuChiDiemId = chatLuongId, TuGiaTri = 0, DenGiaTri = 0, Diem = 60, NhanHienThi = "Không bị trả lại lần nào", ThuTuSapXep = 1 },
                new() { DanhMucTieuChiDiemId = chatLuongId, TuGiaTri = 1, DenGiaTri = 1, Diem = 45, NhanHienThi = "Trả lại 1 lần", ThuTuSapXep = 2 },
                new() { DanhMucTieuChiDiemId = chatLuongId, TuGiaTri = 2, DenGiaTri = 2, Diem = 30, NhanHienThi = "Trả lại 2 lần", ThuTuSapXep = 3 },
                new() { DanhMucTieuChiDiemId = chatLuongId, TuGiaTri = 3, DenGiaTri = 3, Diem = 15, NhanHienThi = "Trả lại 3 lần", ThuTuSapXep = 4 },
                new() { DanhMucTieuChiDiemId = chatLuongId, TuGiaTri = 4, DenGiaTri = null, Diem = 0, NhanHienThi = "Trả lại từ 4 lần trở lên", ThuTuSapXep = 5 }
            };

            _dbContext.DanhMucTieuChiDiems.AddRange(tieuChis);
            _dbContext.DanhMucTieuChiDiemMucs.AddRange(mucs);
            await _dbContext.SaveChangesAsync();
        }

        private static void NormalizeRequest(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc>? mucs)
        {
            request.MaTieuChi = request.MaTieuChi.Trim().ToUpperInvariant();
            request.TenTieuChi = request.TenTieuChi.Trim();
            request.LoaiTieuChi = request.LoaiTieuChi.Trim().ToUpperInvariant();
            request.KieuGiaTri = request.KieuGiaTri.Trim().ToUpperInvariant();
            request.DonViGiaTri = request.DonViGiaTri.Trim().ToUpperInvariant();
            request.MoTa = string.IsNullOrWhiteSpace(request.MoTa) ? null : request.MoTa.Trim();
            request.GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim();

            if (mucs == null)
            {
                return;
            }

            for (var i = 0; i < mucs.Count; i++)
            {
                mucs[i].ThuTuSapXep = mucs[i].ThuTuSapXep <= 0 ? i + 1 : mucs[i].ThuTuSapXep;
                mucs[i].NhanHienThi = string.IsNullOrWhiteSpace(mucs[i].NhanHienThi) ? null : mucs[i].NhanHienThi.Trim();
                mucs[i].GhiChu = string.IsNullOrWhiteSpace(mucs[i].GhiChu) ? null : mucs[i].GhiChu.Trim();
            }
        }

        private static string? ValidateRequest(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc>? mucs)
        {
            if (string.IsNullOrWhiteSpace(request.MaTieuChi))
            {
                return "Mã tiêu chí không được bỏ trống.";
            }

            if (string.IsNullOrWhiteSpace(request.TenTieuChi))
            {
                return "Tên tiêu chí không được bỏ trống.";
            }

            if (request.DiemToiDa < 0)
            {
                return "Điểm tối đa phải lớn hơn hoặc bằng 0.";
            }

            var activeMucs = (mucs ?? new List<DanhMucTieuChiDiemMuc>())
                .Where(x => x.TrangThai)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.TuGiaTri ?? decimal.MinValue)
                .ToList();

            if (activeMucs.Count == 0)
            {
                return "Phải có ít nhất một mức điểm đang kích hoạt.";
            }

            foreach (var muc in activeMucs)
            {
                if (!muc.TuGiaTri.HasValue && !muc.DenGiaTri.HasValue)
                {
                    return "Mỗi mức điểm phải có ít nhất một giá trị đầu hoặc cuối.";
                }

                if (muc.Diem < 0)
                {
                    return "Điểm của từng mức phải lớn hơn hoặc bằng 0.";
                }

                if (muc.Diem > request.DiemToiDa)
                {
                    return $"Điểm từng mức không được vượt quá điểm tối đa của tiêu chí ({request.DiemToiDa}).";
                }

                if (muc.TuGiaTri.HasValue && muc.DenGiaTri.HasValue)
                {
                    if (muc.TuGiaTri.Value > muc.DenGiaTri.Value)
                    {
                        return "Có mức điểm có Từ giá trị lớn hơn Đến giá trị.";
                    }

                    if (muc.TuGiaTri.Value == muc.DenGiaTri.Value && (!muc.BaoGomTuGiaTri || !muc.BaoGomDenGiaTri))
                    {
                        return "Khoảng có cùng giá trị đầu/cuối phải bao gồm cả hai đầu mút.";
                    }
                }
            }

            for (var i = 0; i < activeMucs.Count; i++)
            {
                for (var j = i + 1; j < activeMucs.Count; j++)
                {
                    if (IsOverlap(activeMucs[i], activeMucs[j]))
                    {
                        return "Các mức điểm đang bị chồng lấn khoảng giá trị. Vui lòng kiểm tra lại.";
                    }
                }
            }

            if (request.LoaiTieuChi == "CHAT_LUONG" &&
                activeMucs.Any(x => (x.TuGiaTri ?? 0) < 0 || (x.DenGiaTri ?? 0) < 0))
            {
                return "Tiêu chí chất lượng không được có giá trị âm.";
            }

            return null;
        }

        private static bool IsOverlap(DanhMucTieuChiDiemMuc a, DanhMucTieuChiDiemMuc b)
        {
            var aStart = a.TuGiaTri ?? decimal.MinValue;
            var aEnd = a.DenGiaTri ?? decimal.MaxValue;
            var bStart = b.TuGiaTri ?? decimal.MinValue;
            var bEnd = b.DenGiaTri ?? decimal.MaxValue;

            if (aEnd < bStart || bEnd < aStart)
            {
                return false;
            }

            if (aEnd == bStart)
            {
                return a.BaoGomDenGiaTri && b.BaoGomTuGiaTri;
            }

            if (bEnd == aStart)
            {
                return b.BaoGomDenGiaTri && a.BaoGomTuGiaTri;
            }

            return true;
        }
    }
}
