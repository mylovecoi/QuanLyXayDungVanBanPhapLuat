using System.Net.WebSockets;
using System.Text.RegularExpressions;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Systems;

namespace Base.Controllers.Admin
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IAuthService _authService;
        private readonly ISystemInfoService _systemInfoService;
        private readonly List<(string Name, string Phone)> _contacts = new()
        {
            ("Hoàng Ngọc Long","0985 365 683"),
            ("Nguyễn Trần Huynh","0964 304 891"),
            ("Trịnh Minh Khải","0389 095 454"),
        };

        private ISession? _session => HttpContext?.Session;
        public HomeController(IHomeService homeService, IAuthService authService, ISystemInfoService systemInfoService)
        {
            _homeService = homeService;
            _authService = authService;
            _systemInfoService = systemInfoService;
        }

        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            //if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            //{
            //    return RedirectToAction("Login", "Auth");
            //}
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();
            var response = await _homeService.GetHomeDashboardDataAsync();
            ViewData["DashboardImages"] = _homeService.GetDashboardImages();
            ViewData["Contacts"] = _contacts;
            ViewData["AppName"] = systemInfo != null ? systemInfo.AppName?.Replace("<br />", " ").ToUpper() : "HỆ THỐNG GIẢI PHÁP QUẢN LÝ";
            ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright?.Replace("<br />", " ") : "Bản quyền thuộc về LifeSoft";
            ViewData["Title"] = "Trang chủ";
            ViewData["MenuActive"] = "dashboard";
            return View("Views/Admin/Home/Dashboard/Index.cshtml", response.Data);
        }

        [HttpGet("Systems/ChangePassword")]
        public IActionResult ChangePassword()
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["Title"] = "Thay đổi mật khẩu truy cập";
            return View("Views/Admin/Home/ChangePassword.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(string current_password, string new_password, string verify_password)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }

            if (string.IsNullOrEmpty(current_password) && string.IsNullOrEmpty(new_password) && string.IsNullOrEmpty(verify_password))
            {
                ViewData["Title"] = "Thay đổi mật khẩu";
                ModelState.AddModelError("error", "Thông tin không được bỏ trống");
                ViewData["current_password"] = current_password;
                ViewData["new_password"] = new_password;
                ViewData["verify_password"] = verify_password;
                return View("Views/Admin/Home/ChangePassword.cshtml");
            }

            if (new_password != verify_password)
            {
                ViewData["Title"] = "Thay đổi mật khẩu";
                ModelState.AddModelError("error", "Mật khẩu mới và mật khẩu xác thực không trùng nhau");
                ViewData["current_password"] = current_password;
                return View("Views/Admin/Home/ChangePassword.cshtml");
            }

            if (current_password == verify_password)
            {
                ViewData["Title"] = "Thay đổi mật khẩu";
                ModelState.AddModelError("error", "Mật khẩu mới trùng với mật khẩu hiện tại!");
                ViewData["current_password"] = current_password;
                return View("Views/Admin/Home/ChangePassword.cshtml");
            }

            bool isPasswordCorrect = await _homeService.CheckCurrentPassword(current_password);
            if (!isPasswordCorrect)
            {
                ViewData["Title"] = "Thay đổi mật khẩu";
                ModelState.AddModelError("error", "Mật khẩu hiện tại không đúng!");
                ViewData["new_password"] = new_password;
                ViewData["verify_password"] = verify_password;
                return View("Views/Admin/Home/ChangePassword.cshtml");
            }

            Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!]).{8,20}$");
            if (!regex.IsMatch(verify_password))
            {
                ViewData["Title"] = "Thay đổi mật khẩu";
                ModelState.AddModelError("error", "Mật khẩu từ phải có ít nhất 8 ký tự và không quá 20 ký tự, bao gồm ít nhất 1 chữ cái viết hoa, 1 chữ cái viết thường, 1 chữ số và 1 ký tự đặc biệt!");
                ViewData["current_password"] = current_password;
                ViewData["new_password"] = new_password;
                ViewData["verify_password"] = verify_password;
                return View("Views/Admin/Home/ChangePassword.cshtml");
            }
            var model = await _homeService.UpdatePasswordAsync(verify_password);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Logout", "Auth");
        }

        [HttpGet("Systems/ThemeSetting")]
        public IActionResult ThemeSetting()
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["Title"] = "Thiết lập hiển thị giao diện chương trình";
            return View("Views/Admin/Home/ThemeSetting.cshtml", _authService.GetUserInfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateThemeSetting(User request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _homeService.UpdateThemeAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Logout", "Auth");
        }

        [HttpGet("Systems/UserInfo")]
        public IActionResult UserInfo()
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["Title"] = "Thiết lập hiển thị thông tin báo cáo chương trình";
            return View("Views/Admin/Home/UserInfo.cshtml", _authService.GetUserInfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserInfo(User request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _homeService.UpdateUserInfoAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Logout", "Auth");
        }

        [HttpGet("Home/GetTongHopHoSoStats")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetTongHopHoSoStats()
        {
            var response = await _homeService.GetTongHopHoSoStatsAsync();
            return Json(new { status = response.Status == "success" ? "success" : "error", message = response.Message, data = response.Data });
        }
    }
}
