using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.Settings;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.DinhGiaHHDV.GiaThiTruong;
using Services.Settings;
using Services.Systems;
using Services.Manages;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using UI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace UI.Controllers.Admin.DinhGiaHHDV.GiaThiTruong
{
    [SetViewDataFilter]
    public class GiaThiTruongController(
        IAuthService authService,
        IGiaThiTruongService giaThiTruongService,
        IGiaThiTruongDanhMucService giaThiTruongDanhMucService,
        IDanhMucDonViService danhMucDonViService,
        IAttachedFileService attachedFileService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IGiaThiTruongService _giaThiTruongService = giaThiTruongService;
        private readonly IGiaThiTruongDanhMucService _giaThiTruongDanhMucService = giaThiTruongDanhMucService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IAttachedFileService _attachedFileService = attachedFileService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/DinhGiaHHDV/GiaThiTruong/DanhSach/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index(string Year, string Thang, string DonViId, string Search, int PageSize = 5, int PageCurrent = 1)
        {
            var userInfo = _authService.GetUserInfo();
            
            // Parse DonViId
            Guid filterDonViId = Guid.Empty;
            if (userInfo == null || !userInfo.SSA)
            {
                filterDonViId = userInfo?.DanhMucDonViId ?? Guid.Empty;
            }
            else if (string.IsNullOrEmpty(DonViId))
            {
                // First load for SSA, default to their own unit
                filterDonViId = userInfo.DanhMucDonViId;
            }
            else if (DonViId != "all")
            {
                Guid.TryParse(DonViId, out filterDonViId);
            }

            // Parse Year
            int filterYear = 0;
            if (string.IsNullOrEmpty(Year))
            {
                filterYear = DateTime.Now.Year;
            }
            else if (Year != "all")
            {
                int.TryParse(Year, out filterYear);
            }

            string filterThang = string.IsNullOrEmpty(Thang) ? "all" : Thang;

            var response = await _giaThiTruongService.GetListByFilterAsync(filterYear, filterThang, filterDonViId, Search, PageSize, PageCurrent);
            
            // Get DanhMucDonVi list for filtering
            if (userInfo != null && userInfo.SSA)
            {
                ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            }
            else
            {
                var userDonViId = userInfo?.DanhMucDonViId ?? Guid.Empty;
                ViewData["DanhMucDonVi"] = await _dbContext.DanhMucDonVis.Where(x => x.Id == userDonViId).ToListAsync();
            }
            
            // Get GiaThiTruongDanhMuc list for the add new modal
            var dmResponse = await _giaThiTruongDanhMucService.GetListGiaThiTruongDanhMucAsync("", 1000, 1);
            ViewData["GiaThiTruongDanhMuc"] = dmResponse.Data;

            ViewData["Year"] = filterYear;
            ViewData["Thang"] = filterThang;
            ViewData["DonViId"] = filterDonViId;
            ViewData["Search"] = Search;
            ViewData["PageSize"] = PageSize;
            ViewData["PageCurrent"] = PageCurrent;

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, Search, PageSize, PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Create))]
        public async Task<IActionResult> Create(Guid thongTuId, Guid donViId, string thang, string nam)
        {
            await _attachedFileService.RemoveDatarRedundantAsync();

            var response = await _giaThiTruongService.CreateAsync(thongTuId, donViId, thang, nam);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = (DataAccess.Entities.DinhGiaHHDV.GiaThiTruong)response.Data!;
            model.AttachedFiles = [];
            
            // Load DiaBan list
            ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
            
            // Load DonVi info
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == donViId);
            
            // Load Category info
            var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(thongTuId);
            ViewData["DanhMucThongTu"] = dmCategory;

            return View(ViewPath("CreateOrEdit"), model);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Create))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DataAccess.Entities.DinhGiaHHDV.GiaThiTruong request)
        {
            if (string.IsNullOrWhiteSpace(request.SoQd))
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                TempData["Error"] = "Số văn bản không được để trống!";
                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _giaThiTruongService.StoreAsync(request);
            if (response.Status == "error")
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                TempData["Error"] = response.Message;
                return View(ViewPath("CreateOrEdit"), request);
            }

            await _attachedFileService.UpdateRangeStatus(request.Id, "GiaThiTruong");

            return RedirectToAction(nameof(Index), new { DonViId = request.DonViQuanLyId });
        }

        [HttpGet]
        [AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _giaThiTruongService.EditAsync(id);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = (DataAccess.Entities.DinhGiaHHDV.GiaThiTruong)response.Data!;
            var attachedFiles = await _attachedFileService.GetAllAttachedFilesAsync(model.Id, "GiaThiTruong");
            model.AttachedFiles = attachedFiles ?? [];
            
            ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == model.DonViQuanLyId);
            var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(model.ThongTuId);
            ViewData["DanhMucThongTu"] = dmCategory;

            return View(ViewPath("CreateOrEdit"), model);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DataAccess.Entities.DinhGiaHHDV.GiaThiTruong request)
        {
            if (string.IsNullOrWhiteSpace(request.SoQd))
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                TempData["Error"] = "Số văn bản không được để trống!";
                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _giaThiTruongService.UpdateAsync(request);
            if (response.Status == "error")
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                TempData["Error"] = response.Message;
                return View(ViewPath("CreateOrEdit"), request);
            }

            await _attachedFileService.UpdateRangeStatus(request.Id, "GiaThiTruong");

            return RedirectToAction(nameof(Index), new { DonViId = request.DonViQuanLyId });
        }

        [HttpPost]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chuyen(Guid hoSoId, string trangThai)
        {
            var response = await _giaThiTruongService.ChuyenAsync(hoSoId, trangThai);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _giaThiTruongService.DeleteAsync(id);
            if (response.Status == "success")
            {
                await _attachedFileService.RemoveRangeByGroupId(id);
            }
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpGet]
        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Show(Guid hoSoId)
        {
            var response = await _giaThiTruongService.EditAsync(hoSoId);
            if (response.Status == "error" || response.Data == null)
            {
                return NotFound();
            }

            var model = (DataAccess.Entities.DinhGiaHHDV.GiaThiTruong)response.Data;
            
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == model.DonViQuanLyId);

            var detailsResponse = await _giaThiTruongService.GetDetailsByMaHoSoAsync(model.MaHoSo ?? "");
            ViewData["Details"] = detailsResponse.Data;

            return View(ViewPath("Show"), model);
        }

        [HttpGet("GiaThiTruong/GetCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> GetCodeExcel(string MaHoSo)
        {
            var response = await _giaThiTruongService.GetCodeExcelAsync(MaHoSo);
            if (response.Status == "success")
            {
                return Content(response.Data, "application/json");
            }
            return BadRequest(response.Message);
        }

        [HttpPost("GiaThiTruong/SaveCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> SaveCodeExcel(string MaHoSo)
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var jsonString = await reader.ReadToEndAsync();
                var response = await _giaThiTruongService.SaveCodeExcelAsync(MaHoSo, jsonString);
                return Json(new { success = response.Status == "success" });
            }
        }

        [HttpGet("GiaThiTruong/GetSoLuongGiaThiTruongTheoThang")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetSoLuongGiaThiTruongTheoThang()
        {
            var response = await _giaThiTruongService.GetGiaThiTruongStatsAsync();
            return ReturnJson(response.Status == "success", response.Message, response.Data);
        }
    }
}
