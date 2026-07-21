using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Settings.DanhMucDungChung;
using UI.Helper;
using UI.Security;
using System.Threading.Tasks;
using System;

namespace UI.Controllers.Admin.Settings.DanhMucDungChung
{
    [Route("Settings/DanhMucPhiLePhi")]
    [SetViewDataFilter]
    public class DanhMucPhiLePhiController(
        IDanhMucPhiLePhiService danhMucPhiLePhiService,
        DataAccess.ApplicationDbContext dbContext
    ) : Controller
    {
        private readonly IDanhMucPhiLePhiService _danhMucPhiLePhiService = danhMucPhiLePhiService;
        private readonly DataAccess.ApplicationDbContext _dbContext = dbContext;

        [HttpGet]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(Guid loaiNghiepVu, string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _danhMucPhiLePhiService.GetListDanhMucPhiLePhiAsync(timKiem, pageSize, pageCurrent, loaiNghiepVu);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DanhMucHopDong"] = await _danhMucPhiLePhiService.GetListDanhMucHopDong(loaiNghiepVu);
            ViewData["PhanLoaiPhiLePhi"] = await _danhMucPhiLePhiService.GetListPhanLoaiPhiLePhi();
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            var donViTinhs = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();
            ViewData["DonViTinhs"] = donViTinhs;

            return View("~/Views/Admin/Settings/DanhMucDungChung/DanhMucPhiLePhi/Index.cshtml", model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucPhiLePhi request)
        {
            var model = await _danhMucPhiLePhiService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucPhiLePhiService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucPhiLePhi";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PhanLoaiPhiLePhi"] = await _danhMucPhiLePhiService.GetListPhanLoaiPhiLePhi();
            ViewData["DanhMucHopDong"] = await _danhMucPhiLePhiService.GetListDanhMucHopDong(model.Data?.LoaiHopDongId);

            var donViTinhs = await _dbContext.DanhMucDonViTinhs.AsNoTracking().ToListAsync();
            ViewData["DonViTinhs"] = donViTinhs;

            return PartialView("Views/Admin/Settings/DanhMucDungChung/DanhMucPhiLePhi/_FormFields.cshtml", model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucPhiLePhi request)
        {
            var model = await _danhMucPhiLePhiService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucPhiLePhiService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucPhiLePhi";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucPhiLePhi");
        }
    }
}
