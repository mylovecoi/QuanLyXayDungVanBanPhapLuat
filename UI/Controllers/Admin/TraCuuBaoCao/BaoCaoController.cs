using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.TraCuuBaoCao;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace UI.Controllers.Admin.TraCuuBaoCao
{
    public class BaoCaoController : BaseController
    {
        private readonly ITraCuuService _traCuuService;
        private readonly IBaoCaoService _baoCaoService;
        private readonly ApplicationDbContext _dbContext;

        public BaoCaoController(ITraCuuService traCuuService, IBaoCaoService baoCaoService, ApplicationDbContext dbContext)
        {
            _traCuuService = traCuuService;
            _baoCaoService = baoCaoService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["DanhMucKinhDoanhNganhDG"] = await _traCuuService.GetDanhMucKinhDoanhNganhAsync("DG");
            ViewData["DanhMucKinhDoanhNgheDG"] = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("DG");
            ViewData["DanhMucKinhDoanhNganhKKG"] = await _traCuuService.GetDanhMucKinhDoanhNganhAsync("KKG");
            ViewData["DanhMucKinhDoanhNgheKKG"] = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("KKG");
            ViewData["GiaThiTruongDanhMuc"] = await _traCuuService.GetGiaThiTruongDanhMucAsync();
            ViewData["Title"] = "Báo cáo hồ sơ";
            return View("Views/Admin/TraCuuBaoCao/BaoCao/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> DinhGia(string MaNghe, DateTime? TuNgay, DateTime? DenNgay)
        {
            if (string.IsNullOrEmpty(MaNghe))
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _baoCaoService.SearchDinhGiaReportAsync(MaNghe, TuNgay, DenNgay);
            var categories = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("DG");
            var currentCategory = categories.Find(x => x.MaNghe == MaNghe);

            ViewData["MaNghe"] = MaNghe;
            ViewData["TenNghe"] = currentCategory?.TenNghe ?? "Tất cả";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["Title"] = "Báo cáo hồ sơ Định giá";

            return View("Views/Admin/TraCuuBaoCao/BaoCao/DanhSach/DinhGia.cshtml", results);
        }

        [HttpGet]
        public async Task<IActionResult> KeKhaiDangKyGia(string MaNghe, DateTime? TuNgay, DateTime? DenNgay)
        {
            if (string.IsNullOrEmpty(MaNghe))
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _baoCaoService.SearchKeKhaiDangKyGiaReportAsync(MaNghe, TuNgay, DenNgay);
            var categories = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("KKG");
            var currentCategory = categories.Find(x => x.MaNghe == MaNghe);

            ViewData["MaNghe"] = MaNghe;
            ViewData["TenNghe"] = currentCategory?.TenNghe ?? "Tất cả";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["Title"] = "Báo cáo hồ sơ Kê khai đăng ký giá";

            return View("Views/Admin/TraCuuBaoCao/BaoCao/DanhSach/KeKhaiDangKyGia.cshtml", results);
        }

        [HttpGet]
        public async Task<IActionResult> GiaThiTruong(Guid thongTuId, Guid Matt, DateTime? TuNgay, DateTime? DenNgay)
        {
            var targetId = thongTuId != Guid.Empty ? thongTuId : Matt;
            if (targetId == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _baoCaoService.SearchGiaThiTruongReportAsync(targetId, TuNgay, DenNgay);
            var categories = await _traCuuService.GetGiaThiTruongDanhMucAsync();
            var currentCategory = categories.Find(x => x.Id == targetId);

            ViewData["ThongTuId"] = targetId;
            ViewData["TenTT"] = currentCategory?.TenTT ?? "Tất cả";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["Title"] = "Báo cáo hồ sơ Giá thị trường";

            return View("Views/Admin/TraCuuBaoCao/BaoCao/DanhSach/GiaThiTruong.cshtml", results);
        }

        [HttpGet]
        public async Task<IActionResult> ThamDinhGia(DateTime? TuNgay, DateTime? DenNgay)
        {
            var results = await _baoCaoService.SearchThamDinhGiaReportAsync(TuNgay, DenNgay);

            ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["Title"] = "Báo cáo hồ sơ Thẩm định giá";

            return View("Views/Admin/TraCuuBaoCao/BaoCao/DanhSach/ThamDinhGia.cshtml", results);
        }
    }
}
