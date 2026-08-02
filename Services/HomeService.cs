using DataAccess;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;
using Services.Model;
using Services.Systems;

namespace Services
{
    public interface IHomeService
    {
        Task<CommonResponse> UpdateThemeAsync(User request);
        Task<bool> CheckCurrentPassword(string currentPassword);
        Task<CommonResponse> UpdatePasswordAsync(string password);
        Task<CommonResponse> UpdateUserInfoAsync(User request);
        Task<CommonResponse> GetHomeDashboardDataAsync();
        List<string> GetDashboardImages();
        Task<CommonResponse> GetTongHopHoSoStatsAsync();
    }

    public class HomeService(ApplicationDbContext dbContext, IAuthService authService, IWebHostEnvironment env) : IHomeService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;
        private readonly IWebHostEnvironment _env = env;

        public async Task<bool> CheckCurrentPassword(string currentPassword)
        {
            var userInfo = _authService.GetUserInfo();
            if (userInfo == null)
            {
                return false;
            }

            var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
            return model != null && BCrypt.Net.BCrypt.Verify(currentPassword, model.Password);
        }

        public async Task<CommonResponse> UpdatePasswordAsync(string password)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                if (userInfo == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin dữ liệu");
                }

                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin dữ liệu");
                }

                model.Password = Helper.BCryptHash(password);
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success");
            }
            catch
            {
                return new CommonResponse("error", "Đã xảy ra lỗi. Vui lòng thử lại sau!");
            }
        }

        public async Task<CommonResponse> UpdateThemeAsync(User request)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                if (userInfo == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin dữ liệu");
                }

                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin dữ liệu");
                }

                model.Theme = request.Theme;
                model.Menu = request.Menu;
                model.Content = request.Content;
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success");
            }
            catch
            {
                return new CommonResponse("error", "Đã xảy ra lỗi. Vui lòng thử lại sau!");
            }
        }

        public async Task<CommonResponse> UpdateUserInfoAsync(User request)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                if (userInfo == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin dữ liệu");
                }

                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin dữ liệu");
                }

                model.TenDonViBaoCao = request.TenDonViBaoCao;
                model.TenDonViChuQuanBaoCao = request.TenDonViChuQuanBaoCao;
                model.FirstLogin = request.FirstLogin;
                model.DiaDanh = request.DiaDanh;
                model.ChucDanhKy = request.ChucDanhKy;
                model.HoTenNguoiKy = request.HoTenNguoiKy;
                model.KyHieuDonVi = request.KyHieuDonVi;
                model.AgentId = request.AgentId;
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success");
            }
            catch
            {
                return new CommonResponse("error", "Đã xảy ra lỗi. Vui lòng thử lại sau!");
            }
        }

        public async Task<CommonResponse> GetHomeDashboardDataAsync()
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                var isSSA = userInfo?.SSA ?? false;
                var donViId = userInfo?.DanhMucDonViId ?? Guid.Empty;
                var currentYear = DateTime.Now.Year;
                var result = new HomeResponseDto();
                const int takeData = 5;

                result.ThuTucHanhChinhs = await _dbContext.ThuTucHanhChinhs.AsNoTracking()
                    .OrderByDescending(x => x.NgayQuyetDinh)
                    .Take(takeData)
                    .ToListAsync();

                result.VanBanPhapLuats = await _dbContext.AttachedFiles.AsNoTracking()
                    .Where(x => (userInfo == null ? x.Public : true) && x.TableName == "VanBanPhapLuat")
                    .OrderByDescending(x => x.NgayBanHanh)
                    .Take(takeData)
                    .ToListAsync();

                var hoSoQuery = _dbContext.HoSoVanBans.AsNoTracking().AsQueryable();
                var keHoachQuery = _dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking().AsQueryable();
                var chiTietThiHanhQuery = _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AsNoTracking()
                    .Join(_dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking(), ct => ct.NhiemVuId, nv => nv.Id, (ct, nv) => new { ct, nv })
                    .Join(_dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking(), x => x.nv.KeHoachId, kh => kh.Id, (x, kh) => new { x.ct, x.nv, kh })
                    .AsQueryable();

                if (!isSSA && donViId != Guid.Empty)
                {
                    hoSoQuery = hoSoQuery.Where(x => x.DonViSoanThaoId == donViId);
                    keHoachQuery = keHoachQuery.Where(x => x.DonViChuTriId == donViId);
                    chiTietThiHanhQuery = chiTietThiHanhQuery.Where(x => x.kh.DonViChuTriId == donViId || x.ct.DonViThucHienId == donViId);
                }

                var tongHoSo = await hoSoQuery.CountAsync();
                var hoSoDangXuLy = await hoSoQuery.CountAsync(x => !x.NgayHoanThanh.HasValue);
                var hoSoDaHoanThanh = await hoSoQuery.CountAsync(x => x.NgayHoanThanh.HasValue);
                var hoSoDaBanHanh = await hoSoQuery.CountAsync(x => x.TrangThaiBanHanh == "DA_BAN_HANH");
                var tongKeHoach = await keHoachQuery.CountAsync();

                var thiHanhRows = await chiTietThiHanhQuery.ToListAsync();
                var chiTietIds = thiHanhRows.Select(x => x.ct.Id).ToList();
                var tienDoCounts = await _dbContext.ThiHanhPhapLuatTienDos.AsNoTracking()
                    .Where(x => chiTietIds.Contains(x.ChiTietNhiemVuId))
                    .GroupBy(x => x.ChiTietNhiemVuId)
                    .Select(g => new { ChiTietNhiemVuId = g.Key, Count = g.Count() })
                    .ToListAsync();

                var quaHan = thiHanhRows.Count(x => x.ct.TrangThai != "HOAN_THANH" && x.ct.HanHoanThanh.HasValue && x.ct.HanHoanThanh.Value.Date < DateTime.Today);
                var chamTienDo = thiHanhRows.Count(x => x.ct.TrangThai != "HOAN_THANH" && x.ct.HanHoanThanh.HasValue && x.ct.HanHoanThanh.Value.Date >= DateTime.Today && x.ct.HanHoanThanh.Value.Date <= DateTime.Today.AddDays(3));
                var chuaNhapLieu = thiHanhRows.Count(x => (tienDoCounts.FirstOrDefault(t => t.ChiTietNhiemVuId == x.ct.Id)?.Count ?? 0) == 0);

                result.Summary = new DashboardSummaryDto
                {
                    TongHoSoVanBan = tongHoSo,
                    HoSoDangXuLy = hoSoDangXuLy,
                    HoSoDaHoanThanh = hoSoDaHoanThanh,
                    HoSoDaBanHanh = hoSoDaBanHanh,
                    TongKeHoachThiHanh = tongKeHoach,
                    NhiemVuThiHanhQuaHan = quaHan,
                    NhiemVuThiHanhChamTienDo = chamTienDo,
                    NhiemVuThiHanhChuaNhapLieu = chuaNhapLieu,
                    NamThongKe = currentYear
                };

                var hoSoByYear = await hoSoQuery
                    .Where(x => x.NgayTaoHoSo.Year == currentYear
                        || (x.NgayHoanThanh.HasValue && x.NgayHoanThanh.Value.Year == currentYear)
                        || (x.NgayBanHanh.HasValue && x.NgayBanHanh.Value.Year == currentYear))
                    .ToListAsync();

                result.HoSoChart = new DashboardHoSoChartDto
                {
                    Year = currentYear,
                    Categories = Enumerable.Range(1, 12).Select(x => $"Tháng {x}").ToList(),
                    HoSoTaoMoiTheoThang = Enumerable.Range(1, 12).Select(m => hoSoByYear.Count(x => x.NgayTaoHoSo.Year == currentYear && x.NgayTaoHoSo.Month == m)).ToList(),
                    HoSoHoanThanhTheoThang = Enumerable.Range(1, 12).Select(m => hoSoByYear.Count(x => x.NgayHoanThanh.HasValue && x.NgayHoanThanh.Value.Year == currentYear && x.NgayHoanThanh.Value.Month == m)).ToList(),
                    HoSoBanHanhTheoThang = Enumerable.Range(1, 12).Select(m => hoSoByYear.Count(x => x.NgayBanHanh.HasValue && x.NgayBanHanh.Value.Year == currentYear && x.NgayBanHanh.Value.Month == m)).ToList()
                };

                var xepLoaiData = await hoSoQuery
                    .Where(x => !string.IsNullOrEmpty(x.XepLoaiDanhGia))
                    .GroupBy(x => x.XepLoaiDanhGia!)
                    .Select(g => new { Label = g.Key, Value = g.Count() })
                    .OrderByDescending(x => x.Value)
                    .ToListAsync();

                result.HoSoChart.XepLoaiLabels = xepLoaiData.Select(x => x.Label).ToList();
                result.HoSoChart.XepLoaiValues = xepLoaiData.Select(x => x.Value).ToList();

                var buocMap = await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.TenBuoc);
                var hoSoByStep = await hoSoQuery
                    .Where(x => x.BuocHienTaiId.HasValue)
                    .GroupBy(x => x.BuocHienTaiId!.Value)
                    .Select(g => new { BuocId = g.Key, SoLuong = g.Count() })
                    .OrderByDescending(x => x.SoLuong)
                    .Take(8)
                    .ToListAsync();

                var buocCodeMap = await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.MaBuoc);
                result.HoSoByStep = hoSoByStep.Select(x => new DashboardStepItemDto
                {
                    MaBuoc = buocCodeMap.TryGetValue(x.BuocId, out var maBuoc) ? maBuoc : string.Empty,
                    TenBuoc = buocMap.TryGetValue(x.BuocId, out var tenBuoc) ? tenBuoc : "Chưa xác định",
                    SoLuong = x.SoLuong
                }).ToList();

                var donViMap = await _dbContext.DanhMucDonVis.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.TenDonVi);
                var hoSoByDonVi = await hoSoQuery
                    .GroupBy(x => x.DonViSoanThaoId)
                    .Select(g => new { DonViId = g.Key, SoLuong = g.Count() })
                    .OrderByDescending(x => x.SoLuong)
                    .Take(8)
                    .ToListAsync();

                result.HoSoByDonVi = hoSoByDonVi.Select(x => new DashboardDonViItemDto
                {
                    DonViId = x.DonViId,
                    TenDonVi = donViMap.TryGetValue(x.DonViId, out var tenDonVi) ? tenDonVi : "Chưa xác định",
                    SoLuongHoSo = x.SoLuong
                }).ToList();

                var trangThaiThiHanh = thiHanhRows
                    .GroupBy(x => x.ct.TrangThai)
                    .Select(g => new { Label = g.Key, Value = g.Count() })
                    .OrderByDescending(x => x.Value)
                    .ToList();

                result.ThiHanhChart = new DashboardThiHanhChartDto
                {
                    TrangThaiLabels = trangThaiThiHanh.Select(x => x.Label).ToList(),
                    TrangThaiValues = trangThaiThiHanh.Select(x => x.Value).ToList(),
                    CanhBaoLabels = new List<string> { "Quá hạn", "Chậm tiến độ", "Chưa nhập liệu" },
                    CanhBaoValues = new List<int> { quaHan, chamTienDo, chuaNhapLieu }
                };

                result.ThiHanhCanhBao = new List<DashboardCanhBaoItemDto>
                {
                    new() { MaCanhBao = "QUA_HAN", TieuChi = "Quá hạn", SoLuong = quaHan },
                    new() { MaCanhBao = "CHAM_TIEN_DO", TieuChi = "Chậm tiến độ", SoLuong = chamTienDo },
                    new() { MaCanhBao = "CHUA_NHAP_LIEU", TieuChi = "Chưa nhập liệu", SoLuong = chuaNhapLieu }
                };

                return new CommonResponse("success", "Truy cập dữ liệu thành công", result);
            }
            catch
            {
                return new CommonResponse("error", "Đã xảy ra lỗi. Vui lòng thử lại sau!");
            }
        }

        public List<string> GetDashboardImages()
        {
            const string baseWebPath = "/assets/media/dashboard/";
            var folderPath = Path.Combine(_env.WebRootPath, baseWebPath.Trim('/').Replace('/', Path.DirectorySeparatorChar));
            var urlPath = baseWebPath.EndsWith("/") ? baseWebPath : baseWebPath + "/";

            if (!Directory.Exists(folderPath))
            {
                return new List<string>();
            }

            return Directory.GetFiles(folderPath)
                .Select(Path.GetFileName)
                .Where(x => x != null)
                .Select(x => urlPath + x!)
                .ToList();
        }

        public async Task<CommonResponse> GetTongHopHoSoStatsAsync()
        {
            try
            {
                var response = await GetHomeDashboardDataAsync();
                if (response.Status == "error")
                {
                    return response;
                }

                var data = response.Data as HomeResponseDto;
                return new CommonResponse("success", "Thành công", new
                {
                    Year = data?.Summary.NamThongKe ?? DateTime.Now.Year,
                    MonthlyDG = data?.HoSoChart.HoSoTaoMoiTheoThang ?? new List<int>(),
                    MonthlyKK = data?.HoSoChart.HoSoHoanThanhTheoThang ?? new List<int>(),
                    MonthlyGTT = data?.HoSoChart.HoSoBanHanhTheoThang ?? new List<int>(),
                    MonthlyTotal = data?.ThiHanhChart.CanhBaoValues ?? new List<int>()
                });
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi tổng hợp dữ liệu: " + ex.Message);
            }
        }
    }
}
