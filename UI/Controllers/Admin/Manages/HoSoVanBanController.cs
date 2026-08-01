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

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(
                TimKiem,
                "SOAN_THAO",
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

            ViewData["Title"] = "Soạn thảo văn bản";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.SoanThaoVanBan";
            ViewData["RoutePrefix"] = "/Manages/HoSoVanBan";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem các hồ sơ đã được chuyển từ bước 2 sang bước soạn thảo theo đơn vị tiếp nhận.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/HoSoVanBan/CreatePage")]
        [AuthorizeAction("Create", "HoSoVanBan", "Index")]
        public async Task<IActionResult> CreatePage()
        {
            ViewData["Title"] = "Soạn thảo văn bản";
            ViewData["PageTitle"] = "Thêm mới hồ sơ soạn thảo";
            ViewData["PageSubtitle"] = "Khởi tạo hồ sơ soạn thảo và có thể liên kết tới văn bản đã đăng ký.";
            ViewData["FormAction"] = "/Manages/HoSoVanBan/Store";
            ViewData["SubmitLabel"] = "Thêm mới";
            ViewData["BackUrl"] = "/Manages/HoSoVanBan";
            ViewData["DonViLabel"] = "Đơn vị soạn thảo";
            ViewData["DanhMucVanBans"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["QuyTrinhSoanThaos"] = await _hoSoVanBanWorkflowService.GetQuyTrinhOptionsAsync(loaiQuyTrinh: "XayDung");
            ViewData["WorkflowStepUrl"] = "/Manages/HoSoVanBan/LoadWorkflowSteps";
            ViewData["AttachedFileTableName"] = "HoSoVanBan";
            await PopulateCreateViewDataAsync();

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var sessionDonViId = currentUser?.DanhMucDonViId != Guid.Empty ? currentUser?.DanhMucDonViId : null;
            ViewData["HoSoDangKyOptions"] = await _hoSoVanBanWorkflowService.GetHoSoDangKyOptionsAsync(sessionDonViId, isSSA);

            var model = new HoSoVanBanCreateModel
            {
                Id = Guid.NewGuid(),
                DonViDeNghiId = sessionDonViId,
                HanXuLy = DateTime.Today.AddDays(7),
                TuNgaySoanThao = DateTime.Today,
                DenNgaySoanThao = DateTime.Today.AddDays(7)
            };
            model.QuyTrinhSoanThaoId = (ViewData["QuyTrinhSoanThaos"] as List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>)?.FirstOrDefault()?.Id ?? Guid.Empty;

            return View("Views/Admin/Manages/HoSoVanBan/Create.cshtml", model);
        }

        [HttpGet("Manages/HoSoVanBan/Create")]
        [AuthorizeAction("Create", "HoSoVanBan", "Index")]
        public async Task<IActionResult> Create()
        {
            ViewData["DanhMucVanBans"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["QuyTrinhSoanThaos"] = await _hoSoVanBanWorkflowService.GetQuyTrinhOptionsAsync(loaiQuyTrinh: "XayDung");

            var model = new HoSoVanBanCreateModel
            {
                HanXuLy = DateTime.Today.AddDays(7)
            };
            return PartialView("Views/Admin/Manages/HoSoVanBan/_FormFields.cshtml", model);
        }

        [HttpGet("Manages/HoSoVanBan/LoadWorkflowSteps")]
        [AuthorizeAction("Create", "HoSoVanBan", "Index")]
        public async Task<JsonResult> LoadWorkflowSteps(Guid quyTrinhSoanThaoId)
        {
            var data = await _hoSoVanBanWorkflowService.GetBuocThoiHanOptionsAsync(quyTrinhSoanThaoId);
            return Json(data);
        }

        [HttpPost("Manages/HoSoVanBan/Store")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Store", "HoSoVanBan", "Index")]
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

            ViewData["HideWorkflowAction"] = "true";
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

        [HttpPost("Manages/HoSoVanBan/LayGopYAction")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "HoSoVanBan", "Index")]
        public async Task<IActionResult> LayGopYAction(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "HoSoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/LayGopY.cshtml", model.Data);
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
        [AuthorizeAction("Edit", "HoSoVanBan", "Index")]
        public async Task<JsonResult> HoanThanhXuLy(HoSoVanBanXuLyStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhXuLyAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/HoSoVanBan/HoanThanhLayYKien")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "HoSoVanBan", "Index")]
        public async Task<JsonResult> HoanThanhLayYKien(HoSoVanBanLayYKienStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhLayYKienAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/HoSoVanBan/KhoiTaoLayYKien")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "HoSoVanBan", "Index")]
        public async Task<JsonResult> KhoiTaoLayYKien(HoSoVanBanLayYKienStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.KhoiTaoLayYKienAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/HoSoVanBan/HoanThanhDanhGia")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "HoSoVanBan", "Index")]
        public async Task<JsonResult> HoanThanhDanhGia(HoSoVanBanDanhGiaStepModel request)
        {
            var model = await _hoSoVanBanWorkflowService.HoanThanhDanhGiaAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        private async Task PopulateCreateViewDataAsync()
        {
            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var sessionDonViId = currentUser?.DanhMucDonViId ?? Guid.Empty;
            var donViOptions = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            if (!isSSA)
            {
                donViOptions = donViOptions.Where(x => x.Id == sessionDonViId).ToList();
            }

            ViewData["CreateDonViOptions"] = donViOptions;
            ViewData["CreateIsSSA"] = isSSA;
            ViewData["CreateSessionDonViId"] = sessionDonViId;
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
