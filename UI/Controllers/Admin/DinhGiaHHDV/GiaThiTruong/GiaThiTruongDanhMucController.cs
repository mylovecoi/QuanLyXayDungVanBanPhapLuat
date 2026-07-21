using DataAccess.Entities.DinhGiaHHDV;
using Microsoft.AspNetCore.Mvc;
using Services.DinhGiaHHDV.GiaThiTruong;
using System;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.DinhGiaHHDV.GiaThiTruong
{
    [Route("GiaThiTruong/DanhMuc")]
    [SetViewDataFilter]
    public class GiaThiTruongDanhMucController(IGiaThiTruongDanhMucService giaThiTruongDanhMucService) : BaseController
    {
        private readonly IGiaThiTruongDanhMucService _giaThiTruongDanhMucService = giaThiTruongDanhMucService;
        private string ViewPath(string viewName) => $"../Admin/DinhGiaHHDV/GiaThiTruong/DanhMuc/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _giaThiTruongDanhMucService.GetListGiaThiTruongDanhMucAsync(timKiem, pageSize, pageCurrent);

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

        [HttpPost("Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(GiaThiTruongDanhMuc request)
        {
            var model = await _giaThiTruongDanhMucService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _giaThiTruongDanhMucService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMuc";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(GiaThiTruongDanhMuc request)
        {
            var model = await _giaThiTruongDanhMucService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _giaThiTruongDanhMucService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "GiaThiTruongDanhMuc";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "GiaThiTruongDanhMuc");
        }
    }
}
