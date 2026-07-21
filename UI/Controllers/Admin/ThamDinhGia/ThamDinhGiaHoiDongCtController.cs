using DataAccess.Entities.ThamDinhGia;
using Microsoft.AspNetCore.Mvc;
using Services.ThamDinhGia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.ThamDinhGia
{
    [Route("ThamDinhGia/HoiDongCt")]
    [SetViewDataFilter(controller: "ThamDinhGiaHoiDong")]
    public class ThamDinhGiaHoiDongCtController(
        IThamDinhGiaHoiDongCtService thamDinhGiaHoiDongCtService,
        IThamDinhGiaHoiDongService thamDinhGiaHoiDongService
    ) : Controller
    {
        private readonly IThamDinhGiaHoiDongCtService _thamDinhGiaHoiDongCtService = thamDinhGiaHoiDongCtService;
        private readonly IThamDinhGiaHoiDongService _thamDinhGiaHoiDongService = thamDinhGiaHoiDongService;

        private string ViewPath(string viewName) => $"../Admin/ThamDinhGia/HoiDongCt/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "ThamDinhGiaHoiDong", action: "Index")]
        public async Task<IActionResult> Index(Guid hoiDongId, string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var parentResponse = await _thamDinhGiaHoiDongService.EditAsync(hoiDongId);
            if (parentResponse.Status == "error")
            {
                ViewData["Messages"] = parentResponse.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDong";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var parent = parentResponse.Data as ThamDinhGiaHoiDong;
            ViewData["HoiDongId"] = hoiDongId;
            ViewData["TenHoiDong"] = parent?.TenHoiDong;

            var model = await _thamDinhGiaHoiDongCtService.GetListDanhMucCtAsync(hoiDongId, timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDong";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(ThamDinhGiaHoiDongCt request)
        {
            var model = await _thamDinhGiaHoiDongCtService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _thamDinhGiaHoiDongCtService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDongCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(ThamDinhGiaHoiDongCt request)
        {
            var model = await _thamDinhGiaHoiDongCtService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var detailResponse = await _thamDinhGiaHoiDongCtService.EditAsync(id_delete);
            Guid hoiDongId = Guid.Empty;
            if (detailResponse.Status == "success" && detailResponse.Data is ThamDinhGiaHoiDongCt detail)
            {
                hoiDongId = detail.HoiDongId;
            }

            var model = await _thamDinhGiaHoiDongCtService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDongCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaHoiDongCt", new { hoiDongId = hoiDongId });
        }

        [HttpPost("DeleteAll")]
        [AuthorizeAction("Delete", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(Guid hoiDongId_delete)
        {
            var model = await _thamDinhGiaHoiDongCtService.DeleteAllAsync(hoiDongId_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDongCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaHoiDongCt", new { hoiDongId = hoiDongId_delete });
        }

        [HttpGet("NhanExcel")]
        [AuthorizeAction("Index", controller: "ThamDinhGiaHoiDong", action: "Index")]
        public async Task<IActionResult> NhanExcel(Guid hoiDongId)
        {
            var parentResponse = await _thamDinhGiaHoiDongService.EditAsync(hoiDongId);
            if (parentResponse.Status == "error")
            {
                ViewData["Messages"] = parentResponse.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDong";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var parent = parentResponse.Data as ThamDinhGiaHoiDong;
            ViewData["HoiDongId"] = hoiDongId;
            ViewData["TenHoiDong"] = parent?.TenHoiDong;

            return View(ViewPath("NhanExcel"));
        }

        [HttpPost("ImportExcel")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaHoiDong", action: "Index")]
        public async Task<IActionResult> ImportExcel([FromBody] List<ThamDinhGiaHoiDongCt> items)
        {
            if (items == null || items.Count == 0)
            {
                return Json(new { status = "error", message = "Không nhận được dữ liệu Excel!" });
            }
            var response = await _thamDinhGiaHoiDongCtService.StoreRangeAsync(items);
            return Json(new { status = response.Status, message = response.Message });
        }
    }
}
