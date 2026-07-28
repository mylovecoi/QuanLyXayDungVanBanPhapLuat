
using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Public
{
    [SetViewDataFilter]
    public class HeThongVanBanController : Controller
    {
        private readonly IVanBanPhapLuatService _vanBanPhapLuatService;
        public HeThongVanBanController(IVanBanPhapLuatService vanBanPhapLuatService)
        {
            _vanBanPhapLuatService = vanBanPhapLuatService;
        }

        [HttpGet("Public/HeThongVanBan", Name = "he-thong-van-ban")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            var model = await _vanBanPhapLuatService.GetVanBanPhapLuatsAsync(TimKiem, null, PageSize, PageCurrent, true);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["MenuActive"] = "public_hethongvanban";
            ViewData["Title"] = "HỆ THỐNG VĂN BẢN QUY PHẠM PHÁP LUẬT";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Public/HeThongVanBan/Index.cshtml", model.Data);
        }


        [HttpPost("Public/HeThongVanBan/Show")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await _vanBanPhapLuatService.EditAsync(Id);
            if (model.Data == null || model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("~/Views/Public/HeThongVanBan/Show.cshtml", model.Data);
        }
    }
}
