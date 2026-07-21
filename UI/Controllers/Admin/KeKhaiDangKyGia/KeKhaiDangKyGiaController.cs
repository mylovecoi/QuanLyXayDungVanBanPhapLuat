using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using Services.KeKhaiDangKyGia;
using Services.DTOs.KeKhaiDangKyGia;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.KeKhaiDangKyGia
{
    [Route("KeKhaiDangKyGia")]
    [SetViewDataFilter]
    public class KeKhaiDangKyGiaController : Controller
    {
        private readonly IKeKhaiDangKyGiaService _keKhaiDangKyGiaService;
        private readonly ApplicationDbContext _dbContext;

        public KeKhaiDangKyGiaController(IKeKhaiDangKyGiaService keKhaiDangKyGiaService, ApplicationDbContext dbContext)
        {
            _keKhaiDangKyGiaService = keKhaiDangKyGiaService;
            _dbContext = dbContext;
        }

        private string ViewPath(string viewName) => $"../Admin/KeKhaiDangKyGia/DanhSach/{viewName}";

        [HttpGet("")]
        [AuthorizeAction(nameof(Index), "KeKhaiDangKyGia")]
        public async Task<IActionResult> Index(Guid DoanhNghiepQuanLyId, string MaNghe)
        {
            var filter = new KeKhaiDangKyGiaFilter(Request);

            string level = FuntionGlobal.GetSsAdmin(HttpContext.Session, "Level");
            List<DoanhNghiep> doanhNghieps;

            if (level != "Doanh nghiệp")
            {
                doanhNghieps = await _dbContext.DoanhNghieps
                    .AsNoTracking()
                    .Where(doanhNghiep => doanhNghiep.TrangThai != "CXD" && _dbContext.DoanhNghiepLvKds.Any(linhVucKinhDoanh => linhVucKinhDoanh.DoanhNghiepQuanLyId == doanhNghiep.Id && linhVucKinhDoanh.MaNghe == MaNghe && linhVucKinhDoanh.TrangThai != "CXD"))
                    .OrderBy(t => t.TenDoanhNghiep)
                    .ToListAsync();
            }
            else
            {
                string dnIdStr = FuntionGlobal.GetSsAdmin(HttpContext.Session, "DoanhNghiepId");
                Guid.TryParse(dnIdStr, out var userDnId);
                DoanhNghiepQuanLyId = userDnId;

                doanhNghieps = await _dbContext.DoanhNghieps
                    .AsNoTracking()
                    .Where(x => x.Id == DoanhNghiepQuanLyId)
                    .ToListAsync();
            }
            filter.DoanhNghiepQuanLyId = DoanhNghiepQuanLyId;

            var response = await _keKhaiDangKyGiaService.GetListByFilterAsync(filter);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var dn = await _dbContext.DoanhNghieps
                .FirstOrDefaultAsync(x => x.Id == DoanhNghiepQuanLyId);

            if (dn != null)
            {
                var lvkd = await _dbContext.DoanhNghiepLvKds
                    .FirstOrDefaultAsync(x => x.DoanhNghiepQuanLyId == dn.Id && x.MaNghe == MaNghe);
                ViewData["DonViQuanLyId"] = lvkd?.DonViQuanLyId ?? Guid.Empty;
            }

            var dmKinhDoanh = await _dbContext.DanhMucKinhDoanhs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaNghe == MaNghe);
            var tenNghe = dmKinhDoanh?.TenNghe ?? "";

            if (ViewData["Role"] == null || string.IsNullOrEmpty(ViewData["Role"]?.ToString()))
            {
                ViewData["Role"] = "KeKhaiDangKyGia";
            }

            ViewData["DoanhNghieps"] = doanhNghieps;
            ViewData["DoanhNghiep"] = dn;
            ViewData["Filter"] = filter;
            ViewData["MaNghe"] = MaNghe;
            ViewData["TenNghe"] = tenNghe;
            ViewData["DoanhNghiepQuanLyId"] = DoanhNghiepQuanLyId;

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search ?? "", filter.PageSize, filter.PageCurrent, (List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>)(response.Data ?? new List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>()));
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpGet("Create")]
        [AuthorizeAction(nameof(Create), "KeKhaiDangKyGia")]
        public async Task<IActionResult> Create(Guid DoanhNghiepQuanLyId, string MaNghe)
        {
            try
            {
                var response = await _keKhaiDangKyGiaService.CreateAsync(DoanhNghiepQuanLyId, MaNghe);
                if (response.Status == "error")
                {
                    TempData["Error"] = response.Message;
                    return RedirectToAction(nameof(Index), new { DoanhNghiepQuanLyId = DoanhNghiepQuanLyId, MaNghe = MaNghe });
                }

                var model = (DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia)response.Data;
                ViewData["Role"] = "KeKhaiDangKyGia";
                ViewData["DoanhNghiepQuanLyId"] = DoanhNghiepQuanLyId;
                ViewData["MaNghe"] = MaNghe;

                var hasModelLk = await _dbContext.KeKhaiDangKyGias.AnyAsync(t => t.MaNghe == MaNghe && t.DoanhNghiepQuanLyId == DoanhNghiepQuanLyId && (t.TrangThai == "DD" || t.TrangThai == "CB"));
                ViewData["HasModelLk"] = hasModelLk;

                var dn = await _dbContext.DoanhNghieps
                    .FirstOrDefaultAsync(x => x.Id == DoanhNghiepQuanLyId);
                ViewData["DoanhNghiep"] = dn;

                return View(ViewPath("CreateOrEdit"), model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khởi tạo: " + ex.Message;
                return RedirectToAction(nameof(Index), new { DoanhNghiepQuanLyId = DoanhNghiepQuanLyId, MaNghe = MaNghe });
            }
        }

        [HttpPost("Store")]
        [AuthorizeAction(nameof(Create), "KeKhaiDangKyGia")]
        public async Task<IActionResult> Store(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia request)
        {
            var response = await _keKhaiDangKyGiaService.StoreAsync(request);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
            }
            return RedirectToAction(nameof(Index), new { DoanhNghiepQuanLyId = request.DoanhNghiepQuanLyId, MaNghe = request.MaNghe });
        }

        [HttpGet("Edit")]
        [AuthorizeAction(nameof(Edit), "KeKhaiDangKyGia")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var response = await _keKhaiDangKyGiaService.EditAsync(id);
                if (response.Status == "error")
                {
                    TempData["Error"] = response.Message;
                    return RedirectToAction(nameof(Index));
                }

                var model = (DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia)response.Data;
                ViewData["Role"] = "KeKhaiDangKyGia";
                ViewData["DoanhNghiepQuanLyId"] = model.DoanhNghiepQuanLyId;
                ViewData["MaNghe"] = model.MaNghe;

                var hasModelLk = await _dbContext.KeKhaiDangKyGias.AnyAsync(t => t.MaNghe == model.MaNghe && t.DoanhNghiepQuanLyId == model.DoanhNghiepQuanLyId && (t.TrangThai == "DD" || t.TrangThai == "CB") && t.Id != model.Id);
                ViewData["HasModelLk"] = hasModelLk;

                var dn = await _dbContext.DoanhNghieps
                    .FirstOrDefaultAsync(x => x.Id == model.DoanhNghiepQuanLyId);
                ViewData["DoanhNghiep"] = dn;

                return View(ViewPath("CreateOrEdit"), model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi sửa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Update")]
        [AuthorizeAction(nameof(Edit), "KeKhaiDangKyGia")]
        public async Task<IActionResult> Update(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia request)
        {
            var response = await _keKhaiDangKyGiaService.UpdateAsync(request);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
            }
            return RedirectToAction(nameof(Index), new { DoanhNghiepQuanLyId = request.DoanhNghiepQuanLyId, MaNghe = request.MaNghe });
        }

        [HttpPost("Delete")]
        [AuthorizeAction(nameof(Delete), "KeKhaiDangKyGia")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete, Guid DoanhNghiepQuanLyId, string MaNghe)
        {
            var response = await _keKhaiDangKyGiaService.DeleteAsync(id_delete);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "KeKhaiDangKyGia";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "KeKhaiDangKyGia", new { DoanhNghiepQuanLyId = DoanhNghiepQuanLyId, MaNghe = MaNghe });
        }

        [HttpGet("GetCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> GetCodeExcel(string MaHoSo)
        {
            var response = await _keKhaiDangKyGiaService.GetCodeExcelAsync(MaHoSo);
            if (response.Status == "success")
            {
                return Content(response.Data, "application/json");
            }
            return BadRequest(response.Message);
        }

        [HttpPost("SaveCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> SaveCodeExcel(string MaHoSo)
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var jsonString = await reader.ReadToEndAsync();
                var response = await _keKhaiDangKyGiaService.SaveCodeExcelAsync(MaHoSo, jsonString);
                return Json(new { success = response.Status == "success" });
            }
        }

        [HttpGet("Show")]
        [AuthorizeAction(nameof(Index), "KeKhaiDangKyGia")]
        public async Task<IActionResult> Show(Guid id)
        {
            var response = await _keKhaiDangKyGiaService.GetSingleByIdAsync(id);
            if (response.Status == "error" || response.Data == null)
            {
                return NotFound();
            }

            var model = (DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia)response.Data;
            ViewData["Role"] = "KeKhaiDangKyGia";
            ViewData["MaNghe"] = model.MaNghe ?? "";

            var detailsResponse = await _keKhaiDangKyGiaService.GetDetailsByMaHoSoAsync(model.MaHoSo ?? "");
            ViewData["Details"] = detailsResponse.Data;

            var dmDonVi = await _dbContext.DanhMucDonVis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.DonViQuanLyId);
            ViewData["DonViQuanLy"] = dmDonVi;

            return View(ViewPath("Show"), model);
        }

        [HttpPost("Chuyen")]
        [AuthorizeAction(nameof(Edit), "KeKhaiDangKyGia")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chuyen(Guid hoSoId, Guid donViQuanLyId, string? thongTinNguoiChuyen, string? soDtNguoiChuyen)
        {
            var response = await _keKhaiDangKyGiaService.ChuyenAsync(hoSoId, donViQuanLyId, thongTinNguoiChuyen, soDtNguoiChuyen);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpGet("GetSoLuongKeKhaiTheoThang")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetSoLuongKeKhaiTheoThang()
        {
            var response = await _keKhaiDangKyGiaService.GetKeKhaiDangKyGiaStatsAsync();
            return Json(new { status = response.Status == "success" ? "success" : "error", message = response.Message, data = response.Data });
        }
    }
}
