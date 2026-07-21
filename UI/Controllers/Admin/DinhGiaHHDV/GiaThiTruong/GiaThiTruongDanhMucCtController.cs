using DataAccess.Entities.DinhGiaHHDV;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DinhGiaHHDV.GiaThiTruong;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.DinhGiaHHDV.GiaThiTruong
{
    [Route("GiaThiTruong/DanhMucCt")]
    [SetViewDataFilter(controller: "GiaThiTruongDanhMuc")]
    public class GiaThiTruongDanhMucCtController(
        IGiaThiTruongDanhMucCtService giaThiTruongDanhMucCtService,
        IGiaThiTruongDanhMucService giaThiTruongDanhMucService,
        DataAccess.ApplicationDbContext dbContext
    ) : BaseController
    {
        private readonly IGiaThiTruongDanhMucCtService _giaThiTruongDanhMucCtService = giaThiTruongDanhMucCtService;
        private readonly IGiaThiTruongDanhMucService _giaThiTruongDanhMucService = giaThiTruongDanhMucService;
        private readonly DataAccess.ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/DinhGiaHHDV/GiaThiTruong/DanhMucCt/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "GiaThiTruongDanhMuc")]
        public async Task<IActionResult> Index(Guid thongTuId, string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var thongTuResponse = await _giaThiTruongDanhMucService.EditAsync(thongTuId);
            if (thongTuResponse.Status == "error")
            {
                ViewData["Messages"] = thongTuResponse.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMuc";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var thongTu = thongTuResponse.Data as GiaThiTruongDanhMuc;
            ViewData["ThongTuId"] = thongTuId;
            ViewData["TenTT"] = thongTu?.TenTT;

            var model = await _giaThiTruongDanhMucCtService.GetListDanhMucCtAsync(thongTuId, timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMuc";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            var donViTinhs = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();
            ViewData["DonViTinhs"] = donViTinhs;

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "GiaThiTruongDanhMuc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(GiaThiTruongDanhMucCt request)
        {
            var model = await _giaThiTruongDanhMucCtService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "GiaThiTruongDanhMuc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _giaThiTruongDanhMucCtService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMucCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var donViTinhs = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();
            ViewData["DonViTinhs"] = donViTinhs;

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "GiaThiTruongDanhMuc")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(GiaThiTruongDanhMucCt request)
        {
            var model = await _giaThiTruongDanhMucCtService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "GiaThiTruongDanhMuc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            // We need to know which thongTuId to redirect back to, or we can pass it from the form
            var detailResponse = await _giaThiTruongDanhMucCtService.EditAsync(id_delete);
            Guid thongTuId = Guid.Empty;
            if (detailResponse.Status == "success" && detailResponse.Data is GiaThiTruongDanhMucCt detail)
            {
                thongTuId = detail.ThongTuId;
            }

            var model = await _giaThiTruongDanhMucCtService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMucCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "GiaThiTruongDanhMucCt", new { thongTuId = thongTuId });
        }

        [HttpPost("DeleteAll")]
        [AuthorizeAction("Delete", controller: "GiaThiTruongDanhMuc", action: "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(Guid thongTuId_delete)
        {
            var model = await _giaThiTruongDanhMucCtService.DeleteAllAsync(thongTuId_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMucCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "GiaThiTruongDanhMucCt", new { thongTuId = thongTuId_delete });
        }

        [HttpGet("NhanExcel")]
        [AuthorizeAction("Index", controller: "GiaThiTruongDanhMuc", action: "Index")]
        public async Task<IActionResult> NhanExcel(Guid thongTuId)
        {
            var thongTuResponse = await _giaThiTruongDanhMucService.EditAsync(thongTuId);
            if (thongTuResponse.Status == "error")
            {
                ViewData["Messages"] = thongTuResponse.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMuc";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var thongTu = thongTuResponse.Data as GiaThiTruongDanhMuc;
            ViewData["ThongTuId"] = thongTuId;
            ViewData["TenTT"] = thongTu?.TenTT;

            return View(ViewPath("NhanExcel"));
        }

        [HttpPost("ImportExcel")]
        [AuthorizeAction("Store", controller: "GiaThiTruongDanhMuc", action: "Index")]
        public async Task<IActionResult> ImportExcel([FromBody] List<GiaThiTruongDanhMucCt> items)
        {
            if (items == null || items.Count == 0)
            {
                return Json(new { status = "error", message = "Không nhận được dữ liệu Excel!" });
            }
            var response = await _giaThiTruongDanhMucCtService.StoreRangeAsync(items);
            return Json(new { status = response.Status, message = response.Message });
        }
    }
}
