using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class QuaTrinhToChucThucHienController(
        IThiHanhPhapLuatService thiHanhPhapLuatService,
        IAuthService authService) : Controller
    {
        private readonly IThiHanhPhapLuatService _thiHanhPhapLuatService = thiHanhPhapLuatService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/QuaTrinhToChucThucHien")]
        [AuthorizeAction("Index", "QuaTrinhToChucThucHien", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var currentUser = _authService.GetUserInfo();
            var selectedDonViId = (currentUser?.SSA ?? false)
                ? (DonViId.HasValue && DonViId.Value != Guid.Empty ? DonViId : null)
                : ((currentUser?.DanhMucDonViId ?? Guid.Empty) != Guid.Empty ? currentUser!.DanhMucDonViId : null);

            var donViOptions = await _thiHanhPhapLuatService.GetDonViOptionsAsync();
            if (!(currentUser?.SSA ?? false) && selectedDonViId.HasValue)
            {
                donViOptions = donViOptions.Where(x => x.Id == selectedDonViId.Value).ToList();
            }

            var model = await _thiHanhPhapLuatService.GetDanhSachTienDoAsync(TimKiem, selectedDonViId, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Quá trình tổ chức thực hiện";
            ViewData["Role"] = "ThiHanhPhapLuat.QuaTrinhToChucThucHien";
            ViewData["DonViOptions"] = donViOptions;
            ViewData["SelectedDonViId"] = selectedDonViId;
            ViewData["HideDonViFilter"] = !(currentUser?.SSA ?? false);
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/ThiHanhPhapLuat/TienDoIndex.cshtml", model.Data);
        }

        [HttpGet("Manages/QuaTrinhToChucThucHien/CapNhatTienDo")]
        [AuthorizeAction("Edit", "QuaTrinhToChucThucHien", "Index")]
        public async Task<IActionResult> CapNhatTienDo(Guid chiTietNhiemVuId)
        {
            var model = await _thiHanhPhapLuatService.GetTienDoFormAsync(chiTietNhiemVuId);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "QuaTrinhToChucThucHien";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Cập nhật tiến độ";
            ViewData["TrangThaiBaoCaoOptions"] = new List<(string Value, string Text)>
            {
                ("NHAP", "Nháp"),
                ("DA_GUI", "Đã gửi"),
                ("DA_XAC_NHAN", "Đã xác nhận")
            };
            return View("Views/Admin/Manages/ThiHanhPhapLuat/TienDoEdit.cshtml", model.Data);
        }

        [HttpPost("Manages/QuaTrinhToChucThucHien/CapNhatTienDo")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "QuaTrinhToChucThucHien", "Index")]
        public async Task<IActionResult> CapNhatTienDo(ThiHanhPhapLuatTienDoFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                ViewData["Messages"] = "Không xác định được người dùng.";
                ViewData["Controller"] = "QuaTrinhToChucThucHien";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var result = await _thiHanhPhapLuatService.SaveTienDoAsync(request, currentUser);
            if (result.Status == "error")
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["Title"] = "Cập nhật tiến độ";
                ViewData["TrangThaiBaoCaoOptions"] = new List<(string Value, string Text)>
                {
                    ("NHAP", "Nháp"),
                    ("DA_GUI", "Đã gửi"),
                    ("DA_XAC_NHAN", "Đã xác nhận")
                };
                return View("Views/Admin/Manages/ThiHanhPhapLuat/TienDoEdit.cshtml", request);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
