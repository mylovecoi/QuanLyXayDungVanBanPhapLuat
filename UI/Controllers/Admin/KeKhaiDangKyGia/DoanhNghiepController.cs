using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.KeKhaiDangKyGia;
using Services.Systems;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.KeKhaiDangKyGia
{
    [SetViewDataFilter]
    public class DoanhNghiepController(
        IDoanhNghiepService doanhNghiepService,
        ISystemInfoService systemInfoService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IDoanhNghiepService _doanhNghiepService = doanhNghiepService;
        private readonly ISystemInfoService _systemInfoService = systemInfoService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/KeKhaiDangKyGia/DoanhNghiep/DangKy/{viewName}";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("DoanhNghiep/ThongTin")]
        public async Task<IActionResult> ThongTin()
        {
            var dsDoanhNghiep = await _doanhNghiepService.GetListDoanhNghiepAsync();
            ViewData["Title"] = "Thông tin doanh nghiệp";
            ViewData["DanhSachDoanhNghiep"] = dsDoanhNghiep;
            return View("../Admin/KeKhaiDangKyGia/DoanhNghiep/DanhSach/Index");
        }

        [HttpPost("DoanhNghiep/ThongTin/GetDetail")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var dn = await _doanhNghiepService.GetDoanhNghiepByIdAsync(id);
            if (dn == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy thông tin doanh nghiệp!" });
            }

            var lvkdList = await _doanhNghiepService.GetLvkdByDoanhNghiepIdAsync(dn.Id);
            ViewData["DoanhNghiepLvKd"] = lvkdList;
            ViewData["DanhMucKinhDoanh"] = await _dbContext.DanhMucKinhDoanhs.Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "KKG").OrderBy(t => t.STTSapXep).ToListAsync();
            ViewData["DanhMucDonVi"] = await _dbContext.DanhMucDonVis.OrderBy(t => t.STTSapXep).ToListAsync();

            string html = StaticViewRenderHelper.RenderRazorViewToString(this, "../Admin/KeKhaiDangKyGia/DoanhNghiep/DanhSach/Detail", dn);
            return Json(new { status = "success", message = html });
        }

        [HttpGet("DoanhNghiep/DangKy")]
        [AllowAnonymous]
        public async Task<IActionResult> DangKy()
        {
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();
            ViewData["Title"] = "Đăng ký tài khoản doanh nghiệp";
            ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
            ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";
            ViewData["Train"] = systemInfo != null ? systemInfo.Train : false;

            // Load list of receiving units
            ViewData["DanhMucDonVi"] = await _dbContext.DanhMucDonVis.OrderBy(t => t.STTSapXep).ToListAsync();
            
            // Load sectors and professions filtered by LoaiGia = KKG
            ViewData["DanhMucKinhDoanhNganh"] = await _dbContext.DanhMucKinhDoanhs.Where(t => (t.Level == 0 || t.PhanLoai == "Group") && t.LoaiGia == "KKG").OrderBy(t => t.STTSapXep).ToListAsync();
            ViewData["DanhMucKinhDoanhNghe"] = await _dbContext.DanhMucKinhDoanhs.Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "KKG").OrderBy(t => t.STTSapXep).ToListAsync();

            return View(ViewPath("Index"), new DoanhNghiep());
        }

        [HttpPost("DoanhNghiep/DangKy/Store")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DoanhNghiep model, string username, string password)
        {
            var result = await _doanhNghiepService.CompleteRegistrationAsync(model, username, password);
            if (result.Status == "success")
            {
                return Json(new { status = "success" });
            }

            return Json(new { status = "error", message = result.Message ?? "Đăng ký không thành công!" });
        }

        [HttpGet("DoanhNghiep/DangKy/Success")]
        [AllowAnonymous]
        public async Task<IActionResult> Success()
        {
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();
            ViewData["Title"] = "Đăng ký thành công";
            ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
            ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";
            ViewData["Train"] = systemInfo != null ? systemInfo.Train : false;
            ViewData["Message"] = "Đăng ký tài khoản thành công! Vui lòng chờ kích hoạt tài khoản.";
            ViewData["Controller"] = "Auth";
            ViewData["Action"] = "Login";
            return View("Success");
        }
    }
}
