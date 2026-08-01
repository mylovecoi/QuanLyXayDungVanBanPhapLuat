using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class DuThaoVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/DuThaoVanBan")]
        [AuthorizeAction("Index", "DuThaoVanBan", "Index")]
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

            ViewData["Title"] = "Dự thảo văn bản";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.DuThaoVanBan";
            ViewData["RoutePrefix"] = "/Manages/DuThaoVanBan";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem các hồ sơ dự thảo, gồm hồ sơ chuyển từ bước góp ý sang và hồ sơ được tạo mới trực tiếp.";
            ViewData["ChuyenHoSoTitle"] = "Chuyển xét duyệt dự thảo";
            ViewData["ChuyenHoSoSubtitle"] = "Chọn đơn vị tiếp nhận và thời hạn xử lý trước khi chuyển hồ sơ dự thảo sang bước xét duyệt.";
            ViewData["ChuyenHoSoSubmitLabel"] = "Chuyển xét duyệt dự thảo";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/DuThaoVanBan/CreatePage")]
        [AuthorizeAction("Create", "DuThaoVanBan", "Index")]
        public async Task<IActionResult> CreatePage()
        {
            ViewData["Title"] = "Dự thảo văn bản";
            ViewData["PageTitle"] = "Thêm mới hồ sơ dự thảo";
            ViewData["PageSubtitle"] = "Khởi tạo trực tiếp hồ sơ soạn thảo mà không cần đi qua bước đăng ký.";
            ViewData["FormAction"] = "/Manages/DuThaoVanBan/Store";
            ViewData["SubmitLabel"] = "Thêm mới";
            ViewData["BackUrl"] = "/Manages/DuThaoVanBan";
            ViewData["DonViLabel"] = "Đơn vị soạn thảo";
            ViewData["DanhMucVanBans"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["QuyTrinhSoanThaos"] = await GetXayDungWorkflowOptionsAsync();
            ViewData["WorkflowStepUrl"] = "/Manages/DuThaoVanBan/LoadWorkflowSteps";
            ViewData["AttachedFileTableName"] = "HoSoVanBan";
            ViewData["AttachedFilePageInfo"] = FuntionGlobal.GetPageInfo(0, string.Empty, 5, 1);
            await PopulateCreateViewDataAsync();

            var model = new HoSoVanBanCreateModel
            {
                Id = Guid.NewGuid(),
                DonViDeNghiId = ResolveCreateDonViId(),
                HanXuLy = DateTime.Today.AddDays(7),
                TuNgaySoanThao = DateTime.Today,
                DenNgaySoanThao = DateTime.Today.AddDays(7)
            };
            model.QuyTrinhSoanThaoId = (ViewData["QuyTrinhSoanThaos"] as List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>)?.FirstOrDefault()?.Id ?? Guid.Empty;

            return View("Views/Admin/Manages/HoSoVanBan/Create.cshtml", model);
        }

        [HttpGet("Manages/DuThaoVanBan/LoadWorkflowSteps")]
        [AuthorizeAction("Create", "DuThaoVanBan", "Index")]
        public async Task<JsonResult> LoadWorkflowSteps(Guid quyTrinhSoanThaoId)
        {
            var data = await _hoSoVanBanWorkflowService.GetBuocThoiHanOptionsAsync(quyTrinhSoanThaoId);
            return Json(data);
        }

        [HttpPost("Manages/DuThaoVanBan/Store")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Create", "DuThaoVanBan", "Index")]
        public async Task<IActionResult> Store(HoSoVanBanCreateModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (request.QuyTrinhSoanThaoId == Guid.Empty)
            {
                request.QuyTrinhSoanThaoId = (await GetXayDungWorkflowOptionsAsync()).FirstOrDefault()?.Id ?? Guid.Empty;
            }

            if (!(currentUser?.SSA ?? false))
            {
                request.DonViDeNghiId = currentUser?.DanhMucDonViId;
            }

            var model = await _hoSoVanBanWorkflowService.CreateHoSoAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "DuThaoVanBan");
        }

        [HttpPost("Manages/DuThaoVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "DuThaoVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/DuThaoVanBan";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/DuThaoVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "DuThaoVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/DuThaoVanBan/ChuyenHoSo")]
        [AuthorizeAction("Edit", "DuThaoVanBan", "Index")]
        public async Task<IActionResult> ChuyenHoSo(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChuyenXetDuyetDuThaoModelAsync(id);
            if (model.Status == "error" || model.Data is not HoSoVanBanXuLyStepModel chuyenHoSoModel)
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DonViOptions"] = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            ViewData["EnableDraftTransferFiles"] = true;
            ViewData["DraftTransferFileTableName"] = "HoSoVanBanDuThao";
            return PartialView("Views/Admin/Manages/HoSoVanBan/_ChuyenHoSo.cshtml", chuyenHoSoModel);
        }

        [HttpPost("Manages/DuThaoVanBan/HoanThanhXuLy")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DuThaoVanBan", "Index")]
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

        private Guid? ResolveCreateDonViId()
        {
            var currentUser = _authService.GetUserInfo();
            return currentUser?.DanhMucDonViId != Guid.Empty ? currentUser?.DanhMucDonViId : null;
        }

        private async Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>> GetXayDungWorkflowOptionsAsync()
        {
            return await _hoSoVanBanWorkflowService.GetQuyTrinhOptionsAsync(loaiQuyTrinh: "XayDung");
        }
    }
}
