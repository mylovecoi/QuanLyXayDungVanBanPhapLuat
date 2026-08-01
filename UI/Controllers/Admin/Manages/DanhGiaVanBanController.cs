using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class DanhGiaVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/DanhGiaVanBan")]
        [AuthorizeAction("Index", "DanhGiaVanBan", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(
                TimKiem,
                "BUOC_02_GUI_THAM_DINH",
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

            ViewData["Title"] = "Danh sách văn bản đánh giá";
            ViewData["Role"] = "VanBanQPPL.DanhGiaVanBan.XetDuyetVanBan";
            ViewData["RoutePrefix"] = "/Manages/DanhGiaVanBan";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem danh sách hồ sơ được chuyển từ Dự thảo văn bản sang bước đánh giá để theo dõi tình trạng xử lý.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/DanhGiaVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "DanhGiaVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/DanhGiaVanBan";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhGiaVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/DanhGiaVanBan/WorkflowAction")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "DanhGiaVanBan", "Index")]
        public async Task<IActionResult> WorkflowAction(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/DanhGiaVanBan";
            ViewData["WorkflowActionTitle"] = "Đánh giá dự thảo văn bản";
            ViewData["ReviewMode"] = "DuThao";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhGiaVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/DanhGiaVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "DanhGiaVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhGiaVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/DanhGiaVanBan/ChuyenDanhGiaModel")]
        [AuthorizeAction("Edit", "DanhGiaVanBan", "Index")]
        public async Task<JsonResult> ChuyenDanhGiaModel(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChuyenDanhGiaModelAsync(id);
            return Json(new
            {
                status = model.Status,
                message = model.Message,
                data = model.Data
            });
        }

        [HttpPost("Manages/DanhGiaVanBan/NhanHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhGiaVanBan", "Index")]
        public async Task<JsonResult> NhanHoSo(Guid id, string actionType = "NHAN_HO_SO", string? noiDungXuLy = null, string? ghiChu = null, DateTime? ngayXuLy = null, DateTime? hanXuLy = null)
        {
            var model = await _hoSoVanBanWorkflowService.NhanHoSoAsync(id, actionType, noiDungXuLy, ghiChu, ngayXuLy, hanXuLy);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/DanhGiaVanBan/TraLaiHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhGiaVanBan", "Index")]
        public async Task<JsonResult> TraLaiHoSo(Guid id, string lyDoTraLai, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.TraLaiDanhGiaAsync(id, lyDoTraLai, ghiChu);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/DanhGiaVanBan/HoanThanhDanhGia")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhGiaVanBan", "Index")]
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
