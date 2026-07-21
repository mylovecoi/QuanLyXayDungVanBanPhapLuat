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
    [Route("Settings/DanhMucGia/DanhMucGiaThueTaiNguyenCt")]
    [SetViewDataFilter(controller: "DanhMucGiaThueTaiNguyen")]
    public class DanhMucGiaThueTaiNguyenCtController(
        IDanhMucGiaThueTaiNguyenCtService danhMucGiaThueTaiNguyenCtService,
        IDanhMucGiaThueTaiNguyenService danhMucGiaThueTaiNguyenService,
        DataAccess.ApplicationDbContext dbContext
    ) : BaseController
    {
        private readonly IDanhMucGiaThueTaiNguyenCtService _danhMucGiaThueTaiNguyenCtService = danhMucGiaThueTaiNguyenCtService;
        private readonly IDanhMucGiaThueTaiNguyenService _danhMucGiaThueTaiNguyenService = danhMucGiaThueTaiNguyenService;
        private readonly DataAccess.ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/Settings/DanhMucGia/DanhMucGiaThueTaiNguyenCt/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "DanhMucGiaThueTaiNguyen")]
        public async Task<IActionResult> Index(Guid danhMucGiaThueTaiNguyenId, string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var danhMucResponse = await _danhMucGiaThueTaiNguyenService.EditAsync(danhMucGiaThueTaiNguyenId);
            if (danhMucResponse.Status == "error")
            {
                ViewData["Messages"] = danhMucResponse.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyen";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var danhMuc = danhMucResponse.Data as DanhMucGiaThueTaiNguyen;
            ViewData["DanhMucGiaThueTaiNguyenId"] = danhMucGiaThueTaiNguyenId;
            ViewData["TenDanhMuc"] = danhMuc?.TenDanhMuc;
            ViewData["Title"] = "Chi tiết danh mục giá thuê tài nguyên";

            var model = await _danhMucGiaThueTaiNguyenCtService.GetListDanhMucCtAsync(danhMucGiaThueTaiNguyenId, timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyen";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);
            ViewData["DonViTinhs"] = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "DanhMucGiaThueTaiNguyen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucGiaThueTaiNguyenCt request)
        {
            var model = await _danhMucGiaThueTaiNguyenCtService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "DanhMucGiaThueTaiNguyen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucGiaThueTaiNguyenCtService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyenCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DonViTinhs"] = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "DanhMucGiaThueTaiNguyen")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucGiaThueTaiNguyenCt request)
        {
            var model = await _danhMucGiaThueTaiNguyenCtService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "DanhMucGiaThueTaiNguyen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var detailResponse = await _danhMucGiaThueTaiNguyenCtService.EditAsync(id_delete);
            Guid danhMucGiaThueTaiNguyenId = Guid.Empty;
            if (detailResponse.Status == "success" && detailResponse.Data is DanhMucGiaThueTaiNguyenCt detail)
            {
                danhMucGiaThueTaiNguyenId = detail.DanhMucGiaThueTaiNguyenId;
            }

            var model = await _danhMucGiaThueTaiNguyenCtService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyenCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucGiaThueTaiNguyenCt", new { danhMucGiaThueTaiNguyenId = danhMucGiaThueTaiNguyenId });
        }

        [HttpPost("DeleteAll")]
        [AuthorizeAction("Delete", controller: "DanhMucGiaThueTaiNguyen", action: "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(Guid danhMucGiaThueTaiNguyenId_delete)
        {
            var model = await _danhMucGiaThueTaiNguyenCtService.DeleteAllAsync(danhMucGiaThueTaiNguyenId_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyenCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucGiaThueTaiNguyenCt", new { danhMucGiaThueTaiNguyenId = danhMucGiaThueTaiNguyenId_delete });
        }

        [HttpGet("NhanExcel")]
        [AuthorizeAction("Index", controller: "DanhMucGiaThueTaiNguyen", action: "Index")]
        public async Task<IActionResult> NhanExcel(Guid danhMucGiaThueTaiNguyenId)
        {
            var danhMucResponse = await _danhMucGiaThueTaiNguyenService.EditAsync(danhMucGiaThueTaiNguyenId);
            if (danhMucResponse.Status == "error")
            {
                ViewData["Messages"] = danhMucResponse.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyen";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var danhMuc = danhMucResponse.Data as DanhMucGiaThueTaiNguyen;
            ViewData["DanhMucGiaThueTaiNguyenId"] = danhMucGiaThueTaiNguyenId;
            ViewData["TenDanhMuc"] = danhMuc?.TenDanhMuc;
            ViewData["Title"] = "Nhận chi tiết danh mục giá thuê tài nguyên từ Excel";

            return View(ViewPath("NhanExcel"));
        }

        [HttpPost("ImportExcel")]
        [AuthorizeAction("Store", controller: "DanhMucGiaThueTaiNguyen", action: "Index")]
        public async Task<IActionResult> ImportExcel([FromBody] List<DanhMucGiaThueTaiNguyenCt> items)
        {
            if (items == null || items.Count == 0)
            {
                return Json(new { status = "error", message = "Không nhận được dữ liệu Excel!" });
            }
            var response = await _danhMucGiaThueTaiNguyenCtService.StoreRangeAsync(items);
            return Json(new { status = response.Status, message = response.Message });
        }
    }
}
