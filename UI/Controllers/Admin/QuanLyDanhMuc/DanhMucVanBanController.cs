using DataAccess.Entities.QuanLyDanhMuc;
using Microsoft.AspNetCore.Mvc;
using Services.QuanLyDanhMuc;
using UI.Helper;

namespace UI.Controllers.Admin.QuanLyDanhMuc
{
    public class DanhMucVanBanController(IDanhMucVanBanService danhMucVanBanService) : Controller
    {
        private readonly IDanhMucVanBanService _danhMucVanBanService = danhMucVanBanService;

        [HttpGet("QuanLyDanhMuc/DanhMucVanBan")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _danhMucVanBanService.GetDanhMucVanBansAsync(timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Danh mục văn bản";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);
            return View("Views/Admin/QuanLyDanhMuc/DanhMucVanBan/Index.cshtml", model.Data);
        }

        [HttpGet("QuanLyDanhMuc/DanhMucVanBan/Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Danh mục văn bản";
            return PartialView(
                "Views/Admin/QuanLyDanhMuc/DanhMucVanBan/_FormFields.cshtml",
                new DanhMucVanBan
                {
                    ThuTuSapXep = 1,
                    TrangThai = true
                });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucVanBan/Store")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Store(DanhMucVanBan request)
        {
            var model = await _danhMucVanBanService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucVanBan/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["Title"] = "Danh mục văn bản";
            var model = await _danhMucVanBanService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/QuanLyDanhMuc/DanhMucVanBan/_FormFields.cshtml", model.Data);
        }

        [HttpPost("QuanLyDanhMuc/DanhMucVanBan/Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucVanBan request)
        {
            var model = await _danhMucVanBanService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucVanBan/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucVanBanService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "DanhMucVanBan");
        }
    }
}
