using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class PheDuyetDangKyController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/PheDuyetDangKy")]
        [AuthorizeAction("Index", "PheDuyetDangKy", "Index")]
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
                isSSA ? selectedDonViId : null,
                PageSize,
                PageCurrent,
                true,
                new[] { "CHUYEN_PHE_DUYET", "NHAN_VA_CHUYEN_PHE_DUYET" },
                "DangKy");

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Phê duyệt đăng ký";
            ViewData["Role"] = "VanBanQPPL.DangKyXayDung.PheDuyetDangKy";
            ViewData["RoutePrefix"] = "/Manages/PheDuyetDangKy";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem các hồ sơ đã gửi đến đơn vị của bạn, kể cả những hồ sơ đơn vị bạn đã xử lý và chuyển tiếp.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetDangKy/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "PheDuyetDangKy", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/PheDuyetDangKy";
            ViewData["HideWorkflowAction"] = "true";
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetDangKy";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetDangKy/WorkflowAction")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "PheDuyetDangKy", "Index")]
        public async Task<IActionResult> WorkflowAction(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/PheDuyetDangKy";
            ViewData["WorkflowActionTitle"] = "Phê duyệt đăng ký xây dựng văn bản";
            ViewData["WorkflowActionButton"] = "Gửi kết quả đồng ý/trả lại";
            ViewData["ApprovalReviewMode"] = "DangKy";
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetDangKy";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetDangKy/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "PheDuyetDangKy", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetDangKy";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetDangKy/NhanHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "PheDuyetDangKy", "Index")]
        public async Task<JsonResult> NhanHoSo(Guid id, string actionType = "NHAN_HO_SO")
        {
            var model = await _hoSoVanBanWorkflowService.NhanHoSoAsync(id, actionType);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/PheDuyetDangKy/HoanThanhXuLy")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "PheDuyetDangKy", "Index")]
        public async Task<JsonResult> HoanThanhXuLy(HoSoVanBanXuLyStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhXuLyAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/PheDuyetDangKy/TraLaiHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "PheDuyetDangKy", "Index")]
        public async Task<JsonResult> TraLaiHoSo(Guid id, string lyDoTraLai, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.TraLaiDangKyAsync(id, lyDoTraLai, ghiChu);
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
