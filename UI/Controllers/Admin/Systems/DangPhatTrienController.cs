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

        [HttpGet("Systems/DangPhatTrien/QuyTrinhSoanThao")]
        [AuthorizeAction("Index")]
        public IActionResult QuyTrinhSoanThao()
        {
            return BuildView("Chức năng Danh sách quy trình soạn thảo đang được phát triển.");
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
