using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class LayYKienVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/LayYKienVanBan")]
        [AuthorizeAction("Index", "LayYKienVanBan", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachLayYKienAsync(
                TimKiem,
                selectedDonViId,
                PageSize,
                PageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Lấy góp ý";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.GopYDanhGia";
            ViewData["RoutePrefix"] = "/Manages/LayYKienVanBan";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            ViewData["ReceiveModeMessage"] = "Đơn vị soạn thảo theo dõi và tổng hợp góp ý tại đây. Các đơn vị được xin ý kiến cũng sẽ thấy hồ sơ để phản hồi ý kiến của đơn vị mình.";
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/LayYKienVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "LayYKienVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/LayYKienVanBan";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "LayYKienVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/LayYKienVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "LayYKienVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "LayYKienVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/LayYKienVanBan/PhanHoi")]
        [AuthorizeAction("Edit", "LayYKienVanBan", "Index")]
        public async Task<IActionResult> PhanHoi(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetLayYKienFormAsync(id, "PHAN_HOI_DON_VI");
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "LayYKienVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageTitle"] = "Phản hồi góp ý";
            ViewData["PageSubtitle"] = "Đơn vị được xin ý kiến cập nhật kết quả góp ý và file phản hồi.";
            return View("Views/Admin/Manages/LayYKienVanBan/PhanHoi.cshtml", model.Data);
        }

        [HttpPost("Manages/LayYKienVanBan/PhanHoi")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "LayYKienVanBan", "Index")]
        public async Task<IActionResult> PhanHoiSave(HoSoVanBanLayYKienStepModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (!(currentUser?.SSA ?? false))
            {
                request.DonViDuocLayYKienId = currentUser?.DanhMucDonViId;
            }

            request.ActionMode = "PHAN_HOI_DON_VI";
            var model = await _hoSoVanBanWorkflowService.HoanThanhLayYKienAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return await PhanHoi(request.HoSoVanBanId);
            }

            TempData["SuccessMessage"] = "Đã cập nhật phản hồi góp ý.";
            return RedirectToAction("Index", "LayYKienVanBan");
        }

        [HttpGet("Manages/LayYKienVanBan/TongHop")]
        [AuthorizeAction("Edit", "LayYKienVanBan", "Index")]
        public async Task<IActionResult> TongHop(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetLayYKienFormAsync(id, "TONG_HOP_Y_KIEN");
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "LayYKienVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageTitle"] = "Tổng hợp góp ý";
            ViewData["PageSubtitle"] = "Đơn vị soạn thảo tổng hợp ý kiến và hoàn thành bước lấy góp ý.";
            return View("Views/Admin/Manages/LayYKienVanBan/TongHop.cshtml", model.Data);
        }

        [HttpPost("Manages/LayYKienVanBan/TongHop")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "LayYKienVanBan", "Index")]
        public async Task<IActionResult> TongHopSave(HoSoVanBanLayYKienStepModel request)
        {
            request.ActionMode = "TONG_HOP_Y_KIEN";
            request.TrangThaiPhanHoi = "DA_GAN_KET_QUA_Y_KIEN";
            var model = await _hoSoVanBanWorkflowService.HoanThanhLayYKienAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return await TongHop(request.HoSoVanBanId);
            }

            TempData["SuccessMessage"] = "Đã tổng hợp góp ý và chuyển hồ sơ sang Dự thảo văn bản.";
            return RedirectToAction("Index", "DuThaoVanBan");
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
            ViewData["HideDonViFilter"] = !isSSA;
            return selectedDonViId;
        }
    }
}

