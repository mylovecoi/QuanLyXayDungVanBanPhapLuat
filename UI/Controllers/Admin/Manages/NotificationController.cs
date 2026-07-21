using DataAccess.Entities.Manages;
using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using System.Threading.Tasks;
using UI.Helper;

namespace UI.Controllers.Admin.Manages
{
    public class NotificationController(INotificationService notificationService) : Controller
    {
        private readonly INotificationService _notificationService = notificationService;

        private ISession? _session => HttpContext?.Session;

        [HttpGet("Manages/Notifications")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1, string Status = "")
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            var model = await _notificationService.GetNotificationAsync(TimKiem, PageSize, PageCurrent, Status);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            ViewData["Title"] = "Thông tin thông báo";
            ViewData["Status"] = Status;
            return View("Views/Admin/Manages/Notification/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/Notification/MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return Json(new { status = "success", message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }

        [HttpGet("Manages/Notification/Show")]
        public async Task<IActionResult> Show(Guid Id, string PhanLoai)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            string controller, action;
            object parameter;
            if (!_notificationService.ShowNotification(Id, PhanLoai, out controller, out action, out parameter))
            {
                ViewData["Messages"] = "Không tìm thấy thông tin thông báo";
                ViewData["Controller"] = "Notification";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            await _notificationService.MarkAsReadAsync(Id);
            return RedirectToAction(action, controller, parameter);
        }

        [HttpGet("Manages/Notification/Count")]
        public async Task<IActionResult> CountNotification()
        {
            int count = await _notificationService.CountNotificationAsync();
            if (count == 0)
            {
                return Json(new { status = "error" });
            }
            return Json(new { status = "success", count = count });
        }

        [HttpPost("Manages/Notification/Store")]
        public async Task<IActionResult> Store(Notification request)
        {
            var data = await _notificationService.StoreAsync(request);
            if(data.Status == "error")
            {
                return Ok("lỗi");
            }
            return Ok("success");
        }
    }
}
