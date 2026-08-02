using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class BanHanhVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/BanHanhVanBan")]
        [AuthorizeAction("Index", "BanHanhVanBan", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachBanHanhAsync(TimKiem, isSSA ? selectedDonViId : null, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Ban hành văn bản";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.BanHanhVanBan";
            ViewData["RoutePrefix"] = "/Manages/BanHanhVanBan";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Danh sách hồ sơ đang ở bước cập nhật văn bản đã được thông qua và ban hành.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/BanHanhVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "BanHanhVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/BanHanhVanBan";
            ViewData["HideWorkflowAction"] = "true";
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "BanHanhVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/BanHanhVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "BanHanhVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "BanHanhVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/BanHanhVanBan/Edit")]
        [AuthorizeAction("Edit", "BanHanhVanBan", "Index")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetBanHanhFormAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "BanHanhVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Ban hành văn bản";
            return View("Views/Admin/Manages/BanHanhVanBan/Edit.cshtml", model.Data);
        }

        [HttpPost("Manages/BanHanhVanBan/Edit")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "BanHanhVanBan", "Index")]
        public async Task<IActionResult> Save(HoSoVanBanBanHanhFormModel request, string submitAction = "SAVE")
        {
            var model = await _hoSoVanBanWorkflowService.SaveBanHanhAsync(request, submitAction == "CONFIRM");
            if (model.Status == "error")
            {
                ModelState.AddModelError(string.Empty, model.Message ?? "Không thể cập nhật thông tin ban hành.");
                request.CoQuanBanHanhOptions = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
                return View("Views/Admin/Manages/BanHanhVanBan/Edit.cshtml", request);
            }

            TempData["SuccessMessage"] = model.Message;
            return RedirectToAction("Index");
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
