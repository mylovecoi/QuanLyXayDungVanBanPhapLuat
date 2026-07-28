using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class DangKyVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/DangKyVanBan")]
        [AuthorizeAction("Index", "DangKyVanBan", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachDangKyAsync(TimKiem, selectedDonViId, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Đăng ký văn bản";
            ViewData["Role"] = "VanBanQPPL.DangKyXayDung.DanhSachDangKy";
            ViewData["RoutePrefix"] = "/Manages/DangKyVanBan";
            ViewData["WorkflowStepUrl"] = "/Manages/DangKyVanBan/LoadWorkflowSteps";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/DangKyVanBan/Create")]
        [AuthorizeAction("Create", "DangKyVanBan", "Index")]
        public async Task<IActionResult> Create()
        {
            ViewData["DanhMucVanBans"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["QuyTrinhSoanThaos"] = await _hoSoVanBanWorkflowService.GetQuyTrinhOptionsAsync();
            ViewData["WorkflowStepUrl"] = "/Manages/DangKyVanBan/LoadWorkflowSteps";
            await PopulateCreateViewDataAsync();

            var model = new HoSoVanBanCreateModel
            {
                Id = Guid.NewGuid(),
                DonViDeNghiId = ResolveCreateDonViDeNghiId(),
                HanXuLy = DateTime.Today.AddDays(7),
                TuNgaySoanThao = DateTime.Today,
                DenNgaySoanThao = DateTime.Today.AddDays(7)
            };
            return PartialView("Views/Admin/Manages/HoSoVanBan/_FormFields.cshtml", model);
        }

        [HttpGet("Manages/DangKyVanBan/CreatePage")]
        [AuthorizeAction("Create", "DangKyVanBan", "Index")]
        public async Task<IActionResult> CreatePage()
        {
            ViewData["Title"] = "Đăng ký văn bản";
            ViewData["PageTitle"] = "Đăng ký văn bản";
            ViewData["PageSubtitle"] = "Thêm mới hồ sơ đăng ký văn bản trên màn hình nghiệp vụ riêng.";
            ViewData["FormAction"] = "/Manages/DangKyVanBan/Store";
            ViewData["SubmitLabel"] = "Thêm mới";
            ViewData["DanhMucVanBans"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["QuyTrinhSoanThaos"] = await _hoSoVanBanWorkflowService.GetQuyTrinhOptionsAsync();
            ViewData["WorkflowStepUrl"] = "/Manages/DangKyVanBan/LoadWorkflowSteps";
            ViewData["AttachedFileTableName"] = "HoSoVanBan";
            ViewData["AttachedFilePageInfo"] = FuntionGlobal.GetPageInfo(0, string.Empty, 5, 1);
            await PopulateCreateViewDataAsync();

            var model = new HoSoVanBanCreateModel
            {
                Id = Guid.NewGuid(),
                DonViDeNghiId = ResolveCreateDonViDeNghiId(),
                HanXuLy = DateTime.Today.AddDays(7),
                TuNgaySoanThao = DateTime.Today,
                DenNgaySoanThao = DateTime.Today.AddDays(7)
            };

            return View("Views/Admin/Manages/HoSoVanBan/Create.cshtml", model);
        }

        [HttpGet("Manages/DangKyVanBan/EditPage")]
        [AuthorizeAction("Edit", "DangKyVanBan", "Index")]
        public async Task<IActionResult> EditPage(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetHoSoEditModelAsync(id);
            if (model.Status == "error" || model.Data is not HoSoVanBanCreateModel editModel)
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Cập nhật đăng ký văn bản";
            ViewData["PageTitle"] = "Cập nhật đăng ký văn bản";
            ViewData["PageSubtitle"] = "";
            ViewData["FormAction"] = "/Manages/DangKyVanBan/Update";
            ViewData["SubmitLabel"] = "Cập nhật";
            ViewData["DanhMucVanBans"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["QuyTrinhSoanThaos"] = await _hoSoVanBanWorkflowService.GetQuyTrinhOptionsAsync();
            ViewData["WorkflowStepUrl"] = "/Manages/DangKyVanBan/LoadWorkflowSteps";
            ViewData["AttachedFileTableName"] = "HoSoVanBan";
            ViewData["AttachedFilePageInfo"] = FuntionGlobal.GetPageInfo(0, string.Empty, 5, 1);
            await PopulateCreateViewDataAsync();

            return View("Views/Admin/Manages/HoSoVanBan/Create.cshtml", editModel);
        }

        [HttpGet("Manages/DangKyVanBan/ChuyenHoSo")]
        [AuthorizeAction("Edit", "DangKyVanBan", "Index")]
        public async Task<IActionResult> ChuyenHoSo(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChuyenHoSoModelAsync(id);
            if (model.Status == "error" || model.Data is not HoSoVanBanXuLyStepModel chuyenHoSoModel)
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DonViOptions"] = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            return PartialView("Views/Admin/Manages/HoSoVanBan/_ChuyenHoSo.cshtml", chuyenHoSoModel);
        }

        [HttpGet("Manages/DangKyVanBan/LoadWorkflowSteps")]
        [AuthorizeAction("Create", "DangKyVanBan", "Index")]
        public async Task<JsonResult> LoadWorkflowSteps(Guid quyTrinhSoanThaoId)
        {
            var data = await _hoSoVanBanWorkflowService.GetBuocThoiHanOptionsAsync(quyTrinhSoanThaoId);
            return Json(data);
        }

        [HttpPost("Manages/DangKyVanBan/Store")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Create", "DangKyVanBan", "Index")]
        public async Task<IActionResult> Store(HoSoVanBanCreateModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (!(currentUser?.SSA ?? false))
            {
                request.DonViDeNghiId = currentUser?.DanhMucDonViId;
            }

            var model = await _hoSoVanBanWorkflowService.CreateHoSoAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "DangKyVanBan");
        }

        [HttpPost("Manages/DangKyVanBan/Update")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DangKyVanBan", "Index")]
        public async Task<IActionResult> Update(HoSoVanBanCreateModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (!(currentUser?.SSA ?? false))
            {
                request.DonViDeNghiId = currentUser?.DanhMucDonViId;
            }

            var model = await _hoSoVanBanWorkflowService.UpdateHoSoAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "DangKyVanBan");
        }

        [HttpPost("Manages/DangKyVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "DangKyVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/DangKyVanBan";
            ViewData["HideWorkflowAction"] = "true";
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/DangKyVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "DangKyVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpPost("Manages/DangKyVanBan/NhanHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DangKyVanBan", "Index")]
        public async Task<JsonResult> NhanHoSo(Guid id, string actionType = "NHAN_HO_SO")
        {
            var model = await _hoSoVanBanWorkflowService.NhanHoSoAsync(id, actionType);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/DangKyVanBan/HoanThanhXuLy")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DangKyVanBan", "Index")]
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

        private Guid? ResolveCreateDonViDeNghiId()
        {
            var currentUser = _authService.GetUserInfo();
            return currentUser?.DanhMucDonViId != Guid.Empty ? currentUser?.DanhMucDonViId : null;
        }
    }
}
