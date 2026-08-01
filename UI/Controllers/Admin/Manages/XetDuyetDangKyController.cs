using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class XetDuyetDangKyController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/XetDuyetDangKy")]
        [AuthorizeAction("Index", "XetDuyetDangKy", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(
                TimKiem,
                "BUOC_02_THONG_NHAT",
                isSSA ? selectedDonViId : selectedDonViId,
                PageSize,
                PageCurrent,
                false,
                null,
                "DangKy");

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Xét duyệt đăng ký";
            ViewData["Role"] = "VanBanQPPL.DangKyXayDung.XetDuyetDangKy";
            ViewData["RoutePrefix"] = "/Manages/XetDuyetDangKy";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem các hồ sơ bước xét duyệt đăng ký của đơn vị tạo hoặc đơn vị tiếp nhận.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDangKy/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetDangKy", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/XetDuyetDangKy";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetDangKy";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDangKy/WorkflowAction")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetDangKy", "Index")]
        public async Task<IActionResult> WorkflowAction(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/XetDuyetDangKy";
            ViewData["WorkflowActionTitle"] = "Phê duyệt đăng ký xây dựng văn bản";
            ViewData["ApprovalReviewMode"] = "DangKy";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetDangKy";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDangKy/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetDangKy", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetDangKy";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetDangKy/TraLaiHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetDangKy", "Index")]
        public async Task<JsonResult> TraLaiHoSo(Guid id, string lyDoTraLai, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.TraLaiDangKyAsync(id, lyDoTraLai, ghiChu);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/XetDuyetDangKy/HuyXetDuyet")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetDangKy", "Index")]
        public async Task<JsonResult> HuyXetDuyet(Guid id, string lyDoHuy, DateTime? ngayHuy = null, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.HuyXetDuyetDangKyAsync(id, lyDoHuy, ngayHuy, ghiChu);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpGet("Manages/XetDuyetDangKy/TaoHoSoSoanThao")]
        [AuthorizeAction("Edit", "XetDuyetDangKy", "Index")]
        public async Task<IActionResult> TaoHoSoSoanThaoPage(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetTaoHoSoSoanThaoTuDangKyModelAsync(id);
            if (model.Status == "error" || model.Data is not HoSoVanBanTaoSoanThaoTuDangKyModel data)
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetDangKy";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Tạo hồ sơ soạn thảo";
            ViewData["PageTitle"] = "Tạo hồ sơ soạn thảo từ đăng ký";
            ViewData["PageSubtitle"] = "Hệ thống sẽ dùng workflow hồ sơ soạn thảo riêng cho chức năng này.";
            ViewData["WorkflowOptions"] = await _hoSoVanBanWorkflowService.GetDraftQuyTrinhOptionsAsync(data.DanhMucVanBanId);
            ViewData["DonViOptions"] = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            ViewData["IsSSA"] = _authService.GetUserInfo()?.SSA ?? false;
            ViewData["WorkflowStepUrl"] = "/Manages/XetDuyetDangKy/LoadWorkflowSteps";
            return View("Views/Admin/Manages/XetDuyetDangKy/TaoHoSoSoanThao.cshtml", data);
        }

        [HttpGet("Manages/XetDuyetDangKy/LoadWorkflowSteps")]
        [AuthorizeAction("Edit", "XetDuyetDangKy", "Index")]
        public async Task<JsonResult> LoadWorkflowSteps(Guid quyTrinhSoanThaoId)
        {
            var data = await _hoSoVanBanWorkflowService.GetBuocThoiHanOptionsAsync(quyTrinhSoanThaoId);
            return Json(data);
        }

        [HttpPost("Manages/XetDuyetDangKy/TaoHoSoSoanThao")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetDangKy", "Index")]
        public async Task<IActionResult> TaoHoSoSoanThao(HoSoVanBanTaoSoanThaoTuDangKyModel request)
        {
            var model = await _hoSoVanBanWorkflowService.TaoHoSoSoanThaoTuDangKyAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Title"] = "Tạo hồ sơ soạn thảo";
                ViewData["PageTitle"] = "Tạo hồ sơ soạn thảo từ đăng ký";
                ViewData["PageSubtitle"] = "Hệ thống sẽ dùng workflow hồ sơ soạn thảo riêng cho chức năng này.";
                ViewData["WorkflowOptions"] = await _hoSoVanBanWorkflowService.GetDraftQuyTrinhOptionsAsync(request.DanhMucVanBanId);
                ViewData["DonViOptions"] = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
                ViewData["IsSSA"] = _authService.GetUserInfo()?.SSA ?? false;
                ViewData["WorkflowStepUrl"] = "/Manages/XetDuyetDangKy/LoadWorkflowSteps";
                return View("Views/Admin/Manages/XetDuyetDangKy/TaoHoSoSoanThao.cshtml", request);
            }

            TempData["SuccessMessage"] = model.Message;
            return RedirectToAction("Index", "HoSoVanBan");
        }

        [HttpPost("Manages/XetDuyetDangKy/HoanThanhXuLy")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetDangKy", "Index")]
        public async Task<JsonResult> HoanThanhXuLy(HoSoVanBanXuLyStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhXuLyAsync(request);
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
