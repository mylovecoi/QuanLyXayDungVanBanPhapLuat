using DataAccess;
using DataAccess.Entities.ThamDinhGia;
using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Services.ThamDinhGia;
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

namespace UI.Controllers.Admin.ThamDinhGia
{
    [SetViewDataFilter]
    public class ThamDinhGiaController(
        IAuthService authService,
        IThamDinhGiaService thamDinhGiaService,
        IThamDinhGiaDanhMucHangHoaService thamDinhGiaDanhMucHangHoaService,
        IDanhMucDonViService danhMucDonViService,
        IAttachedFileService attachedFileService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IThamDinhGiaService _thamDinhGiaService = thamDinhGiaService;
        private readonly IThamDinhGiaDanhMucHangHoaService _thamDinhGiaDanhMucHangHoaService = thamDinhGiaDanhMucHangHoaService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IAttachedFileService _attachedFileService = attachedFileService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/ThamDinhGia/DanhSach/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index(string Year, string DonViId, string Search, int PageSize = 5, int PageCurrent = 1)
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

            var response = await _thamDinhGiaService.GetListByFilterAsync(filterYear, filterDonViId, Search, PageSize, PageCurrent);
            
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
            
            // Get ThamDinhGiaDanhMucHangHoa list for the add new modal
            var dmResponse = await _thamDinhGiaDanhMucHangHoaService.GetListThamDinhGiaDanhMucHangHoaAsync("", 1000, 1);
            ViewData["DanhMucHangHoa"] = dmResponse.Data;
            ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();

            ViewData["Year"] = filterYear;
            ViewData["DonViId"] = filterDonViId;
            ViewData["Search"] = Search;
            ViewData["PageSize"] = PageSize;
            ViewData["PageCurrent"] = PageCurrent;

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, Search, PageSize, PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Create))]
        public async Task<IActionResult> Create(Guid hangHoaId, Guid donViId, string phanLoai)
        {
            await _attachedFileService.RemoveDatarRedundantAsync();

            var response = await _thamDinhGiaService.CreateAsync(hangHoaId, donViId, phanLoai);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = (DataAccess.Entities.ThamDinhGia.ThamDinhGia)response.Data!;
            model.AttachedFiles = [];
            
            // Load DiaBan list
            ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
            
            // Load DonVi info
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == donViId);
            
            // Load Category info
            var dmCategory = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(hangHoaId);
            ViewData["DanhMucHangHoa"] = dmCategory;

            // Load DonViThamDinh list
            ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
            ViewData["DanhMucHoiDong"] = await _dbContext.ThamDinhGiaHoiDongs.ToListAsync();

            return View(ViewPath("CreateOrEdit"), model);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Create))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DataAccess.Entities.ThamDinhGia.ThamDinhGia request)
        {
            if (string.IsNullOrWhiteSpace(request.SoTbKl))
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var maHoSoGuid = request.Id;
                var firstDetail = await _dbContext.ThamDinhGiaCts.FirstOrDefaultAsync(x => x.MaHoSo == maHoSoGuid);
                if (firstDetail != null)
                {
                    ViewData["DanhMucHangHoa"] = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(firstDetail.HangHoaId);
                }
                ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
                ViewData["DanhMucHoiDong"] = await _dbContext.ThamDinhGiaHoiDongs.ToListAsync();
                TempData["Error"] = "Số thông báo kết luận không được để trống!";
                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _thamDinhGiaService.StoreAsync(request);
            if (response.Status == "error")
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var maHoSoGuid = request.Id;
                var firstDetail = await _dbContext.ThamDinhGiaCts.FirstOrDefaultAsync(x => x.MaHoSo == maHoSoGuid);
                if (firstDetail != null)
                {
                    ViewData["DanhMucHangHoa"] = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(firstDetail.HangHoaId);
                }
                ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
                ViewData["DanhMucHoiDong"] = await _dbContext.ThamDinhGiaHoiDongs.ToListAsync();
                TempData["Error"] = response.Message;
                return View(ViewPath("CreateOrEdit"), request);
            }

            await _attachedFileService.UpdateRangeStatus(request.Id, "ThamDinhGia");

            return RedirectToAction(nameof(Index), new { DonViId = request.DonViQuanLyId });
        }

        [HttpGet]
        [AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _thamDinhGiaService.EditAsync(id);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = (DataAccess.Entities.ThamDinhGia.ThamDinhGia)response.Data!;
            var attachedFiles = await _attachedFileService.GetAllAttachedFilesAsync(model.Id, "ThamDinhGia");
            model.AttachedFiles = attachedFiles ?? [];
            
            ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == model.DonViQuanLyId);
            
            var maHoSoGuid = model.Id;
            var firstDetail = await _dbContext.ThamDinhGiaCts.FirstOrDefaultAsync(x => x.MaHoSo == maHoSoGuid);
            if (firstDetail != null)
            {
                ViewData["DanhMucHangHoa"] = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(firstDetail.HangHoaId);
            }

            ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
            ViewData["DanhMucHoiDong"] = await _dbContext.ThamDinhGiaHoiDongs.ToListAsync();
            return View(ViewPath("CreateOrEdit"), model);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DataAccess.Entities.ThamDinhGia.ThamDinhGia request)
        {
            if (string.IsNullOrWhiteSpace(request.SoTbKl))
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var maHoSoGuid = request.Id;
                var firstDetail = await _dbContext.ThamDinhGiaCts.FirstOrDefaultAsync(x => x.MaHoSo == maHoSoGuid);
                if (firstDetail != null)
                {
                    ViewData["DanhMucHangHoa"] = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(firstDetail.HangHoaId);
                }
                ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
                ViewData["DanhMucHoiDong"] = await _dbContext.ThamDinhGiaHoiDongs.ToListAsync();
                TempData["Error"] = "Số thông báo kết luận không được để trống!";
                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _thamDinhGiaService.UpdateAsync(request);
            if (response.Status == "error")
            {
                ViewData["DanhMucDiaDanh"] = await _dbContext.DanhMucDiaDanhs.OrderBy(x => x.STTSapXep).ToListAsync();
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var maHoSoGuid = request.Id;
                var firstDetail = await _dbContext.ThamDinhGiaCts.FirstOrDefaultAsync(x => x.MaHoSo == maHoSoGuid);
                if (firstDetail != null)
                {
                    ViewData["DanhMucHangHoa"] = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(firstDetail.HangHoaId);
                }
                ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
                ViewData["DanhMucHoiDong"] = await _dbContext.ThamDinhGiaHoiDongs.ToListAsync();
                TempData["Error"] = response.Message;
                return View(ViewPath("CreateOrEdit"), request);
            }

            await _attachedFileService.UpdateRangeStatus(request.Id, "ThamDinhGia");

            return RedirectToAction(nameof(Index), new { DonViId = request.DonViQuanLyId });
        }

        [HttpPost]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chuyen(Guid hoSoId, string trangThai)
        {
            var response = await _thamDinhGiaService.ChuyenAsync(hoSoId, trangThai);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _thamDinhGiaService.DeleteAsync(id);
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
            var response = await _thamDinhGiaService.EditAsync(hoSoId);
            if (response.Status == "error" || response.Data == null)
            {
                return NotFound();
            }

            var model = (DataAccess.Entities.ThamDinhGia.ThamDinhGia)response.Data;
            
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == model.DonViQuanLyId);
            ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.ToListAsync();
            ViewData["DanhMucHoiDong"] = await _dbContext.ThamDinhGiaHoiDongs.ToListAsync();

            var detailsResponse = await _thamDinhGiaService.GetDetailsByMaHoSoAsync(model.Id.ToString());
            ViewData["Details"] = detailsResponse.Data;

            return View(ViewPath("Show"), model);
        }

        [HttpGet("ThamDinhGia/GetCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> GetCodeExcel(string MaHoSo)
        {
            var response = await _thamDinhGiaService.GetCodeExcelAsync(MaHoSo);
            if (response.Status == "success")
            {
                return Content(response.Data, "application/json");
            }
            return BadRequest(response.Message);
        }

        [HttpPost("ThamDinhGia/SaveCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> SaveCodeExcel(string MaHoSo)
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var jsonString = await reader.ReadToEndAsync();
                var response = await _thamDinhGiaService.SaveCodeExcelAsync(MaHoSo, jsonString);
                return Json(new { success = response.Status == "success" });
            }
        }

        [HttpGet("ThamDinhGia/GetSoLuongThamDinhGiaTheoThang")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetSoLuongThamDinhGiaTheoThang()
        {
            var response = await _thamDinhGiaService.GetThamDinhGiaStatsAsync();
            return Json(new { status = response.Status == "success" ? "success" : "error", message = response.Message, data = response.Data });
        }
    }
}
