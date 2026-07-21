using UI.Helper;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using DataAccess.Entities.Systems;
using UI.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UI.Controllers.Admin.Systems
{
    public class RoleActionController : Controller
    {
        private readonly IRoleActionService _roleActionService;
        private readonly IOptionDataService _optionDataService;
        private ISession? _session => HttpContext?.Session;
        public RoleActionController(IRoleActionService roleActionService, IOptionDataService optionDataService)
        {            
            _roleActionService = roleActionService;
            _optionDataService = optionDataService;
        }

        [HttpGet("Systems/RoleAction")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn không có quyền truy cập vào chức năng này!Vui lòng liên hệ quản trị viên!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await _roleActionService.GetRolesAsync(TimKiem, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            ViewData["Title"] = "Danh sách chức năng chương trình";
            return View("Views/Admin/Systems/RoleAction/Index.cshtml", model.Data);
        }

        [HttpPost("Systems/RoleAction/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid GroupId)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _roleActionService.GetRoleActionInfoAsync(GroupId);
            var data = new RoleAction { 
                PhanLoai = "Group", Level = 0, RoleGroupId = GroupId, 
                Role = "", Status = "Kích hoạt", 
                STTSapXep = await _roleActionService.GetSTTSapXep(GroupId) 
            };
            if (model.Status == "success")
            {
                data.TitleRoleGroup = model.Data?.Title ?? "";
                data.Role = model.Data?.Role + "." ?? "";            
                data.Level = model.Data?.Level + 1 ?? 0;
                data.UseGroup = model.Data?.UseGroup ?? "";

            }
            ViewData["UseGroups"] = await _optionDataService.GetDataOptionsByCodeAsync("NhomQuyen");
            return PartialView("Views/Admin/Systems/RoleAction/_FormFields.cshtml", data);
        }

        [HttpPost("Systems/RoleAction/Store")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Store(RoleAction request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
                return Json(new { status = "error", message = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!" });

            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
                return Json(new { status = "error", message = "Bạn không có quyền truy cập vào chức năng này!Vui lòng liên hệ quản trị viên!" });

            
            if (await _roleActionService.CheckDuplicateAsync(request.Role, Guid.Empty))
            {
                var data = new { status = "error", message = "Có sự trùng lặp chức năng!Bạn cần kiểm tra lại!" };
                return Json(data);
            }

            var model = await _roleActionService.StoreAsync(request);
            if (model.Status == "error")
            {
                var data = new { status = "error", message = model.Message };
                return Json(data);
            }

            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost("Systems/RoleAction/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
                return Json(new { status = "error", message = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!" });

            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
                return Json(new { status = "error", message = "Bạn không có quyền truy cập vào chức năng này!Vui lòng liên hệ quản trị viên!" });            

            var model = await _roleActionService.EditAsync(Id);            
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            var groupPer = await _roleActionService.GetRoleActionInfoAsync(model.Data?.RoleGroupId ?? Guid.Empty);
            if(model.Data == null)
            {
                ViewData["Messages"] = "Không tìm thấy thông tin chức năng!";
                return View("Views/Shared/Error.cshtml");
            }
            model.Data.TitleRoleGroup = groupPer.Data?.Title ?? "";
            ViewData["UseGroups"] = await _optionDataService.GetDataOptionsByCodeAsync("NhomQuyen");
            return PartialView("Views/Admin/Systems/RoleAction/_FormFields.cshtml", model.Data);
        }

        [HttpPost("Systems/RoleAction/Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(RoleAction request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
                return Json(new { status = "error", message = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!" });

            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
                return Json(new { status = "error", message = "Bạn không có quyền truy cập vào chức năng này!Vui lòng liên hệ quản trị viên!" });
            
            if (await _roleActionService.CheckDuplicateAsync(request.Role, request.Id))
                return Json(new { status = "error", message = "Có sự trùng lặp chức năng! Bạn cần kiểm tra lại!" });

            var model = await _roleActionService.UpdateAsync(request);
            if (model.Status == "error")
            {
                var data = new { status = "error", message = model.Message };
                return Json(data);
            }

            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn không có quyền truy cập vào chức năng này!Vui lòng liên hệ quản trị viên!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _roleActionService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Shared/Error");
            }
            return RedirectToAction("Index", "RoleAction");
        }
    }
}