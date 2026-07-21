using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.TraCuuBaoCao;

namespace UI.Controllers.Admin.TraCuuBaoCao
{
    public class TraCuuController : BaseController
    {
        private readonly ITraCuuService _traCuuService;

        public TraCuuController(ITraCuuService traCuuService)
        {
            _traCuuService = traCuuService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["DanhMucKinhDoanhNganhDG"] = await _traCuuService.GetDanhMucKinhDoanhNganhAsync("DG");
            ViewData["DanhMucKinhDoanhNgheDG"] = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("DG");
            ViewData["DanhMucKinhDoanhNganhKKG"] = await _traCuuService.GetDanhMucKinhDoanhNganhAsync("KKG");
            ViewData["DanhMucKinhDoanhNgheKKG"] = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("KKG");
            ViewData["GiaThiTruongDanhMuc"] = await _traCuuService.GetGiaThiTruongDanhMucAsync();
            ViewData["ThamDinhGiaHangHoa"] = await _traCuuService.GetThamDinhGiaDanhMucHangHoaAsync();
            ViewData["Title"] = "Tra cứu hồ sơ";
            return View("Views/Admin/TraCuuBaoCao/TraCuu/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> DinhGia(string MaNghe, DateTime? TuNgay, DateTime? DenNgay, string SoQd, string MoTa, string MaHoSo)
        {
            if (string.IsNullOrEmpty(MaNghe))
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchDinhGiaCtAsync(MaNghe, TuNgay, DenNgay, SoQd, MoTa, MaHoSo);
            var categories = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("DG");
            var currentCategory = categories.Find(x => x.MaNghe == MaNghe);

            ViewData["MaNghe"] = MaNghe;
            ViewData["TenNghe"] = currentCategory?.TenNghe ?? "Tất cả";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoQd"] = SoQd;
            ViewData["MoTa"] = MoTa;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["Title"] = "Tra cứu hồ sơ Định giá";
            ViewData["ParentMap"] = results.Item2;

            return View("Views/Admin/TraCuuBaoCao/TraCuu/DanhSach/DinhGia.cshtml", results.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> KeKhaiDangKyGia(string MaNghe, DateTime? TuNgay, DateTime? DenNgay, string SoQd, string MoTa, string MaHoSo)
        {
            if (string.IsNullOrEmpty(MaNghe))
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchKeKhaiDangKyGiaCtAsync(MaNghe, TuNgay, DenNgay, SoQd, MoTa, MaHoSo);
            var categories = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("KKG");
            var currentCategory = categories.Find(x => x.MaNghe == MaNghe);

            ViewData["MaNghe"] = MaNghe;
            ViewData["TenNghe"] = currentCategory?.TenNghe ?? "Tất cả";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoQd"] = SoQd;
            ViewData["MoTa"] = MoTa;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["Title"] = "Tra cứu hồ sơ Kê khai đăng ký giá";
            ViewData["ParentMap"] = results.Item2;

            return View("Views/Admin/TraCuuBaoCao/TraCuu/DanhSach/KeKhaiDangKyGia.cshtml", results.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> GiaThiTruong(Guid thongTuId, Guid Matt, DateTime? TuNgay, DateTime? DenNgay, string SoQd, string MoTa, string MaHoSo)
        {
            var targetId = thongTuId != Guid.Empty ? thongTuId : Matt;
            if (targetId == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchGiaThiTruongCtAsync(targetId, TuNgay, DenNgay, SoQd, MoTa, MaHoSo);
            var categories = await _traCuuService.GetGiaThiTruongDanhMucAsync();
            var currentCategory = categories.Find(x => x.Id == targetId);

            ViewData["ThongTuId"] = targetId;
            ViewData["TenTT"] = currentCategory?.TenTT ?? "Tất cả";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoQd"] = SoQd;
            ViewData["MoTa"] = MoTa;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["Title"] = "Tra cứu hồ sơ Giá thị trường";
            ViewData["ParentMap"] = results.Item2;

            return View("Views/Admin/TraCuuBaoCao/TraCuu/DanhSach/GiaThiTruong.cshtml", results.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> PrintDinhGia(string MaNghe, DateTime? TuNgay, DateTime? DenNgay, string SoQd, string MoTa, string MaHoSo)
        {
            if (string.IsNullOrEmpty(MaNghe))
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchDinhGiaCtAsync(MaNghe, TuNgay, DenNgay, SoQd, MoTa, MaHoSo);
            var categories = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("DG");
            var currentCategory = categories.Find(x => x.MaNghe == MaNghe);

            ViewData["TenNghe"] = currentCategory?.TenNghe ?? "";
            ViewData["Title"] = "Kết quả tra cứu hồ sơ Định giá";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoQd"] = SoQd;
            ViewData["MoTa"] = MoTa;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["ParentMap"] = results.Item2;
            return View("Views/Admin/TraCuuBaoCao/TraCuu/InTrang/PrintDinhGia.cshtml", results.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> PrintKeKhaiDangKyGia(string MaNghe, DateTime? TuNgay, DateTime? DenNgay, string SoQd, string MoTa, string MaHoSo)
        {
            if (string.IsNullOrEmpty(MaNghe))
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchKeKhaiDangKyGiaCtAsync(MaNghe, TuNgay, DenNgay, SoQd, MoTa, MaHoSo);
            var categories = await _traCuuService.GetDanhMucKinhDoanhNgheAsync("KKG");
            var currentCategory = categories.Find(x => x.MaNghe == MaNghe);

            ViewData["TenNghe"] = currentCategory?.TenNghe ?? "";
            ViewData["Title"] = "Kết quả tra cứu hồ sơ Kê khai đăng ký giá";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoQd"] = SoQd;
            ViewData["MoTa"] = MoTa;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["ParentMap"] = results.Item2;
            return View("Views/Admin/TraCuuBaoCao/TraCuu/InTrang/PrintKeKhaiDangKyGia.cshtml", results.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> PrintGiaThiTruong(Guid thongTuId, Guid Matt, DateTime? TuNgay, DateTime? DenNgay, string SoQd, string MoTa, string MaHoSo)
        {
            var targetId = thongTuId != Guid.Empty ? thongTuId : Matt;
            if (targetId == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchGiaThiTruongCtAsync(targetId, TuNgay, DenNgay, SoQd, MoTa, MaHoSo);
            var categories = await _traCuuService.GetGiaThiTruongDanhMucAsync();
            var currentCategory = categories.Find(x => x.Id == targetId);

            ViewData["TenTT"] = currentCategory?.TenTT ?? "";
            ViewData["Title"] = "Kết quả tra cứu hồ sơ Giá thị trường";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoQd"] = SoQd;
            ViewData["MoTa"] = MoTa;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["ParentMap"] = results.Item2;
            return View("Views/Admin/TraCuuBaoCao/TraCuu/InTrang/PrintGiaThiTruong.cshtml", results.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> ThamDinhGia(Guid hangHoaId, DateTime? TuNgay, DateTime? DenNgay, string SoTbKl, string DvYeuCau, string MaHoSo)
        {
            if (hangHoaId == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchThamDinhGiaCtAsync(hangHoaId, TuNgay, DenNgay, SoTbKl, DvYeuCau, MaHoSo);
            var categories = await _traCuuService.GetThamDinhGiaDanhMucHangHoaAsync();
            var currentCategory = categories.Find(x => x.Id == hangHoaId);

            ViewData["HangHoaId"] = hangHoaId;
            ViewData["TenDanhMucHangHoa"] = currentCategory?.TenDanhMucHangHoa ?? "Tất cả";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoTbKl"] = SoTbKl;
            ViewData["DvYeuCau"] = DvYeuCau;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["Title"] = "Tra cứu hồ sơ Thẩm định giá";
            ViewData["ParentMap"] = results.Item2;

            return View("Views/Admin/TraCuuBaoCao/TraCuu/DanhSach/ThamDinhGia.cshtml", results.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> PrintThamDinhGia(Guid hangHoaId, DateTime? TuNgay, DateTime? DenNgay, string SoTbKl, string DvYeuCau, string MaHoSo)
        {
            if (hangHoaId == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            var results = await _traCuuService.SearchThamDinhGiaCtAsync(hangHoaId, TuNgay, DenNgay, SoTbKl, DvYeuCau, MaHoSo);
            var categories = await _traCuuService.GetThamDinhGiaDanhMucHangHoaAsync();
            var currentCategory = categories.Find(x => x.Id == hangHoaId);

            ViewData["TenDanhMucHangHoa"] = currentCategory?.TenDanhMucHangHoa ?? "";
            ViewData["Title"] = "Kết quả tra cứu hồ sơ Thẩm định giá";
            ViewData["TuNgay"] = TuNgay?.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = DenNgay?.ToString("yyyy-MM-dd");
            ViewData["SoTbKl"] = SoTbKl;
            ViewData["DvYeuCau"] = DvYeuCau;
            ViewData["MaHoSo"] = MaHoSo;
            ViewData["ParentMap"] = results.Item2;
            return View("Views/Admin/TraCuuBaoCao/TraCuu/InTrang/PrintThamDinhGia.cshtml", results.Item1);
        }
    }
}
