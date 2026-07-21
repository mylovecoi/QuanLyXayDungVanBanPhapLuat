using UI.Helper;
using UI.Security;
using UI.ViewModels;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Settings;
using Services.Systems;
using System.Data;

namespace UI.Controllers.Admin.Settings.DanhMucDungChung
{
    [SetViewDataFilter]  // Khai báo đệ tự động truyền ViewData["Title", "MenuActive", "Role"]   
    public class DanhMucDonViController : Controller
    {
        private readonly IDanhMucDonViService _danhMucDonViService;
        private readonly IRoleActionService _roleActionService;
        private readonly IUserService _userService;
        private readonly IGroupPermissionService _groupPermissionService;
        private readonly IOptionDataService _optionDataService;

        private ISession? _session => HttpContext?.Session;
        public DanhMucDonViController(IDanhMucDonViService danhMucDonViService, IRoleActionService roleActionService, IUserService userService,
                                      IGroupPermissionService groupPermissionService, IOptionDataService optionDataService)
        {
            _danhMucDonViService = danhMucDonViService;
            _roleActionService = roleActionService;
            _userService = userService;
            _groupPermissionService = groupPermissionService;
            _optionDataService = optionDataService;
        }

        [HttpGet("Settings/DanhMucDungChung/DanhMucDonVi")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await _danhMucDonViService.GetDanhMucDonViAsync(TimKiem, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Settings/DanhMucDungChung/DanhMucDonVi/Index.cshtml", model.Data);
        }

        [HttpPost("Settings/DanhMucDungChung/DanhMucDonVi/Create")]
        [AuthorizeAction("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid Id)
        {
            var data = new DanhMucDonVi { TenDonVi = "", STTSapXep = await _danhMucDonViService.GetSTTSapXep(Id), Level = 0 };

            var model = await _danhMucDonViService.GetDonViInfoAsync(Id);
            if (model.Status == "success")
            {
                data.DonViChuQuanId = model.Data?.Id ?? Guid.Empty;
                data.TenDonViChuQuan = model.Data?.TenDonVi ?? "";
                data.Level = model.Data?.Level + 1 ?? 0;
            }

            ViewData["GroupPermission"] = await _groupPermissionService.GetAllGroupPermissionsAsync("Kích hoạt");
            ViewData["UseGroups"] = await _optionDataService.GetDataOptionsByCodeAsync("DonVi");
            return PartialView("Views/Admin/Settings/DanhMucDungChung/DanhMucDonVi/_FormFields.cshtml", data);
        }


        [HttpPost("Settings/DanhMucDungChung/DanhMucDonVi/Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Store([FromForm] DanhMucDonVi request,
                                            [FromForm] string Username, [FromForm] string Email, 
                                            [FromForm] string Password, [FromForm] Guid GroupPermissionId)
        {
            if (await _userService.IsUserlExitAsync(Username, Email))
            {
                return Json(new { status = "error", message = "Username và Email đã được sử dụng!" });
            }

            Guid newId = Guid.NewGuid();
            var newUser = new User
            {
                Username = Username, Email = Email, Password = Password,
                Name = request.TenDonVi, DanhMucDonViId = newId, OTPSecretKey = "",
                Status = "Kích hoạt", FirstLogin = true, LoginCount = 0,
                Content = "Max", Menu = "Fixed", Theme = "Light",
                ChucDanhKy = request.ChucDanhQuanLy, HoTenNguoiKy = request.HoVaTenNguoiQuanLy,
                GroupPermissionId = GroupPermissionId,
            };
            var addUser = await _userService.StoreAsync(newUser);
            if(addUser.Status == "error")
            {
                return Json(new { status = "error", message = addUser.Message });
            }
            request.Id = newId;
            var addDonVi = await _danhMucDonViService.StoreAsync(request);
            if(addDonVi.Status == "error")
            {
                return Json(new { status = "error", message = addDonVi.Message });
            }
            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost("Settings/DanhMucDungChung/DanhMucDonVi/Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await _danhMucDonViService.EditAsync(Id);
            if(model.Status == "error")
            {
                ViewData["Message"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["UseGroups"] = await _optionDataService.GetDataOptionsByCodeAsync("DonVi");
            return PartialView("Views/Admin/Settings/DanhMucDungChung/DanhMucDonVi/_FormFields.cshtml", model.Data);
        }

        [HttpPost("Settings/DanhMucDungChung/DanhMucDonVi/Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucDonVi request)
        {
            var model = await _danhMucDonViService.UpdateAsync(request);
            if(model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucDonViService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Message"] = model.Message;
                ViewData["Controller"] = "DanhMucDonVi";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucDonVi");
        }
    }
}
