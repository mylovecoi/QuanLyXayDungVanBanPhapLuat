using DataAccess.Entities.Manages;
using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using UI.Helper;

namespace UI.Controllers.Admin.Manages
{
    public class AttachedFileController : Controller
    {
        private readonly IAttachedFileService _attachedFileService;
        private ISession? _session => HttpContext?.Session;

        public AttachedFileController(IAttachedFileService attachedFileService)
        {
            _attachedFileService = attachedFileService;
        }

        public async Task<IActionResult> GetAttachedFiles(Guid GroupId, string TableName, string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            var model = await _attachedFileService.GetAttachedFilesAsync(GroupId, TableName, TimKiem, PageSize, PageCurrent);
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return PartialView("~/Views/Admin/AttachedFile/Index.cshtml", model.Data as List<AttachedFile> ?? new List<AttachedFile>());
        }

        [HttpGet("Manages/AttachedFile/GetAttachedFilesReadonly")]
        public async Task<IActionResult> GetAttachedFilesReadonly(Guid GroupId, string TableName)
        {
            var files = await _attachedFileService.GetAllAttachedFilesAsync(GroupId, TableName);
            return PartialView("~/Views/Admin/AttachedFile/ReadonlyList.cshtml", files ?? new List<AttachedFile>());
        }

        [HttpPost("Manages/AttachedFile/Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(AttachedFile request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _attachedFileService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return await GetAttachedFiles(request.GroupId, request.TableName ?? string.Empty);
        }

        [HttpPost("Manages/AttachedFile/LoadData")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadData(Guid GroupId, string TableName, string TimKiem, int PageSize, int PageCurrent)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            return await GetAttachedFiles(GroupId, TableName, TimKiem, PageSize, PageCurrent);
        }

        [HttpPost("Manages/AttachedFile/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var data = await _attachedFileService.EditAsync(Id);
            return Ok(data);
        }

        [HttpGet("Manages/Download/AttachedFile")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var data = await _attachedFileService.EditAsync(id);

            if (data == null || data.FileContent == null || string.IsNullOrEmpty(data.ContentType) || string.IsNullOrEmpty(data.FileName))
            {
                ViewData["Messages"] = "Không tìm thấy dữ liệu file đính kèm";
                return View("Views/Shared/Error.cshtml");
            }

            if (!data.Public && string.IsNullOrEmpty(_session?.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                return View("Views/Shared/Error.cshtml");
            }

            return File(data.FileContent, data.ContentType, data.FileName);
        }


        [HttpPost("Manages/AttachedFile/Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AttachedFile request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _attachedFileService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return await GetAttachedFiles(request.GroupId, request.TableName ?? string.Empty);
        }

        [HttpPost("Manages/AttachedFile/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid Id, Guid GroupId, string TableName)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _attachedFileService.DeleteAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return await GetAttachedFiles(GroupId, TableName);
        }
    }
}
