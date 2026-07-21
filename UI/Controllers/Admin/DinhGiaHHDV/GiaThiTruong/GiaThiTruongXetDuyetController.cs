using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.DinhGiaHHDV.GiaThiTruong;
using Services.Systems;
using Services.Settings;
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
    public class GiaThiTruongXetDuyetController(
        IAuthService authService,
        IGiaThiTruongXetDuyetService giaThiTruongXetDuyetService,
        IGiaThiTruongDanhMucService giaThiTruongDanhMucService,
        IDanhMucDonViService danhMucDonViService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IGiaThiTruongXetDuyetService _giaThiTruongXetDuyetService = giaThiTruongXetDuyetService;
        private readonly IGiaThiTruongDanhMucService _giaThiTruongDanhMucService = giaThiTruongDanhMucService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/DinhGiaHHDV/GiaThiTruong/XetDuyet/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index(string Year, string Thang, string Search, int PageSize = 5, int PageCurrent = 1)
        {
            var userInfo = _authService.GetUserInfo();
            bool isSSA = userInfo?.SSA ?? false;
            Guid donViId = userInfo?.DanhMucDonViId ?? Guid.Empty;

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

            // Parse Thang
            string filterThang = string.IsNullOrEmpty(Thang) ? "all" : Thang;

            var response = await _giaThiTruongXetDuyetService.GetListXetDuyetByFilterAsync(
                filterYear, filterThang, donViId, isSSA, Search, PageSize, PageCurrent);

            // Get all units for displaying creator unit names in the view
            ViewData["DanhMucDonVi"] = await _dbContext.DanhMucDonVis.AsNoTracking().ToListAsync();

            // Get GiaThiTruongDanhMuc list to display categories
            var dmResponse = await _giaThiTruongDanhMucService.GetListGiaThiTruongDanhMucAsync("", 1000, 1);
            ViewData["GiaThiTruongDanhMuc"] = dmResponse.Data;

            ViewData["Year"] = filterYear;
            ViewData["Thang"] = filterThang;
            ViewData["Search"] = Search;
            ViewData["PageSize"] = PageSize;
            ViewData["PageCurrent"] = PageCurrent;

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, Search, PageSize, PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(Guid hoSoId)
        {
            var response = await _giaThiTruongXetDuyetService.DuyetAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDuyet(Guid hoSoId)
        {
            var response = await _giaThiTruongXetDuyetService.HuyDuyetAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraLai(Guid hoSoId, string Lydo)
        {
            var response = await _giaThiTruongXetDuyetService.TraLaiAsync(hoSoId, Lydo);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CongBo(Guid hoSoId)
        {
            var response = await _giaThiTruongXetDuyetService.CongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyCongBo(Guid hoSoId)
        {
            var response = await _giaThiTruongXetDuyetService.HuyCongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }
    }
}
