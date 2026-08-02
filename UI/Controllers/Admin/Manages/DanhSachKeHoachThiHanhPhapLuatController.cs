using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class DanhSachKeHoachThiHanhPhapLuatController(
        IThiHanhPhapLuatService thiHanhPhapLuatService,
        IAuthService authService) : Controller
    {
        private readonly IThiHanhPhapLuatService _thiHanhPhapLuatService = thiHanhPhapLuatService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/DanhSachKeHoachThiHanhPhapLuat")]
        [AuthorizeAction("Index", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId, currentUser);
            var model = await _thiHanhPhapLuatService.GetDanhSachKeHoachAsync(TimKiem, selectedDonViId, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                return BuildError(model.Message, "Home", "Index");
            }

            ViewData["Title"] = "Danh sách kế hoạch tổ chức thi hành pháp luật";
            ViewData["Role"] = "ThiHanhPhapLuat.DanhSachKeHoach";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/ThiHanhPhapLuat/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/DanhSachKeHoachThiHanhPhapLuat/Edit")]
        [AuthorizeAction("Edit", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var model = await _thiHanhPhapLuatService.GetKeHoachFormAsync(id);
            if (model.Status == "error")
            {
                return BuildError(model.Message, "DanhSachKeHoachThiHanhPhapLuat", "Index");
            }

            await LoadKeHoachFormViewDataAsync();
            ViewData["Title"] = (id.HasValue && id.Value != Guid.Empty) ? "Cập nhật kế hoạch" : "Thêm mới kế hoạch";
            return View("Views/Admin/Manages/ThiHanhPhapLuat/Edit.cshtml", model.Data);
        }

        [HttpPost("Manages/DanhSachKeHoachThiHanhPhapLuat/Edit")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Save(ThiHanhPhapLuatKeHoachFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return BuildError("Không xác định được người dùng.", "DanhSachKeHoachThiHanhPhapLuat", "Index");
            }

            var result = await _thiHanhPhapLuatService.SaveKeHoachAsync(request, currentUser);
            if (result.Status == "error")
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadKeHoachFormViewDataAsync();
                ViewData["Title"] = request.Id == Guid.Empty ? "Thêm mới kế hoạch" : "Cập nhật kế hoạch";
                return View("Views/Admin/Manages/ThiHanhPhapLuat/Edit.cshtml", request);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet("Manages/DanhSachKeHoachThiHanhPhapLuat/Details")]
        [AuthorizeAction("Details", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _thiHanhPhapLuatService.GetChiTietKeHoachAsync(id);
            if (model.Status == "error")
            {
                return BuildError(model.Message, "DanhSachKeHoachThiHanhPhapLuat", "Index");
            }

            await LoadChiTietViewDataAsync();
            ViewData["Title"] = "Chi tiết kế hoạch";
            ViewData["RoutePrefix"] = "/Manages/DanhSachKeHoachThiHanhPhapLuat";
            return View("Views/Admin/Manages/ThiHanhPhapLuat/Details.cshtml", model.Data);
        }

        [HttpPost("Manages/DanhSachKeHoachThiHanhPhapLuat/SaveNhiemVu")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> SaveNhiemVu(ThiHanhPhapLuatNhiemVuFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Không xác định được người dùng.";
                return RedirectToAction("Details", new { id = request.KeHoachId });
            }

            var result = await _thiHanhPhapLuatService.SaveNhiemVuAsync(request, currentUser);
            TempData[result.Status == "success" ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction("Details", new { id = request.KeHoachId });
        }

        [HttpPost("Manages/DanhSachKeHoachThiHanhPhapLuat/DeleteNhiemVu")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> DeleteNhiemVu(Guid id, Guid keHoachId)
        {
            var result = await _thiHanhPhapLuatService.DeleteNhiemVuAsync(id);
            TempData[result.Status == "success" ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction("Details", new { id = keHoachId });
        }

        [HttpPost("Manages/DanhSachKeHoachThiHanhPhapLuat/SaveChiTietNhiemVu")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> SaveChiTietNhiemVu(ThiHanhPhapLuatChiTietNhiemVuFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Không xác định được người dùng.";
                return RedirectToAction("Details", new { id = request.KeHoachId });
            }

            var result = await _thiHanhPhapLuatService.SaveChiTietNhiemVuAsync(request, currentUser);
            TempData[result.Status == "success" ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction("Details", new { id = request.KeHoachId });
        }

        [HttpPost("Manages/DanhSachKeHoachThiHanhPhapLuat/DeleteChiTietNhiemVu")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhSachKeHoachThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> DeleteChiTietNhiemVu(Guid id, Guid keHoachId)
        {
            var result = await _thiHanhPhapLuatService.DeleteChiTietNhiemVuAsync(id);
            TempData[result.Status == "success" ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction("Details", new { id = keHoachId });
        }

        private async Task LoadKeHoachFormViewDataAsync()
        {
            ViewData["DonViOptions"] = await _thiHanhPhapLuatService.GetDonViOptionsAsync();
            ViewData["DanhMucVanBanOptions"] = await _thiHanhPhapLuatService.GetDanhMucVanBanOptionsAsync();
            ViewData["TrangThaiOptions"] = BuildTrangThaiKeHoachOptions();
        }

        private async Task LoadChiTietViewDataAsync()
        {
            ViewData["DonViOptions"] = await _thiHanhPhapLuatService.GetDonViOptionsAsync();
            ViewData["NguoiDungOptions"] = await _thiHanhPhapLuatService.GetNguoiDungOptionsAsync();
            ViewData["TrangThaiKeHoachOptions"] = BuildTrangThaiKeHoachOptions();
            ViewData["TrangThaiNhiemVuOptions"] = BuildTrangThaiNhiemVuOptions();
            ViewData["MucDoUuTienOptions"] = BuildMucDoUuTienOptions();
            ViewData["LoaiChiTietOptions"] = BuildLoaiChiTietOptions();
        }

        private async Task<Guid?> ApplyDonViFilterViewDataAsync(Guid? donViId, User? currentUser)
        {
            var donViOptions = await _thiHanhPhapLuatService.GetDonViOptionsAsync();
            var canSelectAll = currentUser?.SSA ?? false;
            var selectedDonViId = canSelectAll
                ? (donViId.HasValue && donViId.Value != Guid.Empty ? donViId : null)
                : ((currentUser?.DanhMucDonViId ?? Guid.Empty) != Guid.Empty ? currentUser!.DanhMucDonViId : null);

            if (!canSelectAll && selectedDonViId.HasValue)
            {
                donViOptions = donViOptions.Where(x => x.Id == selectedDonViId.Value).ToList();
            }

            ViewData["DonViOptions"] = donViOptions;
            ViewData["SelectedDonViId"] = selectedDonViId;
            ViewData["HideDonViFilter"] = !canSelectAll;
            return selectedDonViId;
        }

        private IActionResult BuildError(string? message, string controller, string action)
        {
            ViewData["Messages"] = message;
            ViewData["Controller"] = controller;
            ViewData["Action"] = action;
            return View("Views/Shared/Error.cshtml");
        }

        private static List<(string Value, string Text)> BuildTrangThaiKeHoachOptions() =>
        [
            ("NHAP", "Nháp"),
            ("CONG_BO", "Đã công bố"),
            ("DANG_THUC_HIEN", "Đang thực hiện"),
            ("HOAN_THANH", "Hoàn thành"),
            ("TAM_DUNG", "Tạm dừng")
        ];

        private static List<(string Value, string Text)> BuildTrangThaiNhiemVuOptions() =>
        [
            ("CHUA_THUC_HIEN", "Chưa thực hiện"),
            ("DANG_THUC_HIEN", "Đang thực hiện"),
            ("QUA_HAN", "Quá hạn"),
            ("HOAN_THANH", "Hoàn thành")
        ];

        private static List<(string Value, string Text)> BuildMucDoUuTienOptions() =>
        [
            ("THAP", "Thấp"),
            ("TRUNG_BINH", "Trung bình"),
            ("CAO", "Cao")
        ];

        private static List<(string Value, string Text)> BuildLoaiChiTietOptions() =>
        [
            ("CHI_TIEU", "Chỉ tiêu"),
            ("NHIEM_VU_CON", "Nhiệm vụ nhỏ")
        ];
    }
}
