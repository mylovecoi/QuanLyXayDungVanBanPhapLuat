using Microsoft.AspNetCore.Mvc;
using Services.QuanLyDanhMuc;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.QuanLyDanhMuc
{
    [SetViewDataFilter]
    public class QuyTrinhSoanThaoController(IQuyTrinhSoanThaoService quyTrinhSoanThaoService) : Controller
    {
        private readonly IQuyTrinhSoanThaoService _quyTrinhSoanThaoService = quyTrinhSoanThaoService;

        [HttpGet("QuanLyDanhMuc/QuyTrinhSoanThao")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string timKiem = "", int pageSize = 5, int pageCurrent = 1)
        {
            pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
            pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

            var model = await _quyTrinhSoanThaoService.GetDanhSachAsync(timKiem, pageSize, pageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, timKiem, pageSize, pageCurrent);
            return View("Views/Admin/QuanLyDanhMuc/QuyTrinhSoanThao/Index.cshtml", model.Data);
        }

        [HttpGet("QuanLyDanhMuc/QuyTrinhSoanThao/Create")]
        [AuthorizeAction("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["DanhMucVanBans"] = await _quyTrinhSoanThaoService.GetDanhMucVanBanOptionsAsync();
            ViewData["DanhMucDonVis"] = await _quyTrinhSoanThaoService.GetDanhMucDonViOptionsAsync();
            ViewData["FormMode"] = "Create";
            var model = await _quyTrinhSoanThaoService.GetCreateModelAsync();
            return View("Views/Admin/QuanLyDanhMuc/QuyTrinhSoanThao/Upsert.cshtml", model);
        }

        [HttpPost("QuanLyDanhMuc/QuyTrinhSoanThao/Store")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Store")]
        public async Task<IActionResult> Store(QuyTrinhSoanThaoEditModel request)
        {
            var model = await _quyTrinhSoanThaoService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["DanhMucVanBans"] = await _quyTrinhSoanThaoService.GetDanhMucVanBanOptionsAsync();
                ViewData["DanhMucDonVis"] = await _quyTrinhSoanThaoService.GetDanhMucDonViOptionsAsync();
                ViewData["FormMode"] = "Create";
                ViewData["Messages"] = model.Message;
                return View("Views/Admin/QuanLyDanhMuc/QuyTrinhSoanThao/Upsert.cshtml", request);
            }

            return RedirectToAction("Index", "QuyTrinhSoanThao");
        }

        [HttpGet("QuanLyDanhMuc/QuyTrinhSoanThao/Edit/{id:guid}")]
        [AuthorizeAction("Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["DanhMucVanBans"] = await _quyTrinhSoanThaoService.GetDanhMucVanBanOptionsAsync();
            ViewData["DanhMucDonVis"] = await _quyTrinhSoanThaoService.GetDanhMucDonViOptionsAsync();
            ViewData["FormMode"] = "Edit";
            var model = await _quyTrinhSoanThaoService.GetEditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "QuyTrinhSoanThao";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return View("Views/Admin/QuanLyDanhMuc/QuyTrinhSoanThao/Upsert.cshtml", model.Data);
        }

        [HttpPost("QuanLyDanhMuc/QuyTrinhSoanThao/Update")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Update")]
        public async Task<IActionResult> Update(QuyTrinhSoanThaoEditModel request)
        {
            var model = await _quyTrinhSoanThaoService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["DanhMucVanBans"] = await _quyTrinhSoanThaoService.GetDanhMucVanBanOptionsAsync();
                ViewData["DanhMucDonVis"] = await _quyTrinhSoanThaoService.GetDanhMucDonViOptionsAsync();
                ViewData["FormMode"] = "Edit";
                ViewData["Messages"] = model.Message;
                return View("Views/Admin/QuanLyDanhMuc/QuyTrinhSoanThao/Upsert.cshtml", request);
            }

            return RedirectToAction("Index", "QuyTrinhSoanThao");
        }

        [HttpPost("QuanLyDanhMuc/QuyTrinhSoanThao/Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Delete")]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _quyTrinhSoanThaoService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "QuyTrinhSoanThao";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index", "QuyTrinhSoanThao");
        }
    }
}
