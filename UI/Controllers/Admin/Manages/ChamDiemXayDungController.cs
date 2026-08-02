using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class ChamDiemXayDungController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/ChamDiemXayDung")]
        [AuthorizeAction("Index", "ChamDiemXayDung", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachChamDiemXayDungAsync(TimKiem, isSSA ? selectedDonViId : null, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Chấm điểm xây dựng";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.ChamDiemXayDung";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/ChamDiemXayDung/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/ChamDiemXayDung/Edit")]
        [AuthorizeAction("Edit", "ChamDiemXayDung", "Index")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChamDiemXayDungFormAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ChamDiemXayDung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Chấm điểm xây dựng";
            return View("Views/Admin/Manages/ChamDiemXayDung/Edit.cshtml", model.Data);
        }

        [HttpPost("Manages/ChamDiemXayDung/Edit")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "ChamDiemXayDung", "Index")]
        public async Task<IActionResult> Save(HoSoVanBanChamDiemFormModel request)
        {
            var model = await _hoSoVanBanWorkflowService.SaveChamDiemXayDungAsync(request);
            if (model.Status == "error")
            {
                ModelState.AddModelError(string.Empty, model.Message ?? "Không thể lưu bản ghi chấm điểm.");
                ViewData["Title"] = "Chấm điểm xây dựng";
                return View("Views/Admin/Manages/ChamDiemXayDung/Edit.cshtml", request);
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
