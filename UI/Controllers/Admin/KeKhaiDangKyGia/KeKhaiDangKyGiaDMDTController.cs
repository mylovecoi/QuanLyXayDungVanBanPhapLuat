using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using Services.KeKhaiDangKyGia;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.KeKhaiDangKyGia
{
    [Route("KeKhaiDangKyGiaDMDT")]
    [SetViewDataFilter]
    public class KeKhaiDangKyGiaDMDTController(
        IKeKhaiDangKyGiaDanhMucService keKhaiDangKyGiaDanhMucService,
        ApplicationDbContext dbContext) : Controller
    {
        private readonly IKeKhaiDangKyGiaDanhMucService _keKhaiDangKyGiaDanhMucService = keKhaiDangKyGiaDanhMucService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        [HttpGet("")]
        [AuthorizeAction("Index", "KeKhaiDangKyGiaDMDT")]
        public async Task<IActionResult> Index(string DoanhNghiepQuanLyId = "all", string timKiem = "", int pageSize = 10, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            string level = FuntionGlobal.GetSsAdmin(HttpContext.Session, "Level");
            List<DoanhNghiep> doanhNghieps;

            if (level != "Doanh nghiệp")
            {
                doanhNghieps = await _dbContext.DoanhNghieps
                    .AsNoTracking()
                    .Where(doanhNghiep => doanhNghiep.TrangThai != "CXD")
                    .OrderBy(t => t.TenDoanhNghiep)
                    .ToListAsync();
            }
            else
            {
                string dnIdStr = FuntionGlobal.GetSsAdmin(HttpContext.Session, "DoanhNghiepId");
                Guid.TryParse(dnIdStr, out var userDnId);
                DoanhNghiepQuanLyId = userDnId.ToString();

                doanhNghieps = await _dbContext.DoanhNghieps
                    .AsNoTracking()
                    .Where(x => x.Id == userDnId)
                    .ToListAsync();
            }

            var response = await _keKhaiDangKyGiaDanhMucService.GetListDTAsync(DoanhNghiepQuanLyId, timKiem, pageSize, pageCurrent);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DoanhNghieps"] = doanhNghieps;
            ViewData["DoanhNghiepQuanLyId"] = DoanhNghiepQuanLyId;
            ViewData["Search"] = timKiem;

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, timKiem, pageSize, pageCurrent, (List<KeKhaiDangKyGiaDMDT>)(response.Data ?? new List<KeKhaiDangKyGiaDMDT>()));
            return View("~/Views/Admin/KeKhaiDangKyGia/DanhMuc/DanhMucDoiTuong/Index.cshtml", pageInfo);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", "KeKhaiDangKyGiaDMDT")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(KeKhaiDangKyGiaDMDT request)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.StoreDTAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", "KeKhaiDangKyGiaDMDT")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.EditDTAsync(id);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return Json(new { status = "success", data = model.Data });
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", "KeKhaiDangKyGiaDMDT")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(KeKhaiDangKyGiaDMDT request)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.UpdateDTAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", "KeKhaiDangKyGiaDMDT")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.DeleteDTAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "KeKhaiDangKyGiaDMDT";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "KeKhaiDangKyGiaDMDT");
        }
    }
}
