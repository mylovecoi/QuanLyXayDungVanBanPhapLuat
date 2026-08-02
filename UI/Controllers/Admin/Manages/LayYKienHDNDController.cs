using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class LayYKienHDNDController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/LayYKienHDND")]
        [AuthorizeAction("Index", "LayYKienHDND", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(
                TimKiem,
                "BUOC_06_TRINH_HDND_HOP",
                isSSA ? selectedDonViId : null,
                PageSize,
                PageCurrent,
                true,
                null,
                "XayDung");

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Danh sách hồ sơ lấy ý kiến HĐND";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.LayYKienHDND";
            ViewData["RoutePrefix"] = "/Manages/LayYKienHDND";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Danh sách hồ sơ đang ở bước trình HĐND họp để cập nhật ý kiến HĐND.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/LayYKienHDND/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "LayYKienHDND", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/LayYKienHDND";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "LayYKienHDND";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/LayYKienHDND/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "LayYKienHDND", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "LayYKienHDND";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/LayYKienHDND/KetQua")]
        [AuthorizeAction("Edit", "LayYKienHDND", "Index")]
        public async Task<IActionResult> KetQua(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetKetQuaLayYKienFormAsync(id, "HDND");
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "LayYKienHDND";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Kết quả lấy ý kiến HĐND";
            return View("Views/Admin/Manages/LayYKienHDND/KetQua.cshtml", model.Data);
        }

        [HttpPost("Manages/LayYKienHDND/KetQua")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "LayYKienHDND", "Index")]
        public async Task<IActionResult> KetQuaSave(HoSoVanBanKetQuaLayYKienFormModel request, string submitAction = "SAVE")
        {
            request.CoQuanLayYKien = "HDND";
            request.TrangThai = submitAction == "CONFIRM" ? "DA_XAC_NHAN" : "NHAP";
            var model = await _hoSoVanBanWorkflowService.SaveKetQuaLayYKienAsync(request);
            if (model.Status == "error")
            {
                ModelState.AddModelError(string.Empty, model.Message ?? "Không thể lưu kết quả.");
                ViewData["Title"] = "Kết quả lấy ý kiến HĐND";
                return View("Views/Admin/Manages/LayYKienHDND/KetQua.cshtml", request);
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
