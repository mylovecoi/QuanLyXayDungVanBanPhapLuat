using DataAccess;
using DataAccess.Entities.ThamDinhGia;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.ThamDinhGia;
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

namespace UI.Controllers.Admin.ThamDinhGia
{
    [SetViewDataFilter]
    public class ThamDinhGiaXetDuyetController(
        IAuthService authService,
        IThamDinhGiaXetDuyetService thamDinhGiaXetDuyetService,
        IDanhMucDonViService danhMucDonViService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IThamDinhGiaXetDuyetService _thamDinhGiaXetDuyetService = thamDinhGiaXetDuyetService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/ThamDinhGia/XetDuyet/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index(string Year, string Search, int PageSize = 5, int PageCurrent = 1)
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

            var response = await _thamDinhGiaXetDuyetService.GetListXetDuyetByFilterAsync(
                filterYear, donViId, isSSA, Search, PageSize, PageCurrent);

            // Get all units for displaying creator unit names in the view
            ViewData["DanhMucDonVi"] = await _dbContext.DanhMucDonVis.AsNoTracking().ToListAsync();
            ViewData["DanhMucDonViThamDinh"] = await _dbContext.ThamDinhGiaDanhMucDonVis.AsNoTracking().ToListAsync();

            ViewData["Year"] = filterYear;
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
            var response = await _thamDinhGiaXetDuyetService.DuyetAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDuyet(Guid hoSoId)
        {
            var response = await _thamDinhGiaXetDuyetService.HuyDuyetAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraLai(Guid hoSoId, string Lydo)
        {
            var response = await _thamDinhGiaXetDuyetService.TraLaiAsync(hoSoId, Lydo);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CongBo(Guid hoSoId)
        {
            var response = await _thamDinhGiaXetDuyetService.CongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyCongBo(Guid hoSoId)
        {
            var response = await _thamDinhGiaXetDuyetService.HuyCongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }
    }
}
