using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.Settings;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.DinhGiaHHDV.DinhGiaKhac;
using Services.DTOs.DinhGiaHHDV.ThongTinHoSo;
using Services.Settings;
using Services.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using UI.ViewModels;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace UI.Controllers.Admin.DinhGiaHHDV.DinhGiaKhac
{
    [SetViewDataFilter]
    public class DinhGiaXetDuyetController(
        IAuthService authService,
        IDinhGiaService dinhGiaService,
        IDinhGiaXetDuyetService dinhGiaXetDuyetService,
        IDanhMucDonViService danhMucDonViService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IDinhGiaService _dinhGiaService = dinhGiaService;
        private readonly IDinhGiaXetDuyetService _dinhGiaXetDuyetService = dinhGiaXetDuyetService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private string ViewPath(string viewName) => $"../Admin/DinhGiaHHDV/DinhGiaKhac/XetDuyet/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index(string MaNghe)
        {
            var filter = new DinhGiaFilter(Request, _authService);
            var response = await _dinhGiaXetDuyetService.GetListXetDuyetByFilterAsync(filter, MaNghe);

            var listKinhDoanhNganh = await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level == 0 || t.PhanLoai == "Group") && t.LoaiGia == "DG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();
            var listKinhDoanhNghe = await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "DG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();

            ViewData["DanhMucKinhDoanhNganh"] = listKinhDoanhNganh;
            ViewData["DanhMucKinhDoanhNghe"] = listKinhDoanhNghe;
            ViewData["Filter"] = filter;
            ViewData["MaNghe"] = MaNghe;
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(Guid hoSoId)
        {
            var response = await _dinhGiaXetDuyetService.DuyetAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDuyet(Guid hoSoId)
        {
            var response = await _dinhGiaXetDuyetService.HuyDuyetAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraLai(Guid hoSoId, string Lydo)
        {
            var response = await _dinhGiaXetDuyetService.TraLaiAsync(hoSoId, Lydo);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CongBo(Guid hoSoId)
        {
            var response = await _dinhGiaXetDuyetService.CongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }

        [HttpPost]
        [AuthorizeAction("Public")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyCongBo(Guid hoSoId)
        {
            var response = await _dinhGiaXetDuyetService.HuyCongBoAsync(hoSoId);
            return Json(new { isValid = response.Status == "success", message = response.Message });
        }
    }
}
