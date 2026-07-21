using Microsoft.AspNetCore.Mvc;
using Services.Settings.DanhMucGia;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using UI.Helper;
using UI.Security;
using System;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace UI.Controllers.Admin.Settings.DanhMucGia
{
    [SetViewDataFilter]
    public class DanhMucKinhDoanhController : Controller
    {
        private readonly IDmKinhDoanhService _dmKinhDoanhService;
        private readonly ApplicationDbContext _dbContext;

        public DanhMucKinhDoanhController(IDmKinhDoanhService dmKinhDoanhService, ApplicationDbContext dbContext)
        {
            _dmKinhDoanhService = dmKinhDoanhService;
            _dbContext = dbContext;
        }

        [HttpGet("Settings/DanhMucGia/DanhMucKinhDoanh")]
        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new Services.DTOs.Settings.DanhMucDungChung.DanhMucKinhDoanhFilter(Request);
            var response = await _dmKinhDoanhService.GetListByFilterAsync(filter);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["SelectedLoaiGia"] = filter.LoaiGia;
            ViewData["Filter"] = filter;

            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, (List<DanhMucKinhDoanh>)response.Data);
            return View("Views/Admin/Settings/DanhMucGia/DanhMucKinhDoanh/Index.cshtml", pageInfo);
        }

        [HttpPost("Settings/DanhMucGia/DanhMucKinhDoanh/Create")]
        [AuthorizeAction(nameof(Create))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid Id, string LoaiGia)
        {
            var response = await _dmKinhDoanhService.CreateAsync(Id, LoaiGia);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "DanhMucKinhDoanh";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DonVis"] = await _dbContext.DanhMucDonVis.OrderBy(t => t.STTSapXep).ToListAsync();
            return PartialView("Views/Admin/Settings/DanhMucGia/DanhMucKinhDoanh/_FormFields.cshtml", response.Data);
        }

        [HttpPost("Settings/DanhMucGia/DanhMucKinhDoanh/Store")]
        [AuthorizeAction(nameof(Create))]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Store(DanhMucKinhDoanh request, string[] DonViQuanLyList, string[] DonViDongChuyenList)
        {
            var response = await _dmKinhDoanhService.StoreAsync(request, DonViQuanLyList, DonViDongChuyenList);
            return Json(new { status = response.Status, message = response.Message });
        }

        [HttpPost("Settings/DanhMucGia/DanhMucKinhDoanh/Edit")]
        [AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _dmKinhDoanhService.EditAsync(id);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "DanhMucKinhDoanh";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DonVis"] = await _dbContext.DanhMucDonVis.OrderBy(t => t.STTSapXep).ToListAsync();
            return PartialView("Views/Admin/Settings/DanhMucGia/DanhMucKinhDoanh/_FormFields.cshtml", response.Data);
        }

        [HttpPost("Settings/DanhMucGia/DanhMucKinhDoanh/Update")]
        [AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DanhMucKinhDoanh request, string[] DonViQuanLyList, string[] DonViDongChuyenList)
        {
            var response = await _dmKinhDoanhService.UpdateAsync(request, DonViQuanLyList, DonViDongChuyenList);
            return Json(new { status = response.Status, message = response.Message });
        }

        [HttpPost("Settings/DanhMucGia/DanhMucKinhDoanh/Delete")]
        [AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var response = await _dmKinhDoanhService.DeleteAsync(id_delete);
            if (response.Status == "error")
            {
                ViewData["Messages"] = response.Message;
                ViewData["Controller"] = "DanhMucKinhDoanh";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "DanhMucKinhDoanh");
        }
    }
}
