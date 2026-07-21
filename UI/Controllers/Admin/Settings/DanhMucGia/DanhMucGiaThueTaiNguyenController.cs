using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Settings.DanhMucGia;
using System;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using System.Collections.Generic;

namespace UI.Controllers.Admin.Settings.DanhMucGia
{
    [Route("Settings/DanhMucGia/DanhMucGiaThueTaiNguyen")]
    [SetViewDataFilter]
    public class DanhMucGiaThueTaiNguyenController(
        IDanhMucGiaThueTaiNguyenService danhMucGiaThueTaiNguyenService,
        IDanhMucGiaThueTaiNguyenCtService danhMucGiaThueTaiNguyenCtService,
        DataAccess.ApplicationDbContext dbContext
    ) : BaseController
    {
        private readonly IDanhMucGiaThueTaiNguyenService _danhMucGiaThueTaiNguyenService = danhMucGiaThueTaiNguyenService;
        private readonly IDanhMucGiaThueTaiNguyenCtService _danhMucGiaThueTaiNguyenCtService = danhMucGiaThueTaiNguyenCtService;
        private readonly DataAccess.ApplicationDbContext _dbContext = dbContext;
        private string ViewPath(string viewName) => $"../Admin/Settings/DanhMucGia/DanhMucGiaThueTaiNguyen/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _danhMucGiaThueTaiNguyenService.GetListDanhMucGiaThueTaiNguyenAsync(timKiem, pageSize, pageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Danh mục giá thuê tài nguyên";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucGiaThueTaiNguyen request)
        {
            var model = await _danhMucGiaThueTaiNguyenService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucGiaThueTaiNguyenService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyen";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucGiaThueTaiNguyen request)
        {
            var model = await _danhMucGiaThueTaiNguyenService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucGiaThueTaiNguyenService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyen";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucGiaThueTaiNguyen");
        }

        [HttpGet("Show")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            var model = await _danhMucGiaThueTaiNguyenService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaThueTaiNguyen";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var data = model.Data as DanhMucGiaThueTaiNguyen;
            ViewData["Title"] = "Chi tiết danh mục giá thuê tài nguyên";

            // Fetch details list
            var detailsResponse = await _danhMucGiaThueTaiNguyenCtService.GetListDanhMucCtAsync(id, "", 1000, 1);
            ViewData["Details"] = detailsResponse.Status == "success" ? detailsResponse.Data : new List<DanhMucGiaThueTaiNguyenCt>();
            ViewData["DonViTinhs"] = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();

            return View(ViewPath(nameof(Show)), data);
        }
    }
}
