using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.AspNetCore.Mvc;
using Services.Settings.DanhMucGia;
using System;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucGia
{
    [Route("Settings/DanhMucGia/DanhMucNuocSach")]
    [SetViewDataFilter]
    public class DanhMucNuocSachController(
        IDanhMucNuocSachService danhMucNuocSachService,
        IDanhMucNuocSachCtService danhMucNuocSachCtService
    ) : BaseController
    {
        private readonly IDanhMucNuocSachService _danhMucNuocSachService = danhMucNuocSachService;
        private readonly IDanhMucNuocSachCtService _danhMucNuocSachCtService = danhMucNuocSachCtService;
        private string ViewPath(string viewName) => $"../Admin/Settings/DanhMucGia/DanhMucNuocSach/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _danhMucNuocSachService.GetListDanhMucNuocSachAsync(timKiem, pageSize, pageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Danh mục nước sạch";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucNuocSach request)
        {
            var model = await _danhMucNuocSachService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucNuocSachService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucNuocSach";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucNuocSach request)
        {
            var model = await _danhMucNuocSachService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucNuocSachService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucNuocSach";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucNuocSach");
        }

        [HttpGet("Show")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            var model = await _danhMucNuocSachService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucNuocSach";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var data = model.Data as DanhMucNuocSach;
            ViewData["Title"] = "Chi tiết danh mục nước sạch";

            // Fetch details list
            var detailsResponse = await _danhMucNuocSachCtService.GetListDanhMucCtAsync(id, "", 1000, 1);
            ViewData["Details"] = detailsResponse.Status == "success" ? detailsResponse.Data : new List<DanhMucNuocSachCt>();

            return View(ViewPath(nameof(Show)), data);
        }
    }
}
