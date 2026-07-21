using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.Manages.ThongTinHoSo;
using Services.Model;
using Services.Settings;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages.QuanLyHoSo
{
    [Route("Manages/QuanLyHoSo/XuLyHoSoCCCT")]
    [SetViewDataFilter]
    public class XuLyHoSoCCCTController(
        IAuthService authService,
        IDanhMucDonViService danhMucDonViService,
        IDanhMucCanBoService danhMucCanBoService,
        IHoSoCCCTXetDuyetService hoSoCCCTXetDuyetService) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IDanhMucCanBoService _danhMucCanBoService = danhMucCanBoService;
        private readonly IHoSoCCCTXetDuyetService _xetDuyetSerrvice = hoSoCCCTXetDuyetService;
        private string ViewPath(string viewName) => $"../Admin/Manages/QuanLyHoSo/XuLyHoSoCCCT/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new HoSoFilter(Request, loaiNghiepVu: true, _authService);
            var response = await _xetDuyetSerrvice.GetListByFilterAsync(filter);
            var user = _authService.GetUserInfo();
            ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["TinhNangThanhToan"] = await _danhMucDonViService.GetTinhNangThanhToanStatusAsync();
            ViewData["DanhMucCanBo"] = (await _danhMucCanBoService.GetDanhMucCanBoAsync("", 100, 1, user!.DanhMucDonViId, null, "")).Data;
            ViewData["DonViId"] = filter.DonViId;
            ViewData["Status"] = filter.Status;
            ViewData["TargetYear"] = filter.TargetYear;
            ViewData["Filter"] = filter;
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpPost(nameof(TraLai))]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraLai(Guid hoSoId, string lyDoTraLai)
        {
            return await HandleAjaxOperationAsync(() => _xetDuyetSerrvice.TraLaiAsync(hoSoId, lyDoTraLai));
        }

        [HttpPost(nameof(TiepNhan))]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TiepNhan(Guid hoSoId, Guid congChungVienId)
        {
            return await HandleAjaxOperationAsync(() => _xetDuyetSerrvice.ChangeStatusAsync(hoSoId, "CTT", congChungVienId));
        }

        [HttpPost(nameof(XacNhanThanhToan))]
        [AuthorizeAction("Approve", "XuLyHoSoCCCT", "Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(Guid hoSoId, DateTime ngayThanhToan)
        {
            return await HandleAjaxOperationAsync(() => _xetDuyetSerrvice.XacNhanThanhToanAsync(hoSoId, ngayThanhToan));
        }

        [HttpPost(nameof(XacNhan))]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(Guid hoSoId)
        {
            return await HandleAjaxOperationAsync(() => _xetDuyetSerrvice.ChangeStatusAsync(hoSoId, "XL"));
        }

        [HttpPost(nameof(HoanThanh))]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoanThanh(Guid hoSoId, string soQdPheDuyet, DateTime ngayQdPheDuyet, IFormFile fileChuKyDienTy)
        {
            return await HandleAjaxOperationAsync(() => _xetDuyetSerrvice.HoanThanhAsync(hoSoId, soQdPheDuyet, ngayQdPheDuyet, fileChuKyDienTy));
        }

        [HttpPost(nameof(HuyDuyet))]
        [AuthorizeAction("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDuyet(Guid hoSoId)
        {
            return await HandleAjaxOperationAsync(() => _xetDuyetSerrvice.HoanThanhAsync(hoSoId, "", DateTime.MinValue, null!, isHoanThanh: false));
        }

        [HttpPost("GetChiPhiHoSo")]
        [AuthorizeAction("Approve", "XuLyHoSoCCCT", "Approve")]
        public async Task<IActionResult> GetChiPhiHoSo(Guid hoSoId)
        {
            return await HandleAjaxOperationAsync(() => _xetDuyetSerrvice.GetChiPhiHoSoAsync(hoSoId));
        }

        private async Task<IActionResult> HandleAjaxOperationAsync(Func<Task<CommonResponse>> operation)
        {
            var response = await operation();
            return Json(new
            {
                isValid = response.Status == "success",
                message = response.Message,
                status = response.Data,
            });
        }
    }
}
