using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.Systems
{
    [SetViewDataFilter]  // Khai báo đệ tự động truyền ViewData["Title", "MenuActive", "Role"]   
    public class UserController : Controller
    {
        private readonly IRoleActionService _roleActionService;
        private readonly IUserService _userService;
        private readonly IGroupPermissionService _groupPermissionService;

        public UserController(IRoleActionService roleActionService, IUserService userService, IGroupPermissionService groupPermissionService)
        {
            _roleActionService = roleActionService;
            _userService = userService;
            _groupPermissionService = groupPermissionService;
        }

        [HttpGet("Systems/User")]
        [AuthorizeAction("Index")]

        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1, string Level = "")
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await _userService.GetUsersAsync(TimKiem, PageSize, PageCurrent, Level);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            ViewData["Level"] = Level;
            return View("Views/Admin/Systems/User/Index.cshtml", model.Data);
        }

        [HttpPost("Systems/User/Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await _userService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            var groupPermissions = await _groupPermissionService.GetGroupPermissionsAsync("Kích hoạt");
            ViewData["GroupsPermision"] = groupPermissions.Data;
            return PartialView("Views/Admin/Systems/User/Edit.cshtml", model.Data);
        }

        [HttpPost("Systems/User/Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(User request)
        {
            var model = await _userService.UpdateAsync(request);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost("Systems/User/ResetPassword")]
        [AuthorizeAction("Update", "User", "Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPassword(Guid Id)
        {
            var model = await _userService.ResetPasswordAsync(Id);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost("Systems/User/Active")]
        [AuthorizeAction("Update", "User", "Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Active(Guid Id, string Status)
        {
            var model = await _userService.ActiveAsync(Id, Status);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost("Systems/User/Duplicate")]
        [AuthorizeAction("Store", "User", "Create")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Duplicate(Guid IdDuplicate, string Username, string Name, string Email)
        {
            if (await _userService.IsUserlExitAsync(Username, Email))
            {
                return Json(new { status = "error", message = "Username và Email đã tồn tại!" });
            }
            var model = await _userService.DuplicateAsync(IdDuplicate, Username, Name, Email);
            if (model.Status == "error")
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
            var model = await _userService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "User";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "User");
        }
    }
}