using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class PheDuyetDuThaoController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/PheDuyetDuThao")]
        [AuthorizeAction("Index", "PheDuyetDuThao", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(
                TimKiem,
                "BUOC_04_LAY_Y_KIEN",
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

            ViewData["Title"] = "Phê duyệt dự thảo";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.DuThaoVanBan.PheDuyetDuThao";
            ViewData["RoutePrefix"] = "/Manages/PheDuyetDuThao";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem các hồ sơ dự thảo chờ phê duyệt sau khi đơn vị xét duyệt đã tiếp nhận xử lý.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetDuThao/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "PheDuyetDuThao", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/PheDuyetDuThao";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetDuThao";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/PheDuyetDuThao/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "PheDuyetDuThao", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "PheDuyetDuThao";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
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
