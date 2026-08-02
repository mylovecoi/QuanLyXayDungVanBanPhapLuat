using Microsoft.AspNetCore.Mvc;
using DataAccess.Entities.QuanLyDanhMuc;
using Services.QuanLyDanhMuc;
using UI.Helper;
using UI.ViewModels;

namespace UI.Controllers.Admin.QuanLyDanhMuc
{
    public class DanhMucTieuChiDiemController(IDanhMucTieuChiDiemService danhMucTieuChiDiemService) : Controller
    {
        private readonly IDanhMucTieuChiDiemService _danhMucTieuChiDiemService = danhMucTieuChiDiemService;

        [HttpGet("QuanLyDanhMuc/DanhMucTieuChiDiem")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            await _danhMucTieuChiDiemService.EnsureDefaultDataAsync();
            var model = await _danhMucTieuChiDiemService.GetDanhSachAsync(timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Danh mục tiêu chí chấm điểm";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);
            return View("Views/Admin/QuanLyDanhMuc/DanhMucTieuChiDiem/Index.cshtml", model.Data);
        }

        [HttpGet("QuanLyDanhMuc/DanhMucTieuChiDiem/Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Danh mục tiêu chí chấm điểm";
            return PartialView(
                "Views/Admin/QuanLyDanhMuc/DanhMucTieuChiDiem/_FormFields.cshtml",
                new DanhMucTieuChiDiemUpsertViewModel
                {
                    TieuChi = new DanhMucTieuChiDiem
                    {
                        ThuTuSapXep = await _danhMucTieuChiDiemService.GetNextThuTuSapXepAsync(),
                        TrangThai = true,
                        LoaiTieuChi = "THOI_GIAN",
                        KieuGiaTri = "TY_LE",
                        DonViGiaTri = "PERCENT"
                    },
                    Mucs = new List<DanhMucTieuChiDiemMuc>
                    {
                        new() { ThuTuSapXep = 1, TrangThai = true }
                    }
                });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTieuChiDiem/Store")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Store(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc> mucs)
        {
            var model = await _danhMucTieuChiDiemService.StoreAsync(request, mucs);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTieuChiDiem/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["Title"] = "Danh mục tiêu chí chấm điểm";
            var model = await _danhMucTieuChiDiemService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucTieuChiDiem";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var vm = new DanhMucTieuChiDiemUpsertViewModel
            {
                TieuChi = model.Data.TieuChi,
                Mucs = model.Data.Mucs
            };
            return PartialView("Views/Admin/QuanLyDanhMuc/DanhMucTieuChiDiem/_FormFields.cshtml", vm);
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTieuChiDiem/Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucTieuChiDiem request, List<DanhMucTieuChiDiemMuc> mucs)
        {
            var model = await _danhMucTieuChiDiemService.UpdateAsync(request, mucs);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("QuanLyDanhMuc/DanhMucTieuChiDiem/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucTieuChiDiemService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucTieuChiDiem";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "DanhMucTieuChiDiem");
        }
    }
}
