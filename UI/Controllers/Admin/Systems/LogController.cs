using UI.Helper;
using UI.Security;
using UI.ViewModels;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using Azure;



namespace UI.Controllers.Admin.Systems
{
    [TypeFilter(typeof(SetViewDataFilter))] // Khai báo đệ tự động truyền ViewData["Title", "MenuActive", "Role", "TableName"]   
    public class LogController : Controller
    {
        public readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet("Systems/Log")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index( string TimKiem = "", int PageSize = 5,int PageCurrent = 1, DateTime? NgayBatDau = null, DateTime? NgayKetThuc = null)
        {
            NgayBatDau ??= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            NgayKetThuc ??= DateTime.Now;

            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await _logService.GetLogsWithFilterAsync(TimKiem, PageSize, PageCurrent, NgayBatDau.Value, NgayKetThuc.Value);
           
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewBag.NgayBatDau = NgayBatDau.Value;
            ViewBag.NgayKetThuc = NgayKetThuc.Value;            
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Systems/NhatKyHoatDong/Index.cshtml", model.Data);
        }
    }
}
