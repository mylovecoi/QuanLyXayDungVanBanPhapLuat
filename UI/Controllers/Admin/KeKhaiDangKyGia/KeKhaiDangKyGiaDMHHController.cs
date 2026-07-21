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
    [Route("KeKhaiDangKyGiaDMHH")]
    [SetViewDataFilter]
    public class KeKhaiDangKyGiaDMHHController(
        IKeKhaiDangKyGiaDanhMucService keKhaiDangKyGiaDanhMucService,
        ApplicationDbContext dbContext) : Controller
    {
        private readonly IKeKhaiDangKyGiaDanhMucService _keKhaiDangKyGiaDanhMucService = keKhaiDangKyGiaDanhMucService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        [HttpGet("")]
        [AuthorizeAction("Index", "KeKhaiDangKyGiaDMHH")]
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

            var response = await _keKhaiDangKyGiaDanhMucService.GetListHHAsync(DoanhNghiepQuanLyId, timKiem, pageSize, pageCurrent);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var donViTinhs = await _dbContext.DanhMucDonViTinhs
                .AsNoTracking()
                .OrderBy(t => t.TenDonViTinh)
                .ToListAsync();

            ViewData["DoanhNghieps"] = doanhNghieps;
            ViewData["DoanhNghiepQuanLyId"] = DoanhNghiepQuanLyId;
            ViewData["Search"] = timKiem;
            ViewData["DonViTinhs"] = donViTinhs;

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, timKiem, pageSize, pageCurrent, (List<KeKhaiDangKyGiaDMHH>)(response.Data ?? new List<KeKhaiDangKyGiaDMHH>()));
            return View("~/Views/Admin/KeKhaiDangKyGia/DanhMuc/DanhMucHangHoa/Index.cshtml", pageInfo);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", "KeKhaiDangKyGiaDMHH")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(KeKhaiDangKyGiaDMHH request)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.StoreHHAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", "KeKhaiDangKyGiaDMHH")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.EditHHAsync(id);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return Json(new { status = "success", data = model.Data });
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", "KeKhaiDangKyGiaDMHH")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(KeKhaiDangKyGiaDMHH request)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.UpdateHHAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", "KeKhaiDangKyGiaDMHH")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _keKhaiDangKyGiaDanhMucService.DeleteHHAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "KeKhaiDangKyGiaDMHH";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "KeKhaiDangKyGiaDMHH");
        }
    }
}
