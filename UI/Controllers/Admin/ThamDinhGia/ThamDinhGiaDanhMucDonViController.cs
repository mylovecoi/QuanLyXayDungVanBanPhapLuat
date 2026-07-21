using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities.ThamDinhGia;
using Microsoft.AspNetCore.Mvc;
using Services.ThamDinhGia;
using UI.Helper;
using UI.Security;
using UI.ViewModels;

namespace UI.Controllers.Admin.ThamDinhGia
{
    [Route("ThamDinhGia/DanhMucDonVi")]
    [SetViewDataFilter]
    public class ThamDinhGiaDanhMucDonViController(
        IThamDinhGiaDanhMucDonViService thamDinhGiaDanhMucDonViService) : Controller
    {
        [HttpGet]
        [AuthorizeAction("Index", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        public async Task<IActionResult> Index(string Search = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var model = await thamDinhGiaDanhMucDonViService.GetDanhMucDonViAsync(Search, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, Search, PageSize, PageCurrent);
            ViewData["PageCurrent"] = PageCurrent;

            return View("Views/Admin/ThamDinhGia/DanhMucDonVi/Index.cshtml", model.Data);
        }

        [HttpGet("Create")]
        [AuthorizeAction("Create", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        public IActionResult Create()
        {
            var model = new ThamDinhGiaDanhMucDonVi
            {
                Id = Guid.NewGuid(),
                NgayCap = DateTime.Today,
                NgayQd = DateTime.Today
            };
            return PartialView("~/Views/Admin/ThamDinhGia/DanhMucDonVi/_FormFields.cshtml", model);
        }

        [HttpPost("Store")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(ThamDinhGiaDanhMucDonVi request)
        {
            var model = await thamDinhGiaDanhMucDonViService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaDanhMucDonVi");
        }

        [HttpPost("Edit")]
        [AuthorizeAction("Edit", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await thamDinhGiaDanhMucDonViService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            if (model.Data == null)
            {
                ViewData["Messages"] = "Không tìm thấy dữ liệu!";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("~/Views/Admin/ThamDinhGia/DanhMucDonVi/_FormFields.cshtml", model.Data);
        }

        [HttpPost("Update")]
        [AuthorizeAction("Update", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ThamDinhGiaDanhMucDonVi request)
        {
            var model = await thamDinhGiaDanhMucDonViService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaDanhMucDonVi");
        }

        [HttpPost("Delete")]
        [AuthorizeAction("Delete", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await thamDinhGiaDanhMucDonViService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThamDinhGiaDanhMucDonVi");
        }

        [HttpPost("Show")]
        [AuthorizeAction("Show", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await thamDinhGiaDanhMucDonViService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            if (model.Data == null)
            {
                ViewData["Messages"] = "Không tìm thấy dữ liệu!";
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("~/Views/Admin/ThamDinhGia/DanhMucDonVi/Show.cshtml", model.Data);
        }

        [HttpGet("Print")]
        [AuthorizeAction("Index", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        public async Task<IActionResult> Print(string Search = "")
        {
            var model = await thamDinhGiaDanhMucDonViService.GetDanhMucDonViAsync(Search, 10000, 1);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["Search"] = Search;

            return View("Views/Admin/ThamDinhGia/DanhMucDonVi/Print.cshtml", model.Data);
        }

        [HttpGet("NhanExcel")]
        [AuthorizeAction("Create", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        public IActionResult NhanExcel()
        {
            return View("Views/Admin/ThamDinhGia/DanhMucDonVi/NhanExcel.cshtml");
        }

        [HttpPost("ImportExcel")]
        [AuthorizeAction("Store", controller: "ThamDinhGiaDanhMucDonVi", action: "Index")]
        public async Task<IActionResult> ImportExcel([FromBody] List<ThamDinhGiaDanhMucDonVi> items)
        {
            if (items == null || items.Count == 0)
            {
                return Json(new { status = "error", message = "Không nhận được dữ liệu Excel!" });
            }
            var response = await thamDinhGiaDanhMucDonViService.StoreRangeAsync(items);
            return Json(new { status = response.Status, message = response.Message });
        }
    }
}
