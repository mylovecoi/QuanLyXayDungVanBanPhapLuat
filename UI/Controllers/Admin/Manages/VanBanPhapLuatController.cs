using DataAccess.Entities.Manages;
using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class VanBanPhapLuatController(
        IVanBanPhapLuatService vanBanPhapLuatService,
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IVanBanPhapLuatService _vanBanPhapLuatService = vanBanPhapLuatService;
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/VanBanPhapLuat")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", Guid? DonViId = null, int PageSize = 5, int PageCurrent = 1)
        {
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;

            var selectedDonViId = await ApplyDonViFilterViewDataAsync(DonViId);
            var model = await _vanBanPhapLuatService.GetVanBanPhapLuatsAsync(TimKiem, selectedDonViId, PageSize, PageCurrent, false);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Van ban phap luat";
            ViewData["Role"] = "Manages.VanBanPhapLuat";
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/VanBanPhapLuat/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/VanBanPhapLuat/Create")]
        [AuthorizeAction("Create")]
        public async Task<IActionResult> Create()
        {
            await PopulateDonViFormViewDataAsync(null);
            var model = new AttachedFile
            {
                NgayApDung = DateTime.Now,
                NgayBanHanh = DateTime.Now,
                TableName = "VanBanPhapLuat"
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
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _vanBanPhapLuatService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            await PopulateDonViFormViewDataAsync(model.Data as AttachedFile);
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
        public async Task<IActionResult> Show(Guid id)
        {
            var model = await _vanBanPhapLuatService.EditAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("~/Views/Admin/Manages/VanBanPhapLuat/Show.cshtml", model.Data);
        }

        private async Task<Guid?> ApplyDonViFilterViewDataAsync(Guid? donViId)
        {
            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var sessionDonViId = currentUser?.DanhMucDonViId ?? Guid.Empty;
            var selectedDonViId = isSSA
                ? (donViId.HasValue && donViId.Value != Guid.Empty ? donViId : null)
                : (sessionDonViId != Guid.Empty ? sessionDonViId : null);

            var donViOptions = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            if (!isSSA)
            {
                donViOptions = donViOptions.Where(x => x.Id == sessionDonViId).ToList();
            }

            var selectedDonViName = selectedDonViId.HasValue && selectedDonViId.Value != Guid.Empty
                ? donViOptions.FirstOrDefault(x => x.Id == selectedDonViId.Value)?.TenDonVi
                : null;

            ViewData["DonViOptions"] = donViOptions;
            ViewData["SelectedDonViId"] = selectedDonViId;
            ViewData["SelectedDonViName"] = selectedDonViName;
            ViewData["IsSSA"] = isSSA;
            return selectedDonViId;
        }

        private async Task PopulateDonViFormViewDataAsync(AttachedFile? model)
        {
            var currentUser = _authService.GetUserInfo();
            var isSSA = currentUser?.SSA ?? false;
            var sessionDonViId = currentUser?.DanhMucDonViId ?? Guid.Empty;
            var donViOptions = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            var selectedDonViId = model?.DonViId ?? (sessionDonViId != Guid.Empty ? sessionDonViId : (Guid?)null);

            if (!isSSA)
            {
                donViOptions = donViOptions.Where(x => x.Id == sessionDonViId).ToList();
                selectedDonViId = sessionDonViId != Guid.Empty ? sessionDonViId : selectedDonViId;
            }

            ViewData["FormDonViOptions"] = donViOptions;
            ViewData["FormSelectedDonViId"] = selectedDonViId;
            ViewData["FormIsSSA"] = isSSA;
        }
    }
}
