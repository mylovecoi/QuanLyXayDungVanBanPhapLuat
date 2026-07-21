using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Services.Settings;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucDungChung
{
    [SetViewDataFilter]
    public class DanhMucDiaDanhController(IDanhMucDiaDanhService danhMucDiaDanhService, IOptionDataService optionDataService) : Controller
    {
        private readonly IDanhMucDiaDanhService _danhMucDiaDanhService = danhMucDiaDanhService;
        private readonly IOptionDataService _optionDataService = optionDataService;

        [HttpGet("Settings/DanhMucDungChung/DanhMucDiaDanh")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _danhMucDiaDanhService.GetDanhMucDiaDanhsAsync(timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);
            return View("Views/Admin/Settings/DanhMucDungChung/DanhMucDiaDanh/Index.cshtml", model.Data);
        }

        [HttpPost("Settings/DanhMucDungChung/DanhMucDiaDanh/Create")]
        [AuthorizeAction("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid Id)
        {
            ModelState.Clear();
            var data = new DanhMucDiaDanh
            { Id = Guid.Empty, TenDiaDanh = "", STTSapXep = await _danhMucDiaDanhService.GetSTTSapXep(Id), Level = 0 };
            var model = await _danhMucDiaDanhService.EditAsync(Id);
            if (model.Status == "success")
            {
                data.DiaDanhCapTrenId = model.Data?.Id ?? Guid.Empty;
                data.TenDiaDanhChuQuan = model.Data?.TenDiaDanh ?? "";
                data.Level = model.Data?.Level + 1 ?? 0;
            }
            return PartialView("Views/Admin/Settings/DanhMucDungChung/DanhMucDiaDanh/_FormFields.cshtml", data);
        }

        [HttpPost("Settings/DanhMucDungChung/DanhMucDiaDanh/Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Store(DanhMucDiaDanh request)
        {
            var model = await _danhMucDiaDanhService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Settings/DanhMucDungChung/DanhMucDiaDanh/Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucDiaDanhService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucDiaDanh";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("Views/Admin/Settings/DanhMucDungChung/DanhMucDiaDanh/_FormFields.cshtml", model.Data);

        }

        [HttpPost("Settings/DanhMucDungChung/DanhMucDiaDanh/Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucDiaDanh request)
        {
            var model = await _danhMucDiaDanhService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucDiaDanhService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucDiaDanh";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucDiaDanh");
        }
    }
}
