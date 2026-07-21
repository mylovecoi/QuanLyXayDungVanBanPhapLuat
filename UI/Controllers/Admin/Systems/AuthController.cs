using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using UI.ViewModels;
using MailKit;
using Services;

namespace UI.Controllers.Admin.Systems
{
    public class AuthController : Controller
    {
        private readonly OTPService _otpService;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ISystemInfoService _systemInfoService;
        private readonly IViewRenderService _viewRenderService;
        private readonly SmtpMailService _smtpMailService;

        public AuthController(OTPService otpService, IAuthService authService, IUserService userService, 
                                ISystemInfoService systemInfoService, IViewRenderService viewRenderService, SmtpMailService smtpMailService)
        {
            _otpService = otpService;
            _authService = authService;
            _userService = userService;
            _systemInfoService = systemInfoService;
            _viewRenderService = viewRenderService;
            _smtpMailService = smtpMailService;
        }

        [HttpGet("Auth/Login")]
        public async Task<IActionResult> Login(string username)
        {
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();
            ViewData["Title"] = "Đăng nhập vào chương trình";
            ViewData["Username"] = username;
            ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
            ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";
            ViewData["Train"] = systemInfo != null ? systemInfo.Train : false;
            return View("Views/Admin/Systems/Auth/Auth.cshtml");
        }

        [HttpPost("Auth/GetOTPValidate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetOTPValidate(string username, string password)
        {
            var ckIsUser = await _authService.CheckIsUser(username, password);
            if (ckIsUser.Status == "error")
            {
                return Json(ckIsUser);
            }

            var model = await _userService.GetUserByUserNamePasswordAsync(username, password);
            string otpUrl = _otpService.GenerateOtpUrl(model.Email, model.OTPSecretKey);
            var data = new VMOtpValidation  { Username = username, Password = password, OtpQrCodeUrl = otpUrl, FirstLogin = model.FirstLogin };

            return PartialView("~/Views/Admin/Systems/Auth/_OtpValidation.cshtml", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin(string username_otp, string password_otp, string? key_otp = null, long? clientUnixTimestamp_otp = null)
        {
            var ckIsUser = await _authService.CheckIsUser(username_otp, password_otp);
            if (ckIsUser.Status == "error")
            {
                ModelState.AddModelError("error", ckIsUser.Message);
                ViewData["Title"] = "Đăng nhập vào chương trình";
                return View("Views/Admin/Systems/Auth/Auth.cshtml");
            }
            /*
            if (!_authService.CheckOTP(username_otp, password_otp, key_otp, clientUnixTimestamp_otp))
            {
                ModelState.AddModelError("error", "Mã OTP không đúng hoặc đã hết hạn !!!");
                ViewData["Title"] = "Đăng nhập vào chương trình";
                return View("Views/Admin/Systems/Auth/Auth.cshtml");
            }
            */
            await _authService.Sigin(username_otp, password_otp);
           
            if (password_otp == "Life@2012!")
            {
                ModelState.AddModelError("error", "Vui lòng thay đổi mật khẩu để bảo mật dữ liệu!");
                return RedirectToAction("ChangePassword", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Auth/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Auth/ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string username_fp, string email_fp)
        {
            if (string.IsNullOrEmpty(username_fp) || string.IsNullOrEmpty(email_fp))
            {
                ModelState.AddModelError("error", "Thông tin không được bỏ trống !!!");
                var systemInfo = await _systemInfoService.GetSystemInfoAsync();
                ViewData["Title"] = "Đăng nhập vào chương trình";
                ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
                ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";

                return View("Views/Admin/Systems/Auth/Auth.cshtml");
            }
            var ckIsUser = await _userService.IsUserMaillExitAsync(username_fp, email_fp);
            if(!ckIsUser)
            {
                ModelState.AddModelError("error", "Thông tin không chính xác !!!");
                var systemInfo = await _systemInfoService.GetSystemInfoAsync();
                ViewData["Title"] = "Đăng nhập vào chương trình";
                ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
                ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";

                return View("Views/Admin/Systems/Auth/Auth.cshtml");
            }
            var userInfo = await _userService.GetUserByUserEmailAsync(username_fp, email_fp);
            if(userInfo.Status == "error")
            {
                ModelState.AddModelError("error", userInfo.Message);
                var systemInfo = await _systemInfoService.GetSystemInfoAsync();
                ViewData["Title"] = "Đăng nhập vào chương trình";
                ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
                ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";

                return View("Views/Admin/Systems/Auth/Auth.cshtml");
            } 
            var resetPass = await _userService.ResetPasswordAsync(userInfo.Data?.Id);
            if (resetPass.Status == "error")
            {
                ModelState.AddModelError("error", resetPass.Message);
                var systemInfo = await _systemInfoService.GetSystemInfoAsync();
                ViewData["Title"] = "Đăng nhập vào chương trình";
                ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
                ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";
                return View("Views/Admin/Systems/Auth/Auth.cshtml");
            }
            await _smtpMailService.SendMailAsync(email_fp, "Lấy lại mật khẩu truy cập", "Mật khẩu mới của bạn là: Life@2012!");
            ViewData["Message"] = "Mật khẩu mới đã được gửi vào email của bạn. Vui lòng kiểm tra email để đăng nhập!";
            ViewData["Controller"] = "Auth";
            ViewData["Action"] = "Login";
            return View("Views/Shared/Success.cshtml");
        }
    }
}