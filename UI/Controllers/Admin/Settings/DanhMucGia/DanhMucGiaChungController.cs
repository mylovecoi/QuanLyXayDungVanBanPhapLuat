using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Settings.DanhMucGia;
using System;
using System.Linq;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucGia
{
    [Route("Settings/DanhMucGia/DanhMucGiaChung")]
    [SetViewDataFilter]
    public class DanhMucGiaChungController(
        IDanhMucGiaChungService danhMucGiaChungService,
        IDanhMucGiaChungCtService danhMucGiaChungCtService,
        ApplicationDbContext dbContext
    ) : BaseController
    {
        private readonly IDanhMucGiaChungService _danhMucGiaChungService = danhMucGiaChungService;
        private readonly IDanhMucGiaChungCtService _danhMucGiaChungCtService = danhMucGiaChungCtService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/Settings/DanhMucGia/DanhMucGiaChung/{viewName}";

        [HttpGet]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string timKiem = "", string maNghe = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _danhMucGiaChungService.GetListDanhMucGiaChungAsync(timKiem, maNghe, pageSize, pageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            // Get business lists for select inputs
            var listKinhDoanhNganh = await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level == 0 || t.PhanLoai == "Group") && t.LoaiGia == "DG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();

            var listKinhDoanhNghe = await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "DG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();

            listKinhDoanhNghe = listKinhDoanhNghe.Where(t =>
            {
                var maNghe = t.MaNghe;
                if (string.IsNullOrEmpty(maNghe)) return true;

                var type = Type.GetType($"DataAccess.Entities.Settings.DanhMucGia.DanhMuc{maNghe}, DataAccess");
                var detType = Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTiet{maNghe}, DataAccess");
                return type == null || detType == null;
            }).ToList();

            ViewData["DanhMucKinhDoanhNganh"] = listKinhDoanhNganh;
            ViewData["DanhMucKinhDoanhNghe"] = listKinhDoanhNghe;
            ViewData["MaNghe"] = maNghe;
            ViewData["Title"] = "Danh mục giá chung";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);

            return View(ViewPath(nameof(Index)), model.Data);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucGiaChung request)
        {
            var model = await _danhMucGiaChungService.StoreAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _danhMucGiaChungService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaChung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var listKinhDoanhNganh = await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level == 0 || t.PhanLoai == "Group") && t.LoaiGia == "DG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();

            var listKinhDoanhNghe = await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "DG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();

            listKinhDoanhNghe = listKinhDoanhNghe.Where(t =>
            {
                var maNghe = t.MaNghe;
                if (string.IsNullOrEmpty(maNghe)) return true;

                var type = Type.GetType($"DataAccess.Entities.Settings.DanhMucGia.DanhMuc{maNghe}, DataAccess");
                var detType = Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTiet{maNghe}, DataAccess");
                return type == null || detType == null;
            }).ToList();

            ViewData["DanhMucKinhDoanhNganh"] = listKinhDoanhNganh;
            ViewData["DanhMucKinhDoanhNghe"] = listKinhDoanhNghe;

            return PartialView(ViewPath("_FormFields"), model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucGiaChung request)
        {
            var model = await _danhMucGiaChungService.UpdateAsync(request);
            return Json(new { status = model.Status, message = model.Message });
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _danhMucGiaChungService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaChung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucGiaChung");
        }

        [HttpGet("Show")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            var model = await _danhMucGiaChungService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DanhMucGiaChung";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            var data = model.Data as DanhMucGiaChung;
            ViewData["Title"] = "Chi tiết danh mục giá chung";

            // Fetch details list
            var detailsResponse = await _danhMucGiaChungCtService.GetListDanhMucCtAsync(id, "", 1000, 1);
            ViewData["Details"] = detailsResponse.Status == "success" ? detailsResponse.Data : new List<DanhMucGiaChungCt>();

            return View(ViewPath(nameof(Show)), data);
        }
    }
}
