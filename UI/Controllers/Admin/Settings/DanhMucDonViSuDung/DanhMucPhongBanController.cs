using DataAccess.Entities.Settings;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Settings;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucDonViSuDung
{
    [SetViewDataFilter]
    public class DanhMucPhongBanController(
       IDanhMucPhongBanService danhMucPhongBanService,
       IDanhMucDonViService danhMucDonViService) : Controller
    {
        [HttpGet("Settings/DanhMucDonViSuDung/DanhMucPhongBan")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string Search = "", int PageSize = 5, int PageCurrent = 1, Guid? DonViId = null, LoaiPhongBan? LoaiPhongBan = null)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            // Lấy ID đơn vị của người dùng hiện tại nếu chưa chọn
            var userDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);
            if (DonViId == null || DonViId == Guid.Empty)
            {
                DonViId = userDonViId;
            }

            // Lấy danh sách đơn vị theo cấp cha con
            var danhMucDonVis = await danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVis"] = danhMucDonVis;
            ViewData["DonViId"] = DonViId;
            ViewData["LoaiPhongBan"] = LoaiPhongBan;

            var model = await danhMucPhongBanService.GetDanhMucPhongBanAsync(Search, PageSize, PageCurrent, DonViId, LoaiPhongBan);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, Search, PageSize, PageCurrent);
            return View("Views/Admin/Settings/DanhMucDonViSuDung/DanhMucPhongBan/Index.cshtml", model.Data);
        }

        [HttpGet("Settings/DanhMucDonViSuDung/DanhMucPhongBan/Create")]
        [AuthorizeAction("Create")]
        public async Task<IActionResult> Create()
        {
            var model = new DanhMucPhongBan
            {
                Id = Guid.NewGuid()
            };
            ViewData["DanhMucDonVis"] = (await danhMucDonViService.GetDanhMucDonViAsync("", 100, 1)).Data;
            return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucPhongBan/_FormFields.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucPhongBan request)
        {
            var model = await danhMucPhongBanService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucPhongBan");
        }

        [HttpPost("Settings/DanhMucDonViSuDung/DanhMucPhongBan/Edit")]
        [AuthorizeAction("Edit")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await danhMucPhongBanService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            if (model.Data == null)
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DanhMucDonVis"] = (await danhMucDonViService.GetDanhMucDonViAsync("", 100, 1)).Data;
            return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucPhongBan/_FormFields.cshtml", model.Data);
        }

        [HttpPost]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DanhMucPhongBan request)
        {
            var model = await danhMucPhongBanService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucPhongBan");
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await danhMucPhongBanService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucPhongBan");
        }

        [HttpPost("Settings/DanhMucDonViSuDung/DanhMucPhongBan/Show")]
        [AuthorizeAction("Show")]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await danhMucPhongBanService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            if (model.Data == null)
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucPhongBan/Show.cshtml", model.Data);
        }
    }
}