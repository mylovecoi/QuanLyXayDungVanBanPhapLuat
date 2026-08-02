using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class GiaHanXayDungController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private static readonly Guid SoTuPhapDonViId = Guid.Parse("40000000-0000-0000-0000-000000000002");
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/GiaHanXayDung")]
        [AuthorizeAction("Index", "GiaHanXayDung", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var canViewAll = (currentUser?.SSA ?? false) || currentUser?.DanhMucDonViId == SoTuPhapDonViId;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId, canViewAll);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachGiaHanXayDungAsync(TimKiem, canViewAll ? selectedDonViId : currentUser?.DanhMucDonViId, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Gia hạn thời gian xây dựng";
            ViewData["Role"] = "VanBanQPPL.GiaHanThoiGianXayDung.DanhSachVanBan";
            ViewData["HideDonViFilter"] = !canViewAll;
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/GiaHanXayDung/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/GiaHanXayDung/Edit")]
        [AuthorizeAction("Edit", "GiaHanXayDung", "Index")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetGiaHanXayDungFormAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GiaHanXayDung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Gia hạn thời gian xây dựng";
            return View("Views/Admin/Manages/GiaHanXayDung/Edit.cshtml", model.Data);
        }

        [HttpPost("Manages/GiaHanXayDung/Edit")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "GiaHanXayDung", "Index")]
        public async Task<IActionResult> Save(HoSoVanBanGiaHanFormModel request)
        {
            var model = await _hoSoVanBanWorkflowService.SaveGiaHanXayDungAsync(request);
            if (model.Status == "error")
            {
                ModelState.AddModelError(string.Empty, model.Message ?? "Không thể gia hạn thời gian xây dựng.");
                ViewData["Title"] = "Gia hạn thời gian xây dựng";
                var reload = await _hoSoVanBanWorkflowService.GetGiaHanXayDungFormAsync(request.HoSoVanBanId);
                if (reload.Data is HoSoVanBanGiaHanFormModel form)
                {
                    request.LichSus = form.LichSus;
                    request.TenHoSo = form.TenHoSo;
                    request.MaHoSo = form.MaHoSo;
                    request.TenLoaiVanBan = form.TenLoaiVanBan;
                    request.TenBuocHienTai = form.TenBuocHienTai;
                    request.TenDonViSoanThao = form.TenDonViSoanThao;
                    request.TenDonViXuLyHienTai = form.TenDonViXuLyHienTai;
                }
                return View("Views/Admin/Manages/GiaHanXayDung/Edit.cshtml", request);
            }

            TempData["SuccessMessage"] = model.Message;
            return RedirectToAction("Index");
        }

        private async Task<Guid?> ApplyDonViFilterViewDataAsync(Guid? donViId, bool canViewAll)
        {
            var currentUser = _authService.GetUserInfo();
            var sessionDonViId = currentUser?.DanhMucDonViId ?? Guid.Empty;
            var selectedDonViId = canViewAll
                ? (donViId.HasValue && donViId.Value != Guid.Empty ? donViId : null)
                : (sessionDonViId != Guid.Empty ? sessionDonViId : null);

            var donViOptions = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            if (!canViewAll)
            {
                donViOptions = donViOptions.Where(x => x.Id == sessionDonViId).ToList();
            }

            ViewData["DonViOptions"] = donViOptions;
            ViewData["SelectedDonViId"] = selectedDonViId;
            ViewData["SelectedDonViName"] = selectedDonViId.HasValue
                ? donViOptions.FirstOrDefault(x => x.Id == selectedDonViId.Value)?.TenDonVi
                : null;
            ViewData["IsSSA"] = canViewAll;
            return selectedDonViId;
        }
    }
}
