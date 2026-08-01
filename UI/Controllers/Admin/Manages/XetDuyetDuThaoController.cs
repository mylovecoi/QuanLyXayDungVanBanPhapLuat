using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class XetDuyetDuThaoController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/XetDuyetDuThao")]
        [AuthorizeAction("Index", "XetDuyetDuThao", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(
                TimKiem,
                "BUOC_02_GUI_THAM_DINH",
                isSSA ? selectedDonViId : null,
                PageSize,
                PageCurrent,
                true);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Đánh giá văn bản";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.DuThaoVanBan.XetDuyetDuThao";
            ViewData["RoutePrefix"] = "/Manages/XetDuyetDuThao";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem danh sách hồ sơ chờ nhận ở bước Đánh giá văn bản sau khi đơn vị soạn thảo chuyển dự thảo sang bước tiếp theo.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDuThao/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetDuThao", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/XetDuyetDuThao";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetDuThao";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDuThao/WorkflowAction")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetDuThao", "Index")]
        public async Task<IActionResult> WorkflowAction(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/XetDuyetDuThao";
            ViewData["WorkflowActionTitle"] = "Đánh giá dự thảo văn bản";
            ViewData["ReviewMode"] = "DuThao";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetDuThao";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDuThao/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetDuThao", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetDuThao";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDuThao/NhanHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetDuThao", "Index")]
        public async Task<JsonResult> NhanHoSo(Guid id, string actionType = "NHAN_HO_SO", string? noiDungXuLy = null, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.NhanHoSoAsync(id, actionType, noiDungXuLy, ghiChu);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/XetDuyetDuThao/TraLaiHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetDuThao", "Index")]
        public async Task<JsonResult> TraLaiHoSo(Guid id, string lyDoTraLai, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.TraLaiDanhGiaAsync(id, lyDoTraLai, ghiChu);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/XetDuyetDuThao/HoanThanhDanhGia")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetDuThao", "Index")]
        public async Task<JsonResult> HoanThanhDanhGia(HoSoVanBanDanhGiaStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhDanhGiaAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        private async Task<Guid?> ApplyDonViFilterViewDataAsync(Guid? donViId)
        {
            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var sessionDonViId = currentUser?.DanhMucDonViId ?? Guid.Empty;
            var selectedDonViId = isSSA
                ? (donViId.HasValue && donViId.Value != Guid.Empty ? donViId : null)
                : (sessionDonViId != Guid.Empty ? sessionDonViId : null);

            var donViOptions = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            if (!isSSA)
            {
                donViOptions = donViOptions.Where(x => x.Id == sessionDonViId).ToList();
            }

            var selectedDonViName = selectedDonViId.HasValue && selectedDonViId.Value != Guid.Empty
                ? donViOptions.FirstOrDefault(x => x.Id == selectedDonViId.Value)?.TenDonVi
                : null;

            ViewData["DonViOptions"] = donViOptions;
            ViewData["SelectedDonViId"] = selectedDonViId;
            ViewData["SelectedDonViName"] = selectedDonViName;
            ViewData["IsSSA"] = isSSA;
            return selectedDonViId;
        }
    }
}
