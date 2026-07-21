using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.DTOs.Manages.ThongTinHoSo.ExportData;
using Services.Manages.ThongTinHoSo;
using Services.Settings;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages.QuanLyHoSo
{
    [Route("Manages/QuanLyHoSo/DanhSachHoSoCCCT")]
    [SetViewDataFilter]
    public class DanhSachHoSoCCCTController(
        IAuthService authService,
        IHoSoCCCTService hoSoCCCTService,
        IDmHopDongService dmHopDongService,
        IOptionDataService optionDataService,
        IHoSoCCCTDynamicService hoSoCCCTDynamicService,
        IDanhMucDonViService danhMucDonViService) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IHoSoCCCTService _hoSoCCCTService = hoSoCCCTService;
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private readonly IOptionDataService _optionDataService = optionDataService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IHoSoCCCTDynamicService _hoSoCCCTDynamicService = hoSoCCCTDynamicService;
        private string ViewPath(string viewName) => $"../Admin/Manages/QuanLyHoSo/DanhSachHoSoCCCT/{viewName}";

        private async Task InitDataForCreateOrUpdate(HoSoCCCTDto entity)
        {
            ViewData["TinhNangThanhToan"] = await _danhMucDonViService.GetTinhNangThanhToanStatusAsync();
        }

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new HoSoFilter(Request, loaiNghiepVu: true, _authService);
            var response = await _hoSoCCCTService.GetListByFilterAsync(filter);
            ViewData["DanhMucHopDong"] = (await _dmHopDongService.GetListByFilterAsync(new(1000))).Data ?? new List<DanhMucHopDong>();
            ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["Filter"] = filter;
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpGet(nameof(Show))]
        [AuthorizeAction(nameof(Show))]
        public async Task<IActionResult> Show(Guid hoSoId)
        {
            var response = await _hoSoCCCTDynamicService.GetSingleByIdAsync(hoSoId);
            return View(ViewPath(nameof(Show)), response.Data);
        }

        [HttpPost(nameof(Create))]
        [AuthorizeAction(nameof(Create))]
        public async Task<IActionResult> Create(Guid dmHopDongId, Guid donViId)
        {
            var response = await _hoSoCCCTDynamicService.InitDataForCreate(dmHopDongId, donViId);
            ViewData["Messages"] = response.Message;
            if (response.Status == "success")
                await InitDataForCreateOrUpdate(response.Data);
            return View(ViewPath("CreateOrEdit"), response.Data);
        }

        [HttpPost(nameof(Store))]
        [AuthorizeAction(nameof(Store))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(HoSoCCCTDto request)
        {
            var httpRequest = Request;
            var validatResult = await _hoSoCCCTDynamicService.ValidateRequestAsync(request);
            string status = validatResult.Status, message = validatResult.Message;
            if (validatResult.Status == "success")
            {
                var response = await _hoSoCCCTDynamicService.StoreAsync(request);
                status = response.Status;
                message = response.Message;
            }
            else
            {
                validatResult.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
            }
            if (status == "error")
                await InitDataForCreateOrUpdate(request);
            return RenderValidationResult(status == "success", message, request, ViewPath("_FormFields"));
        }

        [HttpGet(nameof(Edit))]
        [AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> Edit(Guid hoSoId)
        {
            var response = await _hoSoCCCTDynamicService.GetSingleByIdAsync(hoSoId);
            ViewData["Messages"] = response.Message;
            if (response.Status == "success")
                await InitDataForCreateOrUpdate(response.Data);
            return View(ViewPath("CreateOrEdit"), response.Data);
        }

        [HttpPost(nameof(Update))]
        [AuthorizeAction(nameof(Update))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(HoSoCCCTDto request)
        {
            var validatResult = await _hoSoCCCTDynamicService.ValidateRequestAsync(request);
            string status = validatResult.Status, message = validatResult.Message;
            if (validatResult.Status == "success")
            {
                var response = await _hoSoCCCTDynamicService.UpdateAsync(request);
                status = response.Status;
                message = response.Message;
            }
            else
            {
                validatResult.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
            }
            if (status == "error")
                await InitDataForCreateOrUpdate(request);
            return RenderValidationResult(status == "success", message, request, ViewPath("_FormFields"));
        }

        [HttpPost(nameof(Chuyen))]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chuyen(Guid hoSoId)
        {
            var response = await _hoSoCCCTService.ChuyenAsync(hoSoId);
            return RenderValidationResult(response.Status == "success", response.Message, new());
        }

        [HttpPost(nameof(Delete))]
        [AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid hoSoId)
        {
            var response = await _hoSoCCCTDynamicService.DeleteAsync(hoSoId);
            return RenderValidationResult(response.Status == "success", response.Message, new());
        }

        #region phí lệ phí
        [HttpPost("ThongTinLePhi/Edit")]
        [AuthorizeAction("Edit", "DanhSachHoSoCCCT", "Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLePhi(Guid idLePhi)
        {
            var response = await _hoSoCCCTService.EditLePhiAsync(idLePhi);

            var html = await StaticViewRenderHelper.RenderPartialViewToStringAsync(this, "Views/Admin/Manages/QuanLyHoSo/DanhSachHoSoCCCT/ChiTietPhiLePhi/_FormFields.cshtml", response.Data);

            return Json(new { status = response.Status, message = response.Message, html });
        }

        [HttpPost("ThongTinLePhi/Update")]
        [AuthorizeAction("Update", "DanhSachHoSoCCCT", "Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLePhi(HoSoCCCTChiPhi request, double giaTriHopDong)
        {
            var response = await _hoSoCCCTService.UpdateLePhiAsync(request, giaTriHopDong);
            return Json(new { status = response.Status, message = response.Message });
        }

        [HttpPost("ThongTinLePhi/DanhSach")]
        [AuthorizeAction("Edit", "DanhSachHoSoCCCT", "Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhSachLePhi(Guid idHoSo)
        {
            var listLePhi = await _hoSoCCCTService.GetListLePhiByHoSoIdAsync(idHoSo);

            return PartialView("Views/Admin/Manages/QuanLyHoSo/DanhSachHoSoCCCT/ChiTietPhiLePhi/_DataTable.cshtml", listLePhi);
        }
        #endregion
    }
}
