using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using System.Threading.Tasks;
using UI.Helper;
using DataAccess.Entities.Systems;

namespace UI.Controllers.Admin.Systems
{
    public class SystemInfoController : Controller
    {
        private readonly ISystemInfoService _systemInfoService;
        private ISession? _session => HttpContext?.Session;
        public SystemInfoController(ISystemInfoService systemInfoService)
        {
            _systemInfoService = systemInfoService;
        }

        [HttpGet("Systems/SystemInfo")]
        public async Task<IActionResult> Index()
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _systemInfoService.GetSystemInfoAsync();
            ViewData["Title"] = "License Information";
            return View("Views/Admin/Systems/SystemInfo/Index.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChange(SystemInfo request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _systemInfoService.SaveChangeAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Logout", "Auth");
        }
    }
}
