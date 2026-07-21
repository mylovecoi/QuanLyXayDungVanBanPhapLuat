using DataAccess.Entities.Manages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services.Manages;
using System.Text.RegularExpressions;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class VanBanPhapLuatController : Controller
    {
        private readonly IVanBanPhapLuatService _vanBanPhapLuatService;
        public VanBanPhapLuatController(IVanBanPhapLuatService vanBanPhapLuatService)
        {
            _vanBanPhapLuatService = vanBanPhapLuatService;
        }

        [HttpGet("Manages/VanBanPhapLuat")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            var model = await _vanBanPhapLuatService.GetVanBanPhapLuatsAsync(TimKiem, PageSize, PageCurrent, false);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/VanBanPhapLuat/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/VanBanPhapLuat/Create")]
        [AuthorizeAction("Create")]
        public IActionResult Create()
        {
            var model = new AttachedFile
            {
                NgayApDung = DateTime.Now,
                NgayBanHanh = DateTime.Now,
                TableName = "VanBanPhapLuat",
            };
            return PartialView("~/Views/Admin/Manages/VanBanPhapLuat/_FormFields.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(AttachedFile request)
        {
            var model = await _vanBanPhapLuatService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "VanBanPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "VanBanPhapLuat");
        }

        [HttpPost("Manages/VanBanPhapLuat/Edit")]
        [AuthorizeAction("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await _vanBanPhapLuatService.EditAsync(Id);
            if(model.Status == "error")
            {
                ViewData["Messages"] = model.Message;               
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("~/Views/Admin/Manages/VanBanPhapLuat/_FormFields.cshtml", model.Data);
        }

        [HttpPost]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AttachedFile request)
        {
            var model = await _vanBanPhapLuatService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "VanBanPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "VanBanPhapLuat");
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await _vanBanPhapLuatService.DeleteAsync(id_delete);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "VanBanPhapLuat";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "VanBanPhapLuat");
        }

        [HttpPost("Manages/VanBanPhapLuat/Show")]
        [AuthorizeAction("Show")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await _vanBanPhapLuatService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return PartialView("~/Views/Admin/Manages/VanBanPhapLuat/Show.cshtml", model.Data);
        }
    }
}
