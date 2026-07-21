using UI.Helper;
using UI.Security;
using UI.ViewModels;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using Azure;

namespace UI.Controllers.Admin.Systems
{
    [SetViewDataFilter]  // Khai báo đệ tự động truyền ViewData["Title", "MenuActive", "Role"]   
    public class GroupPermissionController : Controller
    {
        private readonly IGroupPermissionService _groupPermissionService;
        private readonly IPermissionService _permissionService;
        private readonly IOptionDataService _optionDataService;

        public GroupPermissionController(IGroupPermissionService groupPermissionService, IPermissionService permissionService,
                                            IOptionDataService optionDataService)
        {
            _groupPermissionService = groupPermissionService;
            _permissionService = permissionService;
            _optionDataService = optionDataService;
        }

        [HttpGet("Systems/GroupPermission")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await _groupPermissionService.GetGroupPermissionsAsync(TimKiem);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            ViewData["UseGroups"] = await _optionDataService.GetDataOptionsByCodeAsync("NhomQuyen");
            return View("Views/Admin/Systems/GroupPermission/Index.cshtml", model.Data);
        }

        [HttpPost]
        [AuthorizeAction("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string group_create)
        {
            //Xóa dữ liệu thừa
            await _permissionService.RemoveDatarRedundantAsync();
            //
            Guid newId = Guid.NewGuid();
            await _permissionService.StorePermissionsAsync(group_create, newId);
            var dataPer = await _permissionService.GetPermissionsByGroupIdAsync(newId);
            var model = new GroupPermision
            {
                Id = newId,
                Status = "Kích hoạt",
                Name = "",
                Permissions = dataPer?.Data ?? new List<Permission>()
            };
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(dataPer?.TotalRecord ?? 0, "", 5, 1);
            return View("Views/Admin/Systems/GroupPermission/Create.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(GroupPermision request)
        {

            var model = await _groupPermissionService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            await _permissionService.UpdateStatusByGroupIdAsync(request.Id);
            return RedirectToAction("Index", "GroupPermission");
        }

        [HttpGet]
        [AuthorizeAction("Edit")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await _groupPermissionService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GroupPermission";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            if (model.Data != null)
            {
                var response = await _permissionService.GetPermissionsByGroupIdAsync(Id);
                model.Data.Permissions = response?.Data ?? new List<Permission>();
                ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(response?.TotalRecord ?? 0, "", 5, 1);
            }
            return View("Views/Admin/Systems/GroupPermission/Edit.cshtml", model.Data);
        }

        [HttpPost]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(GroupPermision request)
        {
            var model = await _groupPermissionService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GroupPermission";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            await _permissionService.UpdateStatusByGroupIdAsync(request.Id);
            return RedirectToAction("Index", "GroupPermission");
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _groupPermissionService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            await _permissionService.RemoveRangeByGroupIdAsync(id_delete);
            return RedirectToAction("Index", "GroupPermission");
        }

        //Permission
        [HttpPost("Systems/Permission/Edit")]
        [AuthorizeAction("Edit", "GroupPermission", "Edit")] // per, controller, action
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPermissionByGroupId(Guid Id)
        {
            var model = await _permissionService.EditAsync(Id);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return PartialView("~/Views/Admin/Systems/GroupPermission/Components/_Edit.cshtml", model.Data);
        }

        [HttpPost("Systems/Permission/Update")]
        [AuthorizeAction("Update", "GroupPermission", "Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePermissionByGroupId(Permission request, string? TimKiem = null, int PageSize = 5, int PageCurrent = 1)
        {
            var model = await _permissionService.UpdateAsync(request);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return await GetPermissions(request.GroupPermissionId, TimKiem, PageSize, PageCurrent);
        }

        [HttpPost("Systems/Permission/LoadData")]
        [AuthorizeAction("Index", "GroupPermission", "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadDataPermissionByGroupId(Guid GroupPermissionId, string? TimKiem = null, int PageSize = 5, int PageCurrent = 1)
        {
            return await GetPermissions(GroupPermissionId, TimKiem, PageSize, PageCurrent);
        }

        private async Task<IActionResult> GetPermissions(Guid groupPermissionId, string? timKiem, int pageSize, int pageCurrent)
        {
            var model = await _permissionService.GetPermissionsByGroupIdAsync(groupPermissionId, timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = "Không tìm thấy dữ liệu!" });
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model?.TotalRecord ?? 0, timKiem ?? "", pageSize, pageCurrent);
            return PartialView("~/Views/Admin/Systems/GroupPermission/Components/_Permissions.cshtml", model?.Data ?? new List<Permission>());
        }
    }
}
