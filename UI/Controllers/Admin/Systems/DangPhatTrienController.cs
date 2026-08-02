using Microsoft.AspNetCore.Mvc;
using UI.Security;

namespace UI.Controllers.Admin.Systems
{
    [SetViewDataFilter]
    public class DangPhatTrienController : Controller
    {
        [HttpGet("Systems/DangPhatTrien")]
        [AuthorizeAction("Index")]
        public IActionResult Index() => BuildView("Chá»©c nÄƒng Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/TrangThai")]
        [AuthorizeAction("Index")]
        public IActionResult TrangThai() => BuildView("Chá»©c nÄƒng Danh sĂ¡ch tráº¡ng thĂ¡i Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/DangKyXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult DangKyXayDung() => BuildView("Chá»©c nÄƒng ÄÄƒng kĂ½ xĂ¢y dá»±ng Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/XetDuyetDangKy")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetDangKy() => BuildView("Chá»©c nÄƒng XĂ©t duyá»‡t Ä‘Äƒng kĂ½ Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/PheDuyetDangKy")]
        [AuthorizeAction("Index")]
        public IActionResult PheDuyetDangKy() => BuildView("Chá»©c nÄƒng PhĂª duyá»‡t Ä‘Äƒng kĂ½ Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/QuyTrinhSoanThao")]
        [AuthorizeAction("Index")]
        public IActionResult QuyTrinhSoanThao() => RedirectToAction("Index", "QuyTrinhSoanThao");

        [HttpGet("Systems/DangPhatTrien/XayDungVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XayDungVanBan() => BuildView("Chá»©c nÄƒng XĂ¢y dá»±ng vÄƒn báº£n Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/GopYDanhGia")]
        [AuthorizeAction("Index")]
        public IActionResult GopYDanhGia() => RedirectToAction("Index", "LayYKienUBND");

        [HttpGet("Systems/DangPhatTrien/XetDuyetSoanThao")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetSoanThao() => RedirectToAction("Index", "DuThaoVanBan");

        [HttpGet("Systems/DangPhatTrien/GiaHanXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult GiaHanXayDung() => RedirectToAction("Index", "GiaHanXayDung");

        [HttpGet("Systems/DangPhatTrien/DanhSachGiaHanXayDung")]
        [AuthorizeAction("Index")]
        public IActionResult DanhSachGiaHanXayDung() => RedirectToAction("Index", "GiaHanXayDung");

        [HttpGet("Systems/DangPhatTrien/XetDuyetVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetVanBan() => RedirectToAction("Index", "XetDuyetDuThao");

        [HttpGet("Systems/DangPhatTrien/PheDuyetVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult PheDuyetVanBan() => BuildView("Chá»©c nÄƒng PhĂª duyá»‡t vÄƒn báº£n Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/XetDuyetBanHanhVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult XetDuyetBanHanhVanBan() => BuildView("Chá»©c nÄƒng XĂ©t duyá»‡t ban hĂ nh vÄƒn báº£n Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/BanHanhVanBan")]
        [AuthorizeAction("Index")]
        public IActionResult BanHanhVanBan() => BuildView("Chá»©c nÄƒng Ban hĂ nh vÄƒn báº£n Ä‘ang Ä‘Æ°á»£c phĂ¡t triá»ƒn.");

        [HttpGet("Systems/DangPhatTrien/DanhSachKeHoach")]
        [AuthorizeAction("Index")]
        public IActionResult DanhSachKeHoach() => RedirectToAction("Index", "DanhSachKeHoachThiHanhPhapLuat");

        [HttpGet("Systems/DangPhatTrien/QuaTrinhToChucThucHien")]
        [AuthorizeAction("Index")]
        public IActionResult QuaTrinhToChucThucHien() => RedirectToAction("Index", "QuaTrinhToChucThucHien");

        [HttpGet("Systems/DangPhatTrien/DanhGiaKetQua")]
        [AuthorizeAction("Index")]
        public IActionResult DanhGiaKetQua() => RedirectToAction("Index", "DanhGiaKetQuaThiHanhPhapLuat");

        private IActionResult BuildView(string message)
        {
            ViewData["Messages"] = message;
            ViewData["Controller"] = "Home";
            ViewData["Action"] = "Index";
            return View("Views/Shared/Error.cshtml");
        }
    }
}
