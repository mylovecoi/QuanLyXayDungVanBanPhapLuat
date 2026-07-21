using DataAccess.Entities.ThamDinhGia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.ThamDinhGia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.ThamDinhGia
{
    [Route("ThamDinhGia/DanhMucHangHoaCt")]
    [SetViewDataFilter(controller: "ThamDinhGiaDanhMucHangHoa")]
    public class ThamDinhGiaDanhMucHangHoaCtController(
        IThamDinhGiaDanhMucHangHoaCtService thamDinhGiaDanhMucHangHoaCtService,
        IThamDinhGiaDanhMucHangHoaService thamDinhGiaDanhMucHangHoaService,
        DataAccess.ApplicationDbContext dbContext
    ) : Controller
    {
        private readonly IThamDinhGiaDanhMucHangHoaCtService _thamDinhGiaDanhMucHangHoaCtService = thamDinhGiaDanhMucHangHoaCtService;
        private readonly IThamDinhGiaDanhMucHangHoaService _thamDinhGiaDanhMucHangHoaService = thamDinhGiaDanhMucHangHoaService;
        private readonly DataAccess.ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/ThamDinhGia/DanhMucHangHoaCt/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        public async Task<IActionResult> Index(Guid hangHoaId, string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var parentResponse = await _thamDinhGiaDanhMucHangHoaService.EditAsync(hangHoaId);
            if (parentResponse.Status == "error")
            {
                ViewData["Messages"] = parentResponse.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoa";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var parent = parentResponse.Data as ThamDinhGiaDanhMucHangHoa;
            ViewData["HangHoaId"] = hangHoaId;
            ViewData["TenDanhMucHangHoa"] = parent?.TenDanhMucHangHoa;

            var model = await _thamDinhGiaDanhMucHangHoaCtService.GetListDanhMucCtAsync(hangHoaId, timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoa";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            var donViTinhs = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();
            ViewData["DonViTinhs"] = donViTinhs;

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(ThamDinhGiaDanhMucHangHoaCt request)
        {
            var model = await _thamDinhGiaDanhMucHangHoaCtService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _thamDinhGiaDanhMucHangHoaCtService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoaCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var donViTinhs = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();
            ViewData["DonViTinhs"] = donViTinhs;

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(ThamDinhGiaDanhMucHangHoaCt request)
        {
            var model = await _thamDinhGiaDanhMucHangHoaCtService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var detailResponse = await _thamDinhGiaDanhMucHangHoaCtService.EditAsync(id_delete);
            Guid hangHoaId = Guid.Empty;
            if (detailResponse.Status == "success" && detailResponse.Data is ThamDinhGiaDanhMucHangHoaCt detail)
            {
                hangHoaId = detail.HangHoaId;
            }

            var model = await _thamDinhGiaDanhMucHangHoaCtService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoaCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaDanhMucHangHoaCt", new { hangHoaId = hangHoaId });
        }

        [HttpPost("DeleteAll")]
        [AuthorizeAction("Delete", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll(Guid hangHoaId_delete)
        {
            var model = await _thamDinhGiaDanhMucHangHoaCtService.DeleteAllAsync(hangHoaId_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoaCt";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaDanhMucHangHoaCt", new { hangHoaId = hangHoaId_delete });
        }

        [HttpGet("NhanExcel")]
        [AuthorizeAction("Index", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        public async Task<IActionResult> NhanExcel(Guid hangHoaId)
        {
            var parentResponse = await _thamDinhGiaDanhMucHangHoaService.EditAsync(hangHoaId);
            if (parentResponse.Status == "error")
            {
                ViewData["Messages"] = parentResponse.Message;
                ViewData["Controller"] = "ThamDinhGiaDanhMucHangHoa";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var parent = parentResponse.Data as ThamDinhGiaDanhMucHangHoa;
            ViewData["HangHoaId"] = hangHoaId;
            ViewData["TenDanhMucHangHoa"] = parent?.TenDanhMucHangHoa;

            return View(ViewPath("NhanExcel"));
        }

        [HttpPost("ImportExcel")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaDanhMucHangHoa", action: "Index")]
        public async Task<IActionResult> ImportExcel([FromBody] List<ThamDinhGiaDanhMucHangHoaCt> items)
        {
            if (items == null || items.Count == 0)
            {
                return Json(new { status = "error", message = "Không nhận được dữ liệu Excel!" });
            }
            var response = await _thamDinhGiaDanhMucHangHoaCtService.StoreRangeAsync(items);
            return Json(new { status = response.Status, message = response.Message });
        }
    }
}
