using Microsoft.AspNetCore.Mvc;
using UI.Security;

namespace UI.Controllers.Admin.Systems
{
    [SetViewDataFilter]
    public class DangPhatTrienController : Controller
    {
        [HttpGet("Systems/DangPhatTrien")]
        [AuthorizeAction("Index")]
        public IActionResult Index()
        {
            return BuildView("Chức năng đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/TrangThai")]
        [AuthorizeAction("Index")]
        public IActionResult TrangThai()
        {
            return BuildView("Chức năng Danh sách trạng thái đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/DangKyXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult DangKyXayDung()
        {
            return BuildView("Chức năng Đăng ký xây dựng đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/XetDuyetDangKy")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetDangKy()
        {
            return BuildView("Chức năng Xét duyệt đăng ký đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/PheDuyetDangKy")]
        [AuthorizeAction("Index")]
        public IActionResult PheDuyetDangKy()
        {
            return BuildView("Chức năng Phê duyệt đăng ký đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/QuyTrinhSoanThao")]
        [AuthorizeAction("Index")]
        public IActionResult QuyTrinhSoanThao()
        {
            return RedirectToAction("Index", "QuyTrinhSoanThao");
        }

        [HttpGet("Systems/DangPhatTrien/XayDungVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XayDungVanBan()
        {
            return BuildView("Chức năng Xây dựng văn bản đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/GiaHanXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult GiaHanXayDung()
        {
            return BuildView("Chức năng Gia hạn thời gian xây dựng đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/XetDuyetVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetVanBan()
        {
            return BuildView("Chức năng Xét duyệt văn bản đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/PheDuyetVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult PheDuyetVanBan()
        {
            return BuildView("Chức năng Phê duyệt văn bản đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/DanhSachKeHoach")]
        [AuthorizeAction("Index")]
        public IActionResult DanhSachKeHoach()
        {
            return BuildView("Chức năng Danh sách kế hoạch đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/QuaTrinhToChucThucHien")]
        [AuthorizeAction("Index")]
        public IActionResult QuaTrinhToChucThucHien()
        {
            return BuildView("Chức năng Danh sách quá trình tổ chức thực hiện đang được phát triển.");
        }

        [HttpGet("Systems/DangPhatTrien/DanhGiaKetQua")]
        [AuthorizeAction("Index")]
        public IActionResult DanhGiaKetQua()
        {
            return BuildView("Chức năng Đánh giá kết quả đang được phát triển.");
        }

        private IActionResult BuildView(string message)
        {
            ViewData["Messages"] = message;
            ViewData["Controller"] = "Home";
            ViewData["Action"] = "Index";
            return View("Views/Shared/Error.cshtml");
        }
    }
}
