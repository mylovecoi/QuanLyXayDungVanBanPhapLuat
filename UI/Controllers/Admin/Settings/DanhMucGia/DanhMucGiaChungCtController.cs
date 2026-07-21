using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Settings.DanhMucGia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucGia
{
    [Route("Settings/DanhMucGia/DanhMucGiaChungCt")]
    [SetViewDataFilter(controller: "DanhMucGiaChung")]
    public class DanhMucGiaChungCtController(
        IDanhMucGiaChungCtService danhMucGiaChungCtService,
        IDanhMucGiaChungService danhMucGiaChungService,
        ApplicationDbContext dbContext
    ) : BaseController
    {
        private readonly IDanhMucGiaChungCtService _danhMucGiaChungCtService = danhMucGiaChungCtService;
        private readonly IDanhMucGiaChungService _danhMucGiaChungService = danhMucGiaChungService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/Settings/DanhMucGia/DanhMucGiaChungCt/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "DanhMucGiaChung")]
        public async Task<IActionResult> Index(Guid danhMucGiaChungId, string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var danhMucResponse = await _danhMucGiaChungService.EditAsync(danhMucGiaChungId);
            if (danhMucResponse.Status == "error")
            {
                ViewData["Messages"] = danhMucResponse.Message;
                ViewData["Controller"] = "DanhMucGiaChung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var danhMuc = danhMucResponse.Data as DanhMucGiaChung;
            ViewData["DanhMucGiaChungId"] = danhMucGiaChungId;
            ViewData["TenDanhMuc"] = danhMuc?.TenDanhMuc;
            ViewData["Title"] = "Chi tiết danh mục giá chung";

            var model = await _danhMucGiaChungCtService.GetListDanhMucCtAsync(danhMucGiaChungId, timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaChung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "DanhMucGiaChung")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucGiaChungCt request)
        {
            var model = await _danhMucGiaChungCtService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "DanhMucGiaChung")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucGiaChungCtService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaChungCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "DanhMucGiaChung")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucGiaChungCt request)
        {
            var model = await _danhMucGiaChungCtService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "DanhMucGiaChung")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var detailResponse = await _danhMucGiaChungCtService.EditAsync(id_delete);
            Guid danhMucGiaChungId = Guid.Empty;
            if (detailResponse.Status == "success" && detailResponse.Data is DanhMucGiaChungCt detail)
            {
                danhMucGiaChungId = detail.DanhMucGiaChungId;
            }

            var model = await _danhMucGiaChungCtService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaChungCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucGiaChungCt", new { danhMucGiaChungId = danhMucGiaChungId });
        }

        [HttpPost("DeleteAll")]
        [AuthorizeAction("Delete", controller: "DanhMucGiaChung", action: "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(Guid danhMucGiaChungId_delete)
        {
            var model = await _danhMucGiaChungCtService.DeleteAllAsync(danhMucGiaChungId_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaChungCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucGiaChungCt", new { danhMucGiaChungId = danhMucGiaChungId_delete });
        }

        [HttpGet("NhanExcel")]
        [AuthorizeAction("Index", controller: "DanhMucGiaChung", action: "Index")]
        public async Task<IActionResult> NhanExcel(Guid danhMucGiaChungId)
        {
            var danhMucResponse = await _danhMucGiaChungService.EditAsync(danhMucGiaChungId);
            if (danhMucResponse.Status == "error")
            {
                ViewData["Messages"] = danhMucResponse.Message;
                ViewData["Controller"] = "DanhMucGiaChung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var danhMuc = danhMucResponse.Data as DanhMucGiaChung;
            ViewData["DanhMucGiaChungId"] = danhMucGiaChungId;
            ViewData["TenDanhMuc"] = danhMuc?.TenDanhMuc;
            ViewData["MaNghe"] = danhMuc?.MaNghe;
            ViewData["Title"] = "Nhận chi tiết danh mục giá chung từ Excel";

            return View(ViewPath("NhanExcel"));
        }

        [HttpPost("ImportExcel")]
        [AuthorizeAction("Store", controller: "DanhMucGiaChung", action: "Index")]
        public async Task<IActionResult> ImportExcel([FromBody] List<DanhMucGiaChungCt> items)
        {
            if (items == null || items.Count == 0)
            {
                return Json(new { status = "error", message = "Không nhận được dữ liệu Excel!" });
            }
            var response = await _danhMucGiaChungCtService.StoreRangeAsync(items);
            return Json(new { status = response.Status, message = response.Message });
        }
    }
}
