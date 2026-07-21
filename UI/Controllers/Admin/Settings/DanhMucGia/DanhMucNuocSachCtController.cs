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
    [Route("Settings/DanhMucGia/DanhMucNuocSachCt")]
    [SetViewDataFilter(controller: "DanhMucNuocSach")]
    public class DanhMucNuocSachCtController(
        IDanhMucNuocSachCtService danhMucNuocSachCtService,
        IDanhMucNuocSachService danhMucNuocSachService,
        DataAccess.ApplicationDbContext dbContext
    ) : BaseController
    {
        private readonly IDanhMucNuocSachCtService _danhMucNuocSachCtService = danhMucNuocSachCtService;
        private readonly IDanhMucNuocSachService _danhMucNuocSachService = danhMucNuocSachService;
        private readonly DataAccess.ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/Settings/DanhMucGia/DanhMucNuocSachCt/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "DanhMucNuocSach")]
        public async Task<IActionResult> Index(Guid danhMucNuocSachId, string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var danhMucResponse = await _danhMucNuocSachService.EditAsync(danhMucNuocSachId);
            if (danhMucResponse.Status == "error")
            {
                ViewData["Messages"] = danhMucResponse.Message;
                ViewData["Controller"] = "DanhMucNuocSach";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var danhMuc = danhMucResponse.Data as DanhMucNuocSach;
            ViewData["DanhMucNuocSachId"] = danhMucNuocSachId;
            ViewData["TenDanhMuc"] = danhMuc?.TenDanhMuc;
            ViewData["Title"] = "Chi tiết danh mục nước sạch";

            var model = await _danhMucNuocSachCtService.GetListDanhMucCtAsync(danhMucNuocSachId, timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucNuocSach";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "DanhMucNuocSach")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucNuocSachCt request)
        {
            var model = await _danhMucNuocSachCtService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "DanhMucNuocSach")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucNuocSachCtService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucNuocSachCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "DanhMucNuocSach")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucNuocSachCt request)
        {
            var model = await _danhMucNuocSachCtService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "DanhMucNuocSach")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var detailResponse = await _danhMucNuocSachCtService.EditAsync(id_delete);
            Guid danhMucNuocSachId = Guid.Empty;
            if (detailResponse.Status == "success" && detailResponse.Data is DanhMucNuocSachCt detail)
            {
                danhMucNuocSachId = detail.DanhMucNuocSachId;
            }

            var model = await _danhMucNuocSachCtService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucNuocSachCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucNuocSachCt", new { danhMucNuocSachId = danhMucNuocSachId });
        }

        [HttpPost("DeleteAll")]
        [AuthorizeAction("Delete", controller: "DanhMucNuocSach", action: "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(Guid danhMucNuocSachId_delete)
        {
            var model = await _danhMucNuocSachCtService.DeleteAllAsync(danhMucNuocSachId_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucNuocSachCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucNuocSachCt", new { danhMucNuocSachId = danhMucNuocSachId_delete });
        }

        [HttpGet("NhanExcel")]
        [AuthorizeAction("Index", controller: "DanhMucNuocSach", action: "Index")]
        public async Task<IActionResult> NhanExcel(Guid danhMucNuocSachId)
        {
            var danhMucResponse = await _danhMucNuocSachService.EditAsync(danhMucNuocSachId);
            if (danhMucResponse.Status == "error")
            {
                ViewData["Messages"] = danhMucResponse.Message;
                ViewData["Controller"] = "DanhMucNuocSach";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var danhMuc = danhMucResponse.Data as DanhMucNuocSach;
            ViewData["DanhMucNuocSachId"] = danhMucNuocSachId;
            ViewData["TenDanhMuc"] = danhMuc?.TenDanhMuc;
            ViewData["Title"] = "Nhận chi tiết danh mục nước sạch từ Excel";

            return View(ViewPath("NhanExcel"));
        }

        [HttpPost("ImportExcel")]
        [AuthorizeAction("Store", controller: "DanhMucNuocSach", action: "Index")]
        public async Task<IActionResult> ImportExcel([FromBody] List<DanhMucNuocSachCt> items)
        {
            if (items == null || items.Count == 0)
            {
                return Json(new { status = "error", message = "Không nhận được dữ liệu Excel!" });
            }
            var response = await _danhMucNuocSachCtService.StoreRangeAsync(items);
            return Json(new { status = response.Status, message = response.Message });
        }
    }
}
