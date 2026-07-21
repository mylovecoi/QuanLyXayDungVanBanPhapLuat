using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DinhGiaHHDV.DinhGiaKhac;

using Services.DTOs.DinhGiaHHDV.ThongTinHoSo;
using Services.Settings;
using Services.Systems;
using System.IO;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.DinhGiaHHDV.DinhGiaKhac
{
    [SetViewDataFilter]
    public class DinhGiaController(
        IAuthService authService,
        IDinhGiaService dinhGiaService,
        IDanhMucDonViService danhMucDonViService,
        DataAccess.ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IDinhGiaService _dinhGiaService = dinhGiaService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly DataAccess.ApplicationDbContext _dbContext = dbContext;
        private string ViewPath(string viewName) => $"../Admin/DinhGiaHHDV/DinhGiaKhac/DanhSach/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index(string MaNghe)
        {
            var filter = new DinhGiaFilter(Request, _authService);
            var response = await _dinhGiaService.GetListByFilterAsync(filter, MaNghe);
            ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["Filter"] = filter;
            ViewData["MaNghe"] = MaNghe;
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpPost("DinhGia/GetCategories")]
        public async Task<IActionResult> GetCategories(string maNghe)
        {
            try
            {
                var type = Type.GetType($"DataAccess.Entities.Settings.DanhMucGia.DanhMuc{maNghe}, DataAccess")
                           ?? Type.GetType($"DataAccess.Entities.Settings.DanhMuc{maNghe}, DataAccess");
                var detType = Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTiet{maNghe}, DataAccess")
                           ?? Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet{maNghe}, DataAccess");
                if (type == null || detType == null)
                {
                    type = typeof(DataAccess.Entities.Settings.DanhMucGia.DanhMucGiaChung);
                }

                var method = typeof(DbContext).GetMethods()
                    .First(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0)
                    .MakeGenericMethod(type);
                var dbSet = (IQueryable)method.Invoke(_dbContext, null)!;

                var list = new List<object>();
                foreach (var item in dbSet)
                {
                    var trangThaiVal = type.GetProperty("TrangThai")?.GetValue(item)?.ToString();
                    if (trangThaiVal == "TD")
                    {
                        list.Add(new
                        {
                            Id = type.GetProperty("Id")?.GetValue(item),
                            MaDanhMuc = type.GetProperty("MaDanhMuc")?.GetValue(item),
                            TenDanhMuc = type.GetProperty("TenDanhMuc")?.GetValue(item),
                            MaNghe = type.GetProperty("MaNghe")?.GetValue(item)
                        });
                    }
                }

                return Json(new { status = "success", data = list });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizeAction(nameof(Create))]
        public async Task<IActionResult> Create(Guid donViId, string MaNghe, Guid? danhMucId)
        {
            var response = await _dinhGiaService.CreateAsync(donViId, MaNghe, danhMucId);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index), new { MaNghe });
            }

            if (response.Data == null)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index), new { MaNghe });
            }

            var model = (DinhGia)response.Data;
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == donViId);
            ViewData["MaNghe"] = MaNghe;
            ViewData["DanhMucId"] = danhMucId;

            var (danhMucTable, chiTietTable) = _dinhGiaService.GetTableNames(MaNghe);
            ViewData["DanhMucTable"] = danhMucTable;
            ViewData["ChiTietTable"] = chiTietTable;

            return View(ViewPath("CreateOrEdit"), model);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Create))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DinhGia request)
        {
            if (string.IsNullOrWhiteSpace(request.SoQd) || string.IsNullOrWhiteSpace(request.MoTa))
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                ViewData["MaNghe"] = request.MaNghe ?? "";
                TempData["Error"] = "Số quyết định và Mô tả không được để trống!";

                var (danhMucTable, chiTietTable) = _dinhGiaService.GetTableNames(request.MaNghe ?? "");
                ViewData["DanhMucTable"] = danhMucTable;
                ViewData["ChiTietTable"] = chiTietTable;

                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _dinhGiaService.StoreAsync(request);
            if (response.Status == "error")
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                ViewData["MaNghe"] = request.MaNghe ?? "";
                TempData["Error"] = response.Message;

                var (danhMucTable, chiTietTable) = _dinhGiaService.GetTableNames(request.MaNghe ?? "");
                ViewData["DanhMucTable"] = danhMucTable;
                ViewData["ChiTietTable"] = chiTietTable;

                return View(ViewPath("CreateOrEdit"), request);
            }
            return RedirectToAction(nameof(Index), new { MaNghe = request.MaNghe, DonViId = request.DonViQuanLyId });
        }

        [HttpGet]
        [AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> Edit(Guid hoSoId)
        {
            var response = await _dinhGiaService.EditAsync(hoSoId);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            if (response.Data == null)
            {
                TempData["Error"] = "Không tìm thấy dữ liệu hồ sơ.";
                return RedirectToAction(nameof(Index));
            }

            var model = (DinhGia)response.Data;
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == model.DonViQuanLyId);
            ViewData["MaNghe"] = model.MaNghe ?? "";

            var (danhMucTable, chiTietTable) = _dinhGiaService.GetTableNames(model.MaNghe ?? "");
            ViewData["DanhMucTable"] = danhMucTable;
            ViewData["ChiTietTable"] = chiTietTable;

            return View(ViewPath("CreateOrEdit"), model);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DinhGia request)
        {
            if (string.IsNullOrWhiteSpace(request.SoQd) || string.IsNullOrWhiteSpace(request.MoTa))
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                ViewData["MaNghe"] = request.MaNghe ?? "";
                TempData["Error"] = "Số quyết định và Mô tả không được để trống!";

                var (danhMucTable, chiTietTable) = _dinhGiaService.GetTableNames(request.MaNghe ?? "");
                ViewData["DanhMucTable"] = danhMucTable;
                ViewData["ChiTietTable"] = chiTietTable;

                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _dinhGiaService.UpdateAsync(request);
            if (response.Status == "error")
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                ViewData["MaNghe"] = request.MaNghe ?? "";
                TempData["Error"] = response.Message;

                var (danhMucTable, chiTietTable) = _dinhGiaService.GetTableNames(request.MaNghe ?? "");
                ViewData["DanhMucTable"] = danhMucTable;
                ViewData["ChiTietTable"] = chiTietTable;

                return View(ViewPath("CreateOrEdit"), request);
            }
            return RedirectToAction(nameof(Index), new { MaNghe = request.MaNghe, DonViId = request.DonViQuanLyId });
        }

        [HttpPost]
        [AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid hoSoId)
        {
            var response = await _dinhGiaService.DeleteAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chuyen(Guid hoSoId, string trangThai)
        {
            var response = await _dinhGiaService.ChuyenAsync(hoSoId, trangThai);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpGet]
        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Show(Guid hoSoId)
        {
            var response = await _dinhGiaService.GetSingleByIdAsync(hoSoId);
            if (response.Status == "error")
            {
                return NotFound();
            }

            if (response.Data == null)
            {
                return NotFound();
            }

            var model = (DinhGia)response.Data;
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == model.DonViQuanLyId);
            ViewData["MaNghe"] = model.MaNghe ?? "";

            var detailsResponse = await _dinhGiaService.GetDetailsByMaHoSoAsync(model.MaHoSo ?? "");
            ViewData["Details"] = detailsResponse.Data;

            return View(ViewPath("Show"), model);
        }

        [HttpGet("DinhGia/GetCodeExcel/{Mahs}")]
        public async Task<IActionResult> GetCodeExcel(string Mahs)
        {
            var response = await _dinhGiaService.GetCodeExcelAsync(Mahs);
            if (response.Status == "success")
            {
                return Content(response.Data, "application/json");
            }
            return BadRequest(response.Message);
        }

        [HttpPost("DinhGia/SaveCodeExcel/{Mahs}")]
        public async Task<IActionResult> SaveCodeExcel(string Mahs)
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var jsonString = await reader.ReadToEndAsync();
                var response = await _dinhGiaService.SaveCodeExcelAsync(Mahs, jsonString);
                return Json(new { success = response.Status == "success" });
            }
        }
    }
}
