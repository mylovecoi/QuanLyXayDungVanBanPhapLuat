using DataAccess.Entities.Manages;
using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Settings;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucDungChung
{
    [SetViewDataFilter]
    public class DanhMucThuTucController(
       IThuTucHanhChinhService thuTucHanhChinhService,
       IAttachedFileService attachedFileService) : Controller
    {
        [HttpGet("Settings/DanhMucDungChung/ThuTuc")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await thuTucHanhChinhService.GetThuTucHanhChinhsAsync(TimKiem, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Settings/DanhMucDungChung/DanhMucThuTuc/Index.cshtml", model.Data);
        }

        [HttpGet("Settings/DanhMucDungChung/ThuTuc/Create")]
        [AuthorizeAction("Create")]
        public async Task<IActionResult> Create()
        {
            await attachedFileService.RemoveDatarRedundantAsync();
            var model = new ThuTucHanhChinh
            {
                Id = Guid.NewGuid(),
                NgayQuyetDinh = DateTime.Now
            };
            return View("Views/Admin/Settings/DanhMucDungChung/DanhMucThuTuc/Create.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(ThuTucHanhChinh request)
        {
            await attachedFileService.UpdateRangeStatus(request.Id, "ThuTucHanhChinh");
            var model = await thuTucHanhChinhService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucThuTuc");
        }

        [HttpGet("Settings/DanhMucDungChung/ThuTuc/Edit")]
        [AuthorizeAction("Edit")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await thuTucHanhChinhService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            var attachedFiles = await attachedFileService.GetAttachedFilesAsync(Id, "ThuTucHanhChinh", "", 5, 1);
            if (model.Data == null)
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            model.Data.DSFileDinhKem = attachedFiles?.Data ?? new List<AttachedFile>();
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(attachedFiles?.TotalRecord ?? 0, "", 5, 1);
            return View("Views/Admin/Settings/DanhMucDungChung/DanhMucThuTuc/Edit.cshtml", model.Data);
        }

        [HttpPost]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ThuTucHanhChinh request)
        {
            var model = await thuTucHanhChinhService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            await attachedFileService.UpdateRangeStatus(request.Id, "ThuTucHanhChinh");
            return RedirectToAction("Index", "DanhMucThuTuc");
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await thuTucHanhChinhService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            await attachedFileService.RemoveRangeByGroupId(id_delete);
            return RedirectToAction("Index", "DanhMucThuTuc");
        }

        [HttpGet("Settings/DanhMucDungChung/ThuTuc/Show")]
        [AuthorizeAction("Show")]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await thuTucHanhChinhService.EditAsync(Id);
            var attachedFiles = await attachedFileService.GetAllAttachedFilesAsync(Id, "ThuTucHanhChinh");
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
            model.Data.DSFileDinhKem = attachedFiles ?? [];
            return View("Views/Admin/Settings/DanhMucDungChung/DanhMucThuTuc/Show.cshtml", model.Data);
        }
    }
}
