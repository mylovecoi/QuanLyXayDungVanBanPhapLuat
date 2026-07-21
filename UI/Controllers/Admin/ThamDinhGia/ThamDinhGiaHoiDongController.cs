using DataAccess.Entities.ThamDinhGia;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ThamDinhGia;
using System;
using System.IO;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.ThamDinhGia
{
    [Route("ThamDinhGia/HoiDong")]
    [SetViewDataFilter]
    public class ThamDinhGiaHoiDongController(
        IThamDinhGiaHoiDongService thamDinhGiaHoiDongService,
        IThamDinhGiaHoiDongCtService thamDinhGiaHoiDongCtService
    ) : Controller
    {
        private readonly IThamDinhGiaHoiDongService _thamDinhGiaHoiDongService = thamDinhGiaHoiDongService;
        private readonly IThamDinhGiaHoiDongCtService _thamDinhGiaHoiDongCtService = thamDinhGiaHoiDongCtService;
        private string ViewPath(string viewName) => $"../Admin/ThamDinhGia/HoiDong/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index", controller: "ThamDinhGiaHoiDong", action: "Index")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _thamDinhGiaHoiDongService.GetListThamDinhGiaHoiDongAsync(timKiem, pageSize, pageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpGet("Show")]
        [AuthorizeAction("Index", controller: "ThamDinhGiaHoiDong", action: "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            var model = await _thamDinhGiaHoiDongService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDong";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var hoiDong = model.Data as ThamDinhGiaHoiDong;

            // Fetch members list
            var membersResponse = await _thamDinhGiaHoiDongCtService.GetListDanhMucCtAsync(id, "", 1000, 1);
            ViewData["Members"] = membersResponse.Status == "success" ? membersResponse.Data : new List<ThamDinhGiaHoiDongCt>();

            return View(ViewPath(nameof(Show)), hoiDong);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store([FromForm] ThamDinhGiaHoiDong request)
        {
            if (request.Ipf1Upload != null && request.Ipf1Upload.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FileUpload", "ThamDinhGia");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Ipf1Upload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Ipf1Upload.CopyToAsync(fileStream);
                }
                request.Ipf1 = uniqueFileName;
            }

            var model = await _thamDinhGiaHoiDongService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _thamDinhGiaHoiDongService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDong";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update([FromForm] ThamDinhGiaHoiDong request)
        {
            if (request.Ipf1Upload != null && request.Ipf1Upload.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FileUpload", "ThamDinhGia");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Ipf1Upload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Ipf1Upload.CopyToAsync(fileStream);
                }
                request.Ipf1 = uniqueFileName;
            }

            var model = await _thamDinhGiaHoiDongService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "ThamDinhGiaHoiDong", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _thamDinhGiaHoiDongService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "ThamDinhGiaHoiDong";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaHoiDong");
        }
    }
}
