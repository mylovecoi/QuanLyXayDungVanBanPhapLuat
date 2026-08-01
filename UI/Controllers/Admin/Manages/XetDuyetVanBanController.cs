using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.ReportGenerators;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class XetDuyetVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/XetDuyetVanBan")]
        [AuthorizeAction("Index", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoBuocAsync(
                TimKiem,
                "BUOC_03_THAM_DINH_VAN_BAN",
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

            ViewData["Title"] = "Thẩm định văn bản";
            ViewData["Role"] = "VanBanQPPL.DanhGiaVanBan.XetDuyetVanBan";
            ViewData["RoutePrefix"] = "/Manages/XetDuyetVanBan";
            ViewData["HideDonViFilter"] = !isSSA;
            ViewData["ReceiveModeMessage"] = "Đang xem danh sách hồ sơ xử lý ở bước Thẩm định văn bản sau khi đơn vị soạn thảo chuyển dự thảo sang bước tiếp theo.";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/HoSoVanBan/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/XetDuyetVanBan/WorkflowPage")]
        [AuthorizeAction("Edit", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> WorkflowPage(Guid id)
        {
            ViewData["Title"] = "Thẩm định văn bản";
            ViewData["PageTitle"] = "Xét duyệt văn bản";
            ViewData["PageSubtitle"] = "Cập nhật kết quả thẩm định trên màn hình nghiệp vụ riêng.";
            ViewData["RoutePrefix"] = "/Manages/XetDuyetVanBan";
            ViewData["WorkflowActionTitle"] = "Thẩm định văn bản";
            ViewData["ReviewMode"] = "DuThao";
            ViewData["ForceWorkflowAction"] = "true";
            ViewData["WorkflowPageMode"] = "true";
            ViewData["CompactWorkflowPage"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return View("Views/Admin/Manages/XetDuyetVanBan/WorkflowPage.cshtml", model.Data);
        }

        [HttpGet("Manages/XetDuyetVanBan/SoSanhDuThao")]
        [AuthorizeAction("Index", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> SoSanhDuThao(Guid id, Guid? sourceFileId = null, Guid? targetFileId = null)
        {
            ViewData["Title"] = "So sánh dự thảo";
            var model = await _hoSoVanBanWorkflowService.GetSoSanhDuThaoAsync(id, sourceFileId, targetFileId);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return View("Views/Admin/Manages/XetDuyetVanBan/SoSanhDuThao.cshtml", model.Data);
        }

        [HttpGet("Manages/XetDuyetVanBan/ExportSoSanhDuThaoWord")]
        [AuthorizeAction("Index", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> ExportSoSanhDuThaoWord(Guid id, Guid? sourceFileId = null, Guid? targetFileId = null)
        {
            var response = await _hoSoVanBanWorkflowService.GetSoSanhDuThaoAsync(id, sourceFileId, targetFileId);
            if (response.Status == "error" || response.Data is not HoSoVanBanDraftCompareModel model)
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "XetDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var fileName = $"SoSanhDuThao_{model.HoSoVanBanId:N}.docx";
            var fileBytes = HoSoVanBanDraftCompareWordReportGenerator.GenerateReport(model);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        [HttpGet("Manages/XetDuyetVanBan/ExportSoSanhDuThaoPdf")]
        [AuthorizeAction("Index", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> ExportSoSanhDuThaoPdf(Guid id, Guid? sourceFileId = null, Guid? targetFileId = null)
        {
            ViewData["Title"] = "Xuất PDF so sánh dự thảo";
            var response = await _hoSoVanBanWorkflowService.GetSoSanhDuThaoAsync(id, sourceFileId, targetFileId);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "XetDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return View("Views/Admin/Manages/XetDuyetVanBan/SoSanhDuThaoPdf.cshtml", response.Data);
        }

        [HttpPost("Manages/XetDuyetVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/XetDuyetVanBan";
            ViewData["HideWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetVanBan/WorkflowAction")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> WorkflowAction(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/XetDuyetVanBan";
            ViewData["WorkflowActionTitle"] = "Thẩm định văn bản";
            ViewData["ReviewMode"] = "DuThao";
            ViewData["ForceWorkflowAction"] = "true";

            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/XetDuyetVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "XetDuyetVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "XetDuyetVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/XetDuyetVanBan/ChuyenPheDuyetModel")]
        [AuthorizeAction("Edit", "XetDuyetVanBan", "Index")]
        public async Task<JsonResult> ChuyenPheDuyetModel(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChuyenPheDuyetModelAsync(id);
            return Json(new
            {
                status = model.Status,
                message = model.Message,
                data = model.Data
            });
        }

        [HttpPost("Manages/XetDuyetVanBan/NhanHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetVanBan", "Index")]
        public async Task<JsonResult> NhanHoSo(Guid id, string actionType = "NHAN_HO_SO", string? noiDungXuLy = null, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.NhanHoSoAsync(id, actionType, noiDungXuLy, ghiChu);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/XetDuyetVanBan/TraLaiHoSo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetVanBan", "Index")]
        public async Task<JsonResult> TraLaiHoSo(Guid id, string lyDoTraLai, string? ghiChu = null)
        {
            var model = await _hoSoVanBanWorkflowService.TraLaiDanhGiaAsync(id, lyDoTraLai, ghiChu);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/XetDuyetVanBan/HoanThanhDanhGia")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "XetDuyetVanBan", "Index")]
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
