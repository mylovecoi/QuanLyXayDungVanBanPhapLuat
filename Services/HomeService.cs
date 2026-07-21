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
        Task<bool> CheckCurrentPassword(string current_password);
        Task<CommonResponse> UpdatePasswordAsync(string password);
        Task<CommonResponse> UpdateUserInfoAsync(User request);
        Task<CommonResponse> GetHomeDashboardDataAsync();
        List<string> GetDashboardImages();
        Task<CommonResponse> GetTongHopHoSoStatsAsync();
    }
    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;

        public HomeService(ApplicationDbContext dbContext, IAuthService authService, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _authService = authService;
            _env = env;
        }
        public async Task<bool> CheckCurrentPassword(string current_password)
        {
            var userInfo = _authService.GetUserInfo();
            if (userInfo != null)
            {
                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
                if (model != null && BCrypt.Net.BCrypt.Verify(current_password, model.Password))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<CommonResponse> UpdatePasswordAsync(string password)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                if (userInfo == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                }
                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                }
                model.Password = Helper.BCryptHash(password);
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> UpdateThemeAsync(User request)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                if (userInfo == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                }
                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                }
                model.Theme = request.Theme;
                model.Menu = request.Menu;
                model.Content = request.Content;
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> UpdateUserInfoAsync(User request)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                if (userInfo == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                }
                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == userInfo.Username);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
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
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> GetHomeDashboardDataAsync()
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                var donViId = Guid.Empty;
                if (userInfo != null)
                {
                    donViId = userInfo.DanhMucDonViId;
                }
                var result = new HomeResponseDto();
                const int intTakeData = 5;
                result.ThuTucHanhChinhs = await _dbContext.ThuTucHanhChinhs.AsNoTracking().OrderByDescending(x => x.NgayQuyetDinh).Take(intTakeData).ToListAsync();
                result.VanBanPhapLuats = await _dbContext.AttachedFiles.AsNoTracking().Where(x => (userInfo == null ? x.Public == true : true) && x.TableName == "VanBanPhapLuat").OrderByDescending(x => x.NgayBanHanh).Take(intTakeData).ToListAsync();
                return new("success", "Truy cập dữ liệu thành công", result);
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public List<string> GetDashboardImages()
        {
            string baseWebPath = "/assets/media/dashboard/";

            string folderPath = Path.Combine(
                _env.WebRootPath,
                baseWebPath.Trim('/').Replace('/', Path.DirectorySeparatorChar)
            );

            string urlPath = baseWebPath.EndsWith("/") ? baseWebPath : baseWebPath + "/";

            if (!Directory.Exists(folderPath))
                return new List<string>();

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
                var userInfo = _authService.GetUserInfo();
                Guid? donViId = userInfo?.DanhMucDonViId;
                bool isSSA = userInfo?.SSA ?? false;

                var queryDG = _dbContext.DinhGias.AsNoTracking().Where(x => x.TrangThai != "CXD");
                var queryKK = _dbContext.KeKhaiDangKyGias.AsNoTracking().Where(x => x.TrangThai != "CXD");
                var queryGTT = _dbContext.GiaThiTruongs.AsNoTracking().Where(x => x.TrangThai != "CXD");

                if (donViId != null && donViId != Guid.Empty && !isSSA)
                {
                    var userDonVi = await _dbContext.DanhMucDonVis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == donViId);
                    if (userDonVi != null && userDonVi.Level > 0)
                    {
                        queryDG = queryDG.Where(x => x.DonViQuanLyId == donViId.Value);
                        queryKK = queryKK.Where(x => x.DonViQuanLyId == donViId.Value);
                        queryGTT = queryGTT.Where(x => x.DonViQuanLyId == donViId.Value);
                    }
                }

                int currentYear = DateTime.Now.Year;
                int maxYear = 0;
                if (await queryDG.AnyAsync()) maxYear = Math.Max(maxYear, await queryDG.MaxAsync(x => x.ThoiDiem.Year));
                if (await queryKK.AnyAsync()) maxYear = Math.Max(maxYear, await queryKK.MaxAsync(x => x.ThoiDiem.Year));
                if (await queryGTT.AnyAsync()) maxYear = Math.Max(maxYear, await queryGTT.MaxAsync(x => x.Thoidiem.Year));

                if (maxYear > 0)
                {
                    currentYear = maxYear;
                }

                var listDG = await queryDG.Where(x => x.ThoiDiem.Year == currentYear).ToListAsync();
                var listKK = await queryKK.Where(x => x.ThoiDiem.Year == currentYear).ToListAsync();
                var listGTT = await queryGTT.Where(x => x.Thoidiem.Year == currentYear).ToListAsync();

                var monthlyTotal = new List<int>();
                var monthlyDG = new List<int>();
                var monthlyKK = new List<int>();
                var monthlyGTT = new List<int>();

                for (int i = 1; i <= 12; i++)
                {
                    int dgCount = listDG.Count(x => x.ThoiDiem.Month == i);
                    int kkCount = listKK.Count(x => x.ThoiDiem.Month == i);
                    int gttCount = listGTT.Count(x => x.Thoidiem.Month == i);

                    monthlyDG.Add(dgCount);
                    monthlyKK.Add(kkCount);
                    monthlyGTT.Add(gttCount);
                    monthlyTotal.Add(dgCount + kkCount + gttCount);
                }

                var result = new
                {
                    Year = currentYear,
                    MonthlyDG = monthlyDG,
                    MonthlyKK = monthlyKK,
                    MonthlyGTT = monthlyGTT,
                    MonthlyTotal = monthlyTotal
                };

                return new CommonResponse("success", "Thành công", result);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi tổng hợp dữ liệu: " + ex.Message);
            }
        }
    }
}
