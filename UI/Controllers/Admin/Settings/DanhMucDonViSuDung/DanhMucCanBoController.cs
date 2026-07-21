using DataAccess.Entities.Settings;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Settings;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucDonViSuDung
{
    [SetViewDataFilter]
    public class DanhMucCanBoController(
       IDanhMucCanBoService danhMucCanBoService,
       IDanhMucDonViService danhMucDonViService,
       IDanhMucPhongBanService danhMucPhongBanService) : Controller
    {
        [HttpGet("Settings/DanhMucDonViSuDung/DanhMucCanBo")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string Search = "", int PageSize = 5, int PageCurrent = 1, Guid? DonViId = null, Guid? PhongBanId = null, string Status = "")
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            // Lấy ID đơn vị của người dùng hiện tại nếu chưa chọn
            var userDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);
            if (DonViId == null || DonViId == Guid.Empty)
            {
                DonViId = userDonViId;
            }

            // Lấy danh sách đơn vị theo cấp cha con
            var danhMucDonVis = await danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucDonVis"] = danhMucDonVis;
            ViewData["DonViId"] = DonViId;

            // Lấy danh sách phòng ban theo đơn vị được chọn
            var phongBans = await danhMucPhongBanService.GetDanhMucPhongBanAsync("", 100, 1, DonViId);
            ViewData["DanhMucPhongBans"] = phongBans.Status == "success" ? phongBans.Data : new List<DanhMucPhongBan>();
            ViewData["PhongBanId"] = PhongBanId;

            // Lưu trạng thái đã chọn
            ViewData["Status"] = Status;

            var model = await danhMucCanBoService.GetDanhMucCanBoAsync(Search, PageSize, PageCurrent, DonViId, PhongBanId, Status);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, Search, PageSize, PageCurrent);
            return View("Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/Index.cshtml", model.Data);
        }

        [HttpGet("Settings/DanhMucDonViSuDung/DanhMucCanBo/Create")]
        [AuthorizeAction("Create")]
        public async Task<IActionResult> Create()
        {
            var userDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);

            var model = new DanhMucCanBo
            {
                Id = Guid.NewGuid(),
                LoaiLaoDong = LoaiLaoDong.CongChungVien,
                NgaySinh = DateTime.Now.AddYears(-30),
                Status = "Kích hoạt",
                GioiTinh = true,
                DonViQuanLyId = userDonViId // Sử dụng đơn vị của người dùng
            };

            // Lấy danh sách phòng ban của đơn vị hiện tại
            var phongBans = await danhMucPhongBanService.GetDanhMucPhongBanAsync("", 100, 1, userDonViId);
            ViewData["DanhMucPhongBans"] = phongBans.Status == "success" ? phongBans.Data : new List<DanhMucPhongBan>();
            ViewData["DonViQuanLyId"] = userDonViId;
            ViewData["FromCreate"] = true;
            return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/_FormFields.cshtml", model);
        }

        [HttpPost("Settings/DanhMucDonViSuDung/DanhMucCanBo/Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucCanBo request)
        {
            var userDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);
            request.DonViQuanLyId = userDonViId; // Đảm bảo đơn vị luôn là đơn vị của người dùng

            ModelState.Remove("Status");
            request.Status = "Kích hoạt";

            if (!ModelState.IsValid)
            {
                var phongBans = await danhMucPhongBanService.GetDanhMucPhongBanAsync("", 100, 1, userDonViId);
                ViewData["DanhMucPhongBans"] = phongBans.Status == "success" ? phongBans.Data : new List<DanhMucPhongBan>();
                ViewData["DonViQuanLyId"] = userDonViId;
                ViewData["FromCreate"] = true;
                return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/_FormFields.cshtml", request);
            }

            var model = await danhMucCanBoService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                var phongBans = await danhMucPhongBanService.GetDanhMucPhongBanAsync("", 100, 1, userDonViId);
                ViewData["DanhMucPhongBans"] = phongBans.Status == "success" ? phongBans.Data : new List<DanhMucPhongBan>();
                ViewData["DonViQuanLyId"] = userDonViId;
                ViewData["FromCreate"] = true;
                return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/_FormFields.cshtml", request);
            }
            return RedirectToAction("Index", "DanhMucCanBo");
        }

        [HttpPost("Settings/DanhMucDonViSuDung/DanhMucCanBo/Edit")]
        [AuthorizeAction("Edit")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await danhMucCanBoService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            if (model.Data == null)
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            var userDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);
            var phongBans = await danhMucPhongBanService.GetDanhMucPhongBanAsync("", 100, 1, userDonViId);
            ViewData["DanhMucPhongBans"] = phongBans.Status == "success" ? phongBans.Data : new List<DanhMucPhongBan>();
            ViewData["DonViQuanLyId"] = userDonViId;
            ViewData["FromCreate"] = false;
            return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/_FormFields.cshtml", model.Data);
        }

        [HttpPost("Settings/DanhMucDonViSuDung/DanhMucCanBo/Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DanhMucCanBo request)
        {
            ModelState.Remove("Username");
            ModelState.Remove("Email");
            ModelState.Remove("Password");

            var userDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);
            request.DonViQuanLyId = userDonViId; // Đảm bảo đơn vị luôn là đơn vị của người dùng

            if (!ModelState.IsValid)
            {
                var phongBans = await danhMucPhongBanService.GetDanhMucPhongBanAsync("", 100, 1, userDonViId);
                ViewData["DanhMucPhongBans"] = phongBans.Status == "success" ? phongBans.Data : new List<DanhMucPhongBan>();
                ViewData["DonViQuanLyId"] = userDonViId;
                ViewData["FromCreate"] = false;
                return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/_FormFields.cshtml", request);
            }

            var model = await danhMucCanBoService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                var phongBans = await danhMucPhongBanService.GetDanhMucPhongBanAsync("", 100, 1, userDonViId);
                ViewData["DanhMucPhongBans"] = phongBans.Status == "success" ? phongBans.Data : new List<DanhMucPhongBan>();
                ViewData["DonViQuanLyId"] = userDonViId;
                ViewData["FromCreate"] = false;
                return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/_FormFields.cshtml", request);
            }
            return RedirectToAction("Index", "DanhMucCanBo");
        }

        [HttpPost("Settings/DanhMucDonViSuDung/DanhMucCanBo/Delete")]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await danhMucCanBoService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucCanBo");
        }

        [HttpPost("Settings/DanhMucDonViSuDung/DanhMucCanBo/Show")]
        [AuthorizeAction("Show")]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await danhMucCanBoService.EditAsync(Id);
            if (model.Status == "error" || model.Data == null)
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("~/Views/Admin/Settings/DanhMucDonViSuDung/DanhMucCanBo/Show.cshtml", model.Data);
        }
    }
}