using Microsoft.AspNetCore.Mvc;
using Services.KeKhaiDangKyGia;
using Services.DTOs.KeKhaiDangKyGia;
using Services.Settings;
using Services.Settings.DanhMucDungChung;
using Services.Settings.DanhMucGia;
using Services.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using UI.ViewModels;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace UI.Controllers.Admin.KeKhaiDangKyGia
{
    [Route("KeKhaiDangKyGiaXetDuyet")]
    [SetViewDataFilter]
    public class KeKhaiDangKyGiaXetDuyetController(
        IAuthService authService,
        IKeKhaiDangKyGiaXetDuyetService keKhaiDangKyGiaXetDuyetService,
        IDanhMucDonViService danhMucDonViService,
        IDmKinhDoanhService dmKinhDoanhService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IKeKhaiDangKyGiaXetDuyetService _keKhaiDangKyGiaXetDuyetService = keKhaiDangKyGiaXetDuyetService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IDmKinhDoanhService _dmKinhDoanhService = dmKinhDoanhService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/KeKhaiDangKyGia/XetDuyet/{viewName}";

        [HttpGet("")]
        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new KeKhaiDangKyGiaFilter(Request);
            var user = _authService.GetUserInfo();
            if (user != null && !user.SSA)
            {
                filter.SetDonViQuanLyId(user.DanhMucDonViId);
            }

            var response = await _keKhaiDangKyGiaXetDuyetService.GetListXetDuyetByFilterAsync(filter);
            
            List<DataAccess.Entities.Settings.DanhMucDonVi> listDonVi;
            if (user != null && user.SSA)
            {
                listDonVi = await _dbContext.DanhMucDonVis.AsNoTracking()
                    .OrderBy(x => x.Level).ThenBy(x => x.STTSapXep).ToListAsync();
            }
            else
            {
                Guid userDonViId = user?.DanhMucDonViId ?? Guid.Empty;
                listDonVi = await _dbContext.DanhMucDonVis.AsNoTracking()
                    .Where(x => x.Id == userDonViId)
                    .OrderBy(x => x.Level).ThenBy(x => x.STTSapXep).ToListAsync();
            }
            ViewData["DanhMucDonVi"] = listDonVi;

            ViewData["DanhMucKinhDoanh"] = await _keKhaiDangKyGiaXetDuyetService.GetDanhMucKinhDoanhByFilterAsync(filter);
            ViewData["Filter"] = filter;
            ViewData["Role"] = "KeKhaiDangKyGia.XetDuyetHoSo";

            var dataList = (List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>)(response.Data ?? new List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>());
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search ?? "", filter.PageSize, filter.PageCurrent, dataList);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpGet("LayThongTinHoSo")]
        public async Task<IActionResult> LayThongTinHoSo(Guid id)
        {
            var response = await _keKhaiDangKyGiaXetDuyetService.GetSingleByIdAsync(id);
            if (response.Status == "success" && response.Data != null)
            {
                var model = (DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia)response.Data;
                string soHsDuyet = model.SoHsDuyet ?? "";
                if (string.IsNullOrEmpty(soHsDuyet))
                {
                    var list = await _dbContext.KeKhaiDangKyGias
                        .Where(x => !string.IsNullOrEmpty(x.SoHsDuyet))
                        .Select(x => x.SoHsDuyet)
                        .ToListAsync();

                    int maxVal = 0;
                    foreach (var item in list)
                    {
                        if (int.TryParse(item, out int val))
                        {
                            if (val > maxVal) maxVal = val;
                        }
                    }
                    soHsDuyet = (maxVal + 1).ToString();
                }

                return Json(new { 
                    success = true, 
                    soQd = model.SoQd, 
                    soHsDuyet = soHsDuyet 
                });
            }
            return Json(new { success = false, message = "Không tìm thấy hồ sơ" });
        }

        [HttpPost("Duyet")]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(Guid hoSoId, string soHsDuyet)
        {
            var response = await _keKhaiDangKyGiaXetDuyetService.DuyetAsync(hoSoId, soHsDuyet);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost("HuyDuyet")]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDuyet(Guid hoSoId)
        {
            var response = await _keKhaiDangKyGiaXetDuyetService.HuyDuyetAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost("TraLai")]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraLai(Guid hoSoId, string Lydo)
        {
            var response = await _keKhaiDangKyGiaXetDuyetService.TraLaiAsync(hoSoId, Lydo);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost("CongBo")]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CongBo(Guid hoSoId)
        {
            var response = await _keKhaiDangKyGiaXetDuyetService.CongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost("HuyCongBo")]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyCongBo(Guid hoSoId)
        {
            var response = await _keKhaiDangKyGiaXetDuyetService.HuyCongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }
    }
}
