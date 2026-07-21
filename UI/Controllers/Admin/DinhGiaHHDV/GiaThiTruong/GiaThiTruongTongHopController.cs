using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DinhGiaHHDV.GiaThiTruong;
using Services.Settings;
using Services.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.DinhGiaHHDV.GiaThiTruong
{
    [SetViewDataFilter]
    public class GiaThiTruongTongHopController(
        IAuthService authService,
        IGiaThiTruongTongHopService giaThiTruongTongHopService,
        IGiaThiTruongService giaThiTruongService,
        IGiaThiTruongDanhMucService giaThiTruongDanhMucService,
        IDanhMucDonViService danhMucDonViService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IGiaThiTruongTongHopService _giaThiTruongTongHopService = giaThiTruongTongHopService;
        private readonly IGiaThiTruongService _giaThiTruongService = giaThiTruongService;
        private readonly IGiaThiTruongDanhMucService _giaThiTruongDanhMucService = giaThiTruongDanhMucService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/DinhGiaHHDV/GiaThiTruong/TongHop/{viewName}";

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

            var response = await _giaThiTruongTongHopService.GetListByFilterAsync(filterYear, filterThang, filterDonViId, Search, PageSize, PageCurrent);

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

        [HttpGet]
        public async Task<IActionResult> GetGiaThiTruongRecords(Guid thongTuId, string thang, string nam)
        {
            var query = _dbContext.GiaThiTruongs.AsQueryable();

            if (thongTuId != Guid.Empty)
            {
                query = query.Where(x => x.ThongTuId == thongTuId);
            }

            if (!string.IsNullOrEmpty(thang) && thang != "all")
            {
                query = query.Where(x => x.Thang == thang);
            }

            if (!string.IsNullOrEmpty(nam) && nam != "all")
            {
                query = query.Where(x => x.Nam == nam);
            }

            // Condition: TrangThai is DD (Đã duyệt) or CB (Công bố)
            query = query.Where(x => x.TrangThai == "DD" || x.TrangThai == "CB");

            var data = await query.ToListAsync();

            var result = new List<object>();
            foreach (var item in data)
            {
                var unit = await _dbContext.DanhMucDonVis.FindAsync(item.DonViQuanLyId);
                var area = await _dbContext.DanhMucDiaDanhs.FindAsync(item.DiaBanId);

                result.Add(new
                {
                    item.Id,
                    item.MaHoSo,
                    item.SoQd,
                    ThoiDiem = item.Thoidiem.ToString("dd/MM/yyyy"),
                    TenDonVi = unit?.TenDonVi ?? "",
                    TenDiaBan = area?.TenDiaDanh ?? ""
                });
            }

            return Json(result);
        }

        [HttpGet]
        [AuthorizeAction(nameof(Create))]
        public async Task<IActionResult> Create(Guid thongTuId, string thang, string nam, Guid donViId, string[] hoso)
        {
            if (hoso == null || hoso.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một hồ sơ để tổng hợp!";
                return RedirectToAction(nameof(Index));
            }

            var response = await _giaThiTruongTongHopService.CreateAsync(thongTuId, donViId, thang, nam, hoso);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = (GiaThiTruongTongHop)response.Data!;
            var details = await (from ct in _dbContext.GiaThiTruongTongHopCts.Where(x => x.MaHoSo == model.MaHoSo)
                                 join dm in _dbContext.GiaThiTruongDanhMucCts.Where(x => x.ThongTuId == model.ThongTuId && x.TheoDoi == "TD") on ct.MaHhDv equals dm.MaHhDv
                                 select new GiaThiTruongTongHopCt
                                 {
                                     Id = ct.Id,
                                     MaHoSo = ct.MaHoSo,
                                     ThongTuId = ct.ThongTuId,
                                     MaHhDv = ct.MaHhDv,
                                     TenHhDv = dm.TenHhDv,
                                     DacDiemKt = dm.DacDiemKt,
                                     DonViTinh = dm.DonViTinh,
                                     GiaKyTruoc = ct.GiaKyTruoc,
                                     GiaKyNay = ct.GiaKyNay,
                                     TrangThai = ct.TrangThai,
                                     STTSapXep = ct.STTSapXep
                                 }).OrderBy(x => x.STTSapXep != null ? x.STTSapXep.Length : 0)
                                   .ThenBy(x => x.STTSapXep)
                                   .ToListAsync();

            ViewData["Details"] = details;

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
        public async Task<IActionResult> Store(GiaThiTruongTongHop request, List<GiaThiTruongTongHopCt> details)
        {
            if (string.IsNullOrWhiteSpace(request.SoBc))
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                ViewData["Details"] = details;
                TempData["Error"] = "Số báo cáo không được để trống!";
                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _giaThiTruongTongHopService.StoreAsync(request, details);
            if (response.Status == "error")
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                ViewData["Details"] = details;
                TempData["Error"] = response.Message;
                return View(ViewPath("CreateOrEdit"), request);
            }

            return RedirectToAction(nameof(Index), new { DonViId = request.DonViQuanLyId });
        }

        [HttpGet]
        [AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _giaThiTruongTongHopService.EditAsync(id);
            if (response.Status == "error")
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = (GiaThiTruongTongHop)response.Data!;
            var details = await (from ct in _dbContext.GiaThiTruongTongHopCts.Where(x => x.MaHoSo == model.MaHoSo)
                                 join dm in _dbContext.GiaThiTruongDanhMucCts.Where(x => x.ThongTuId == model.ThongTuId && x.TheoDoi == "TD") on ct.MaHhDv equals dm.MaHhDv
                                 select new GiaThiTruongTongHopCt
                                 {
                                     Id = ct.Id,
                                     MaHoSo = ct.MaHoSo,
                                     ThongTuId = ct.ThongTuId,
                                     MaHhDv = ct.MaHhDv,
                                     TenHhDv = dm.TenHhDv,
                                     DacDiemKt = dm.DacDiemKt,
                                     DonViTinh = dm.DonViTinh,
                                     GiaKyTruoc = ct.GiaKyTruoc,
                                     GiaKyNay = ct.GiaKyNay,
                                     TrangThai = ct.TrangThai,
                                     STTSapXep = ct.STTSapXep
                                 }).OrderBy(x => x.STTSapXep != null ? x.STTSapXep.Length : 0)
                                   .ThenBy(x => x.STTSapXep)
                                   .ToListAsync();

            ViewData["Details"] = details;

            // Load DonVi info
            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == model.DonViQuanLyId);

            // Load Category info
            var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(model.ThongTuId);
            ViewData["DanhMucThongTu"] = dmCategory;

            return View(ViewPath("CreateOrEdit"), model);
        }

        [HttpPost]
        [AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(GiaThiTruongTongHop request, List<GiaThiTruongTongHopCt> details)
        {
            if (string.IsNullOrWhiteSpace(request.SoBc))
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                ViewData["Details"] = details;
                TempData["Error"] = "Số báo cáo không được để trống!";
                return View(ViewPath("CreateOrEdit"), request);
            }

            var response = await _giaThiTruongTongHopService.UpdateAsync(request, details);
            if (response.Status == "error")
            {
                var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
                ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == request.DonViQuanLyId);
                var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.ThongTuId);
                ViewData["DanhMucThongTu"] = dmCategory;
                ViewData["Details"] = details;
                TempData["Error"] = response.Message;
                return View(ViewPath("CreateOrEdit"), request);
            }

            return RedirectToAction(nameof(Index), new { DonViId = request.DonViQuanLyId });
        }

        [HttpPost]
        [AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _giaThiTruongTongHopService.DeleteAsync(id);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpGet]
        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Show(Guid id)
        {
            var record = await _dbContext.GiaThiTruongTongHops.FindAsync(id);
            if (record == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ!";
                return RedirectToAction(nameof(Index));
            }

            var details = await (from ct in _dbContext.GiaThiTruongTongHopCts.Where(x => x.MaHoSo == record.MaHoSo)
                                 join dm in _dbContext.GiaThiTruongDanhMucCts.Where(x => x.ThongTuId == record.ThongTuId && x.TheoDoi == "TD") on ct.MaHhDv equals dm.MaHhDv
                                 select new GiaThiTruongTongHopCt
                                 {
                                     Id = ct.Id,
                                     MaHoSo = ct.MaHoSo,
                                     ThongTuId = ct.ThongTuId,
                                     MaHhDv = ct.MaHhDv,
                                     TenHhDv = dm.TenHhDv,
                                     DacDiemKt = dm.DacDiemKt,
                                     DonViTinh = dm.DonViTinh,
                                     GiaKyTruoc = ct.GiaKyTruoc,
                                     GiaKyNay = ct.GiaKyNay,
                                     TrangThai = ct.TrangThai,
                                     STTSapXep = ct.STTSapXep
                                 }).OrderBy(x => x.STTSapXep != null ? x.STTSapXep.Length : 0)
                                   .ThenBy(x => x.STTSapXep)
                                   .ToListAsync();

            ViewData["Details"] = details;

            var dmDonViData = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVi"] = dmDonViData?.FirstOrDefault(x => x.Id == record.DonViQuanLyId);

            var dmCategory = await _dbContext.GiaThiTruongDanhMucs.FindAsync(record.ThongTuId);
            ViewData["DanhMucThongTu"] = dmCategory;

            return View(ViewPath("Show"), record);
        }

        [HttpGet("GiaThiTruongTongHop/GetCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> GetCodeExcel(string MaHoSo)
        {
            var response = await _giaThiTruongTongHopService.GetCodeExcelAsync(MaHoSo);
            return Content(response.Data?.ToString() ?? "{}", "application/json");
        }

        [HttpPost("GiaThiTruongTongHop/SaveCodeExcel/{MaHoSo}")]
        public async Task<IActionResult> SaveCodeExcel(string MaHoSo, [FromBody] object data)
        {
            var response = await _giaThiTruongTongHopService.SaveCodeExcelAsync(MaHoSo, data.ToString() ?? "{}");
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }
    }
}
