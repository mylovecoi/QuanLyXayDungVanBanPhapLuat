using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Services.Systems;
using Services.KeKhaiDangKyGia;
using Services.DTOs.KeKhaiDangKyGia;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.KeKhaiDangKyGia
{
    [Route("KeKhaiDangKyGiaTheoDoi")]
    [SetViewDataFilter]
    public class KeKhaiDangKyGiaTheoDoiController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IKeKhaiDangKyGiaTheoDoiService _keKhaiDangKyGiaTheoDoiService;

        public KeKhaiDangKyGiaTheoDoiController(
            ApplicationDbContext dbContext, 
            IAuthService authService,
            IKeKhaiDangKyGiaTheoDoiService keKhaiDangKyGiaTheoDoiService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _keKhaiDangKyGiaTheoDoiService = keKhaiDangKyGiaTheoDoiService;
        }

        private string ViewPath(string viewName) => $"../Admin/KeKhaiDangKyGia/TheoDoi/{viewName}";

        [HttpGet("")]
        [AuthorizeAction(nameof(Index), "KeKhaiDangKyGia")]
        public async Task<IActionResult> Index()
        {
            var filter = new KeKhaiDangKyGiaFilter(Request);
            if (!Request.Query.ContainsKey("TrangThai"))
            {
                filter.TrangThai = "all";
            }

            List<DanhMucDonVi> listDonVi = await _dbContext.DanhMucDonVis
                .AsNoTracking()
                .OrderBy(x => x.Level)
                .ThenBy(x => x.STTSapXep)
                .ToListAsync();

            List<DanhMucKinhDoanh> listKinhDoanhNganh = await _dbContext.DanhMucKinhDoanhs
                .AsNoTracking()
                .Where(x => x.PhanLoai == "Group" && x.LoaiGia == "KKG")
                .OrderBy(x => x.STTSapXep)
                .ToListAsync();

            List<DanhMucKinhDoanh> listKinhDoanhNghe = await _dbContext.DanhMucKinhDoanhs
                .AsNoTracking()
                .Where(x => x.PhanLoai == "Detail" && x.LoaiGia == "KKG")
                .OrderBy(x => x.STTSapXep)
                .ToListAsync();

            // Fetch records via service
            var response = await _keKhaiDangKyGiaTheoDoiService.GetListTheoDoiByFilterAsync(filter);

            var dataList = (List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>)(response.Data ?? new List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>());

            // Prepare ViewData
            ViewData["DanhMucDonVi"] = listDonVi;
            ViewData["DanhMucKinhDoanhNganh"] = listKinhDoanhNganh;
            ViewData["DanhMucKinhDoanhNghe"] = listKinhDoanhNghe;
            ViewData["Year"] = filter.TargetYear;
            ViewData["SelectedDonVi"] = filter.DonViQuanLyId == Guid.Empty ? "all" : filter.DonViQuanLyId.ToString();
            ViewData["SelectedMaNghe"] = filter.MaNghe ?? "all";
            ViewData["SelectedTrangThai"] = filter.TrangThai ?? "all";
            ViewData["Search"] = filter.Search ?? "";
            ViewData["Role"] = "KeKhaiDangKyGia"; // Base permission role

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search ?? "", filter.PageSize, filter.PageCurrent, dataList);

            return View(ViewPath("Index"), pageInfo);
        }
    }
}
