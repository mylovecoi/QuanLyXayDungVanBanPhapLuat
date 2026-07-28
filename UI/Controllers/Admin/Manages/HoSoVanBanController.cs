using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class HoSoVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/HoSoVanBan")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachAsync(TimKiem, selectedDonViId, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Hồ sơ văn bản";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.DanhSachVanBan";
            ViewData["RoutePrefix"] = "/Manages/HoSoVanBan";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/HoSoVanBan/Create")]
        [AuthorizeAction("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["DanhMucVanBans"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["QuyTrinhSoanThaos"] = await _hoSoVanBanWorkflowService.GetQuyTrinhOptionsAsync();

            var model = new HoSoVanBanCreateModel
            {
                HanXuLy = DateTime.Today.AddDays(7)
            };
            return PartialView("Views/Admin/Manages/HoSoVanBan/_FormFields.cshtml", model);
        }

        [HttpGet("Manages/HoSoVanBan/LoadWorkflowSteps")]
        [AuthorizeAction("Create")]
        public async Task<JsonResult> LoadWorkflowSteps(Guid quyTrinhSoanThaoId)
        {
            var data = await _hoSoVanBanWorkflowService.GetBuocThoiHanOptionsAsync(quyTrinhSoanThaoId);
            return Json(data);
        }

        [HttpPost("Manages/HoSoVanBan/Store")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Store")]
        public async Task<IActionResult> Store(HoSoVanBanCreateModel request)
        {
            var model = await _hoSoVanBanWorkflowService.CreateHoSoAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "HoSoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "HoSoVanBan");
        }

        [HttpPost("Manages/HoSoVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Show")]
        public async Task<IActionResult> Show(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "HoSoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/HoSoVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "HoSoVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "HoSoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpPost("Manages/HoSoVanBan/NhanHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "HoSoVanBan", "Index")]
        public async Task<JsonResult> NhanHoSo(Guid id, string actionType = "NHAN_HO_SO")
        {
            var model = await _hoSoVanBanWorkflowService.NhanHoSoAsync(id, actionType);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/HoSoVanBan/HoanThanhXuLy")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit")]
        public async Task<JsonResult> HoanThanhXuLy(HoSoVanBanXuLyStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhXuLyAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/HoSoVanBan/HoanThanhLayYKien")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit")]
        public async Task<JsonResult> HoanThanhLayYKien(HoSoVanBanLayYKienStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhLayYKienAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/HoSoVanBan/HoanThanhDanhGia")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit")]
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
