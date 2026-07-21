using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Settings;
using UI.Helper;

namespace UI.Controllers.Public.ThongTuQuyetDinh
{
    public class ThuTucHanhChinhController(
        IThuTucHanhChinhService thuTucHanhChinhService,
        IAttachedFileService attachedFileService) : Controller
    {
        [HttpGet("Public/ThuTucHanhChinh", Name = "thu-tuc-hanh-chinh")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await thuTucHanhChinhService.GetThuTucHanhChinhsAsync(TimKiem, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            ViewData["MenuActive"] = "public_thutuchanhchinh";
            ViewData["Title"] = "Thông tin thủ tục hành chính";
            return View("Views/Public/ThongTuQuyetDinh/ThuTucHanhChinh/Index.cshtml", model.Data);
        }

        [HttpGet("Public/ThuTucHanhChinh/Show")]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await thuTucHanhChinhService.EditAsync(Id);
            if (model.Data == null || model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            var attachedFiles = await attachedFileService.GetAllAttachedFilesAsync(Id, "ThuTucHanhChinh");
            model.Data!.DSFileDinhKem = attachedFiles ?? [];
            return View("Views/Public/ThongTuQuyetDinh/ThuTucHanhChinh/Show.cshtml", model.Data);
        }
    }
}
