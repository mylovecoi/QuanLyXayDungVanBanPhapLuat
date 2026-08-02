using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class TheoDoiTienDoXayDungController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private static readonly Guid SoTuPhapDonViId = Guid.Parse("40000000-0000-0000-0000-000000000002");
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/TheoDoiTienDoXayDung")]
        [AuthorizeAction("Index", "TheoDoiTienDoXayDung", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, string? MaBuoc = null, string? MucCanhBao = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var canViewAll = isSSA || currentUser?.DanhMucDonViId == SoTuPhapDonViId;
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId, canViewAll);

            var model = await _hoSoVanBanWorkflowService.GetDanhSachTheoDoiTienDoXayDungAsync(
                TimKiem,
                canViewAll ? selectedDonViId : currentUser?.DanhMucDonViId,
                MaBuoc,
                MucCanhBao,
                PageSize,
                PageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Theo dõi tiến độ xây dựng";
            ViewData["Role"] = "VanBanQPPL.XayDungVanBan.TheoDoiTienDoXayDung";
            ViewData["SelectedMaBuoc"] = MaBuoc;
            ViewData["SelectedMucCanhBao"] = MucCanhBao;
            ViewData["MaBuocOptions"] = await _hoSoVanBanWorkflowService.GetBuocTheoDoiTienDoOptionsAsync();
            ViewData["MucCanhBaoOptions"] = new List<SelectOptionModel>
            {
                new() { Value = "BINH_THUONG", Text = "Bình thường" },
                new() { Value = "SAP_DEN_HAN", Text = "Sắp đến hạn" },
                new() { Value = "QUA_HAN", Text = "Quá hạn" },
                new() { Value = "TRA_LAI_NHIEU", Text = "Trả lại nhiều lần" }
            };
            ViewData["HideDonViFilter"] = !canViewAll;
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);

            return View("Views/Admin/Manages/TheoDoiTienDoXayDung/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/TheoDoiTienDoXayDung/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TheoDoiTienDoXayDung", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "TheoDoiTienDoXayDung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["RoutePrefix"] = "/Manages/TheoDoiTienDoXayDung";
            ViewData["HideWorkflowAction"] = "true";
            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/TheoDoiTienDoXayDung/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TheoDoiTienDoXayDung", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "TheoDoiTienDoXayDung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        [HttpGet("Manages/TheoDoiTienDoXayDung/DonDoc")]
        [AuthorizeAction("Index", "TheoDoiTienDoXayDung", "Index")]
        public async Task<IActionResult> DonDoc(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetDonDocTienDoFormAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "TheoDoiTienDoXayDung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/TheoDoiTienDoXayDung/_DonDocModalBody.cshtml", model.Data);
        }

        [HttpPost("Manages/TheoDoiTienDoXayDung/DonDoc")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TheoDoiTienDoXayDung", "Index")]
        public async Task<IActionResult> GuiDonDoc(HoSoVanBanDonDocFormModel request)
        {
            var model = await _hoSoVanBanWorkflowService.GuiDonDocTienDoAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Manages/TheoDoiTienDoXayDung/DonDocHangLoat")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TheoDoiTienDoXayDung", "Index")]
        public async Task<IActionResult> DonDocHangLoat([FromForm] List<Guid> hoSoVanBanIds)
        {
            var model = await _hoSoVanBanWorkflowService.GetDonDocTienDoHangLoatFormAsync(hoSoVanBanIds);
            if (model.Status == "error")
            {
                return Json(new { status = model.Status, message = model.Message });
            }

            return PartialView("Views/Admin/Manages/TheoDoiTienDoXayDung/_DonDocHangLoatModalBody.cshtml", model.Data);
        }

        [HttpPost("Manages/TheoDoiTienDoXayDung/GuiDonDocHangLoat")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TheoDoiTienDoXayDung", "Index")]
        public async Task<IActionResult> GuiDonDocHangLoat(HoSoVanBanDonDocHangLoatFormModel request)
        {
            var model = await _hoSoVanBanWorkflowService.GuiDonDocTienDoHangLoatAsync(request);
            return Json(new { status = model.Status, message = model.Message });
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
