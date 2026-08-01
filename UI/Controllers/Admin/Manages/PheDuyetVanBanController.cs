using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class PheDuyetVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/PheDuyetVanBan")]
        [AuthorizeAction("Index", "PheDuyetVanBan", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(TimKiem, "BUOC_06_TRINH_THAM_QUYEN", isSSA ? selectedDonViId : null, PageSize, PageCurrent, true);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Phê duyệt văn bản";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.PheDuyetVanBan";
            ViewData["RoutePrefix"] = "/Manages/PheDuyetVanBan";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem các hồ sơ đã gửi đến đơn vị của bạn, kể cả những hồ sơ đơn vị bạn đã xử lý và chuyển tiếp.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "PheDuyetVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/PheDuyetVanBan";
            ViewData["WorkflowActionTitle"] = "Phê duyệt văn bản trình cơ quan có thẩm quyền";
            ViewData["WorkflowActionButton"] = "Gửi kết quả đồng ý/trả lại";
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpGet("Manages/PheDuyetVanBan/WorkflowPage")]
        [AuthorizeAction("Edit", "PheDuyetVanBan", "Index")]
        public async Task<IActionResult> WorkflowPage(Guid id)
        {
            ViewData["Title"] = "Phê duyệt văn bản";
            ViewData["PageTitle"] = "Phê duyệt văn bản";
            ViewData["PageSubtitle"] = "Cập nhật kết quả phê duyệt trên màn hình nghiệp vụ riêng.";
            ViewData["RoutePrefix"] = "/Manages/PheDuyetVanBan";
            ViewData["WorkflowActionTitle"] = "Phê duyệt văn bản";
            ViewData["ForceWorkflowAction"] = "true";
            ViewData["WorkflowPageMode"] = "true";
            ViewData["CompactWorkflowPage"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return View("Views/Admin/Manages/PheDuyetVanBan/WorkflowPage.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "PheDuyetVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/PheDuyetVanBan/ChuyenBanHanhModel")]
        [AuthorizeAction("Edit", "PheDuyetVanBan", "Index")]
        public async Task<JsonResult> ChuyenBanHanhModel(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChuyenBanHanhModelAsync(id);
            return Json(new { status = model.Status, message = model.Message, data = model.Data });
        }

        [HttpPost("Manages/PheDuyetVanBan/NhanHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "PheDuyetVanBan", "Index")]
        public async Task<JsonResult> NhanHoSo(Guid id, string actionType = "NHAN_HO_SO", DateTime? ngayXuLy = null, DateTime? hanXuLy = null)
        {
            var model = await _hoSoVanBanWorkflowService.NhanHoSoAsync(id, actionType, null, null, ngayXuLy, hanXuLy);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/PheDuyetVanBan/HoanThanhXuLy")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "PheDuyetVanBan", "Index")]
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
