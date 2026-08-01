using Microsoft.AspNetCore.Mvc;
using UI.Security;

namespace UI.Controllers.Admin.Systems
{
    [SetViewDataFilter]
    public class DangPhatTrienController : Controller
    {
        [HttpGet("Systems/DangPhatTrien")]
        [AuthorizeAction("Index")]
        public IActionResult Index() => BuildView("Chức năng đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/TrangThai")]
        [AuthorizeAction("Index")]
        public IActionResult TrangThai() => BuildView("Chức năng Danh sách trạng thái đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/DangKyXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult DangKyXayDung() => BuildView("Chức năng Đăng ký xây dựng đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/XetDuyetDangKy")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetDangKy() => BuildView("Chức năng Xét duyệt đăng ký đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/PheDuyetDangKy")]
        [AuthorizeAction("Index")]
        public IActionResult PheDuyetDangKy() => BuildView("Chức năng Phê duyệt đăng ký đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/QuyTrinhSoanThao")]
        [AuthorizeAction("Index")]
        public IActionResult QuyTrinhSoanThao() => RedirectToAction("Index", "QuyTrinhSoanThao");

        [HttpGet("Systems/DangPhatTrien/XayDungVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XayDungVanBan() => BuildView("Chức năng Xây dựng văn bản đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/GopYDanhGia")]
        [AuthorizeAction("Index")]
        public IActionResult GopYDanhGia() => RedirectToAction("Index", "LayYKienVanBan");

        [HttpGet("Systems/DangPhatTrien/XetDuyetSoanThao")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetSoanThao() => RedirectToAction("Index", "DuThaoVanBan");

        [HttpGet("Systems/DangPhatTrien/GiaHanXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult GiaHanXayDung() => BuildView("Chức năng Gia hạn thời gian xây dựng đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/DanhSachGiaHanXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult DanhSachGiaHanXayDung() => BuildView("Chức năng Danh sách văn bản gia hạn đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/XetDuyetVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetVanBan() => RedirectToAction("Index", "XetDuyetDuThao");

        [HttpGet("Systems/DangPhatTrien/PheDuyetVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult PheDuyetVanBan() => BuildView("Chức năng Phê duyệt văn bản đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/XetDuyetBanHanhVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetBanHanhVanBan() => BuildView("Chức năng Xét duyệt ban hành văn bản đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/BanHanhVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult BanHanhVanBan() => BuildView("Chức năng Ban hành văn bản đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/DanhSachKeHoach")]
        [AuthorizeAction("Index")]
        public IActionResult DanhSachKeHoach() => BuildView("Chức năng Danh sách kế hoạch đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/QuaTrinhToChucThucHien")]
        [AuthorizeAction("Index")]
        public IActionResult QuaTrinhToChucThucHien() => BuildView("Chức năng Danh sách quá trình tổ chức thực hiện đang được phát triển.");

        [HttpGet("Systems/DangPhatTrien/DanhGiaKetQua")]
        [AuthorizeAction("Index")]
        public IActionResult DanhGiaKetQua() => BuildView("Chức năng Đánh giá kết quả đang được phát triển.");

        private IActionResult BuildView(string message)
        {
            ViewData["Messages"] = message;
            ViewData["Controller"] = "Home";
            ViewData["Action"] = "Index";
            return View("Views/Shared/Error.cshtml");
        }
    }
}
