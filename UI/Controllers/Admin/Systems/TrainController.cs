using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using UI.Helper;

namespace UI.Controllers.Admin.Systems
{
    [Route("DanhSachTapHuan")]
    public class TrainController(IUserService userService, ISystemInfoService systemInfoService) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly ISystemInfoService _systemInfoService = systemInfoService;

        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();

            if (systemInfo.Train == false)
            {
                ViewData["Messages"] = "Thao tác không hợp lệ!!!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var model = await _userService.GetUsersAsync(timKiem, pageSize, pageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Danh sách tập huấn";
            ViewData["AppName"] = systemInfo != null ? systemInfo.AppName : "Hệ thống giải pháp quản lý dữ liệu";
            ViewData["Copyright"] = systemInfo != null ? systemInfo.Copyright : "LifeSoft";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View("~/Views/Admin/Systems/Train/Index.cshtml", model.Data);
        }
    }
}
