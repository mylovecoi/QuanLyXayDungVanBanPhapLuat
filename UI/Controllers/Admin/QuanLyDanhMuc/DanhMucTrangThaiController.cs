using DataAccess.Entities.QuanLyDanhMuc;
using Microsoft.AspNetCore.Mvc;
using Services.QuanLyDanhMuc;
using UI.Helper;

namespace UI.Controllers.Admin.QuanLyDanhMuc
{
    public class DanhMucTrangThaiController(IDanhMucTrangThaiService danhMucTrangThaiService) : Controller
    {
        private readonly IDanhMucTrangThaiService _danhMucTrangThaiService = danhMucTrangThaiService;

        [HttpGet("QuanLyDanhMuc/DanhMucTrangThai")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _danhMucTrangThaiService.GetDanhMucTrangThaisAsync(timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Danh muc trang thai";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);
            return View("Views/Admin/QuanLyDanhMuc/DanhMucTrangThai/Index.cshtml", model.Data);
        }

        [HttpGet("QuanLyDanhMuc/DanhMucTrangThai/Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Danh muc trang thai";
            return PartialView(
                "Views/Admin/QuanLyDanhMuc/DanhMucTrangThai/_FormFields.cshtml",
                new DanhMucTrangThai
                {
                    ThuTuSapXep = await _danhMucTrangThaiService.GetNextThuTuSapXepAsync(),
                    TrangThai = true,
                    MaMauHex = "#28A745"
                });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTrangThai/Store")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Store(DanhMucTrangThai request)
        {
            var model = await _danhMucTrangThaiService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTrangThai/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["Title"] = "Danh muc trang thai";
            var model = await _danhMucTrangThaiService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucTrangThai";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/QuanLyDanhMuc/DanhMucTrangThai/_FormFields.cshtml", model.Data);
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTrangThai/Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucTrangThai request)
        {
            var model = await _danhMucTrangThaiService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTrangThai/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucTrangThaiService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucTrangThai";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "DanhMucTrangThai");
        }
    }
}
