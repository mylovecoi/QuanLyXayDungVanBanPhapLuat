using DataAccess.Entities.ThamDinhGia;
using Microsoft.AspNetCore.Mvc;
using Services.ThamDinhGia;
using System;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.ThamDinhGia
{
    [Route("ThamDinhGia/DanhMucHangHoa")]
    [SetViewDataFilter]
    public class ThamDinhGiaDanhMucHangHoaController(IThamDinhGiaDanhMucHangHoaService thamDinhGiaDanhMucHangHoaService) : Controller
    {
        private readonly IThamDinhGiaDanhMucHangHoaService _thamDinhGiaDanhMucHangHoaService = thamDinhGiaDanhMucHangHoaService;
        private string ViewPath(string viewName) => $"../Admin/ThamDinhGia/DanhMucHangHoa/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _thamDinhGiaDanhMucHangHoaService.GetListThamDinhGiaDanhMucHangHoaAsync(timKiem, pageSize, pageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(ThamDinhGiaDanhMucHangHoa request)
        {
            var model = await _thamDinhGiaDanhMucHangHoaService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _thamDinhGiaDanhMucHangHoaService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoa";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(ThamDinhGiaDanhMucHangHoa request)
        {
            var model = await _thamDinhGiaDanhMucHangHoaService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _thamDinhGiaDanhMucHangHoaService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoa";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaDanhMucHangHoa");
        }
    }
}
