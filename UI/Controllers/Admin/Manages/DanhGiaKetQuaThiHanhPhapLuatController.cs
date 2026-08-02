using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class DanhGiaKetQuaThiHanhPhapLuatController(
        IThiHanhPhapLuatService thiHanhPhapLuatService,
        IAuthService authService) : Controller
    {
        private readonly IThiHanhPhapLuatService _thiHanhPhapLuatService = thiHanhPhapLuatService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/DanhGiaKetQuaThiHanhPhapLuat")]
        [AuthorizeAction("Index", "DanhGiaKetQuaThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, string? CanhBao = null, int PageSize = 5, int PageCurrent = 1)
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

            var model = await _thiHanhPhapLuatService.GetDanhSachDanhGiaAsync(TimKiem, selectedDonViId, CanhBao, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "ÄĂ¡nh giĂ¡ káº¿t quáº£ thá»±c hiá»‡n";
            ViewData["Role"] = "ThiHanhPhapLuat.DanhGiaKetQua";
            ViewData["DonViOptions"] = donViOptions;
            ViewData["SelectedDonViId"] = selectedDonViId;
            ViewData["HideDonViFilter"] = !(currentUser?.SSA ?? false);
            ViewData["SelectedCanhBao"] = CanhBao;
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/ThiHanhPhapLuat/DanhGia.cshtml", model.Data);
        }

        [HttpGet("Manages/DanhGiaKetQuaThiHanhPhapLuat/Details")]
        [AuthorizeAction("Details", "DanhGiaKetQuaThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Details(Guid id)
        {
            return RedirectToAction("Details", "DanhSachKeHoachThiHanhPhapLuat", new { id });
        }

        [HttpGet("Manages/DanhGiaKetQuaThiHanhPhapLuat/Edit")]
        [AuthorizeAction("Edit", "DanhGiaKetQuaThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Edit(Guid keHoachId)
        {
            var model = await _thiHanhPhapLuatService.GetDanhGiaFormAsync(keHoachId);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhGiaKetQuaThiHanhPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "ÄĂ¡nh giĂ¡ káº¿t quáº£ thá»±c hiá»‡n";
            ViewData["KetQuaOptions"] = new List<(string Value, string Text)>
            {
                ("CHUA_THUC_HIEN", "ChÆ°a thá»±c hiá»‡n"),
                ("DANG_THUC_HIEN", "Äang thá»±c hiá»‡n"),
                ("HOAN_THANH", "HoĂ n thĂ nh"),
                ("KHONG_DAT", "KhĂ´ng Ä‘áº¡t")
            };
            ViewData["CanhBaoOptions"] = new List<(string Value, string Text)>
            {
                ("BINH_THUONG", "BĂ¬nh thÆ°á»ng"),
                ("CHUA_THUC_HIEN", "ChÆ°a thá»±c hiá»‡n"),
                ("CHUA_NHAP_LIEU", "ChÆ°a nháº­p liá»‡u"),
                ("CHAM_TIEN_DO", "Cháº­m tiáº¿n Ä‘á»™"),
                ("QUA_HAN", "QuĂ¡ háº¡n")
            };
            ViewData["TrangThaiOptions"] = new List<(string Value, string Text)>
            {
                ("NHAP", "NhĂ¡p"),
                ("CHINH_THUC", "ChĂ­nh thá»©c")
            };
            return View("Views/Admin/Manages/ThiHanhPhapLuat/DanhGiaEdit.cshtml", model.Data);
        }

        [HttpPost("Manages/DanhGiaKetQuaThiHanhPhapLuat/Edit")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhGiaKetQuaThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> Edit(ThiHanhPhapLuatDanhGiaFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                ViewData["Messages"] = "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c ngÆ°á»i dĂ¹ng.";
                ViewData["Controller"] = "DanhGiaKetQuaThiHanhPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var result = await _thiHanhPhapLuatService.SaveDanhGiaAsync(request, currentUser);
            if (result.Status == "error")
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["Title"] = "ÄĂ¡nh giĂ¡ káº¿t quáº£ thá»±c hiá»‡n";
                ViewData["KetQuaOptions"] = new List<(string Value, string Text)>
                {
                    ("CHUA_THUC_HIEN", "ChÆ°a thá»±c hiá»‡n"),
                    ("DANG_THUC_HIEN", "Äang thá»±c hiá»‡n"),
                    ("HOAN_THANH", "HoĂ n thĂ nh"),
                    ("KHONG_DAT", "KhĂ´ng Ä‘áº¡t")
                };
                ViewData["CanhBaoOptions"] = new List<(string Value, string Text)>
                {
                    ("BINH_THUONG", "BĂ¬nh thÆ°á»ng"),
                    ("CHUA_THUC_HIEN", "ChÆ°a thá»±c hiá»‡n"),
                    ("CHUA_NHAP_LIEU", "ChÆ°a nháº­p liá»‡u"),
                    ("CHAM_TIEN_DO", "Cháº­m tiáº¿n Ä‘á»™"),
                    ("QUA_HAN", "QuĂ¡ háº¡n")
                };
                ViewData["TrangThaiOptions"] = new List<(string Value, string Text)>
                {
                    ("NHAP", "NhĂ¡p"),
                    ("CHINH_THUC", "ChĂ­nh thá»©c")
                };
                return View("Views/Admin/Manages/ThiHanhPhapLuat/DanhGiaEdit.cshtml", request);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet("Manages/DanhGiaKetQuaThiHanhPhapLuat/TongHop")]
        [AuthorizeAction("Edit", "DanhGiaKetQuaThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> TongHop(Guid keHoachId)
        {
            var model = await _thiHanhPhapLuatService.GetTongHopFormAsync(keHoachId);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhGiaKetQuaThiHanhPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Tá»•ng há»£p káº¿t quáº£ thá»±c hiá»‡n";
            ViewData["TrangThaiOptions"] = new List<(string Value, string Text)>
            {
                ("NHAP", "NhĂ¡p"),
                ("CHINH_THUC", "ChĂ­nh thá»©c")
            };
            return View("Views/Admin/Manages/ThiHanhPhapLuat/TongHopEdit.cshtml", model.Data);
        }

        [HttpPost("Manages/DanhGiaKetQuaThiHanhPhapLuat/TongHop")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "DanhGiaKetQuaThiHanhPhapLuat", "Index")]
        public async Task<IActionResult> TongHop(ThiHanhPhapLuatTongHopFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                ViewData["Messages"] = "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c ngÆ°á»i dĂ¹ng.";
                ViewData["Controller"] = "DanhGiaKetQuaThiHanhPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var result = await _thiHanhPhapLuatService.SaveTongHopAsync(request, currentUser);
            if (result.Status == "error")
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["Title"] = "Tá»•ng há»£p káº¿t quáº£ thá»±c hiá»‡n";
                ViewData["TrangThaiOptions"] = new List<(string Value, string Text)>
                {
                    ("NHAP", "NhĂ¡p"),
                    ("CHINH_THUC", "ChĂ­nh thá»©c")
                };
                return View("Views/Admin/Manages/ThiHanhPhapLuat/TongHopEdit.cshtml", request);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
