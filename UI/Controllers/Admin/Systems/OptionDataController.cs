using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;
using System.Threading.Tasks;
using UI.Helper;

namespace UI.Controllers.Admin.Systems
{
    public class OptionDataController : Controller
    {
        private readonly IOptionDataService _optionDataService;
        private ISession? _session => HttpContext?.Session;
        private readonly List<string> _code = new()
        {
           "NhomQuyen", "DiaDanh", "DonVi", "LoaiGiayTo", "LoaiTaiSan", "PhiLePhi"
        };
        public OptionDataController(IOptionDataService optionDataService)
        {
            _optionDataService = optionDataService;
        }

        [HttpGet("Systems/OptionData")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await _optionDataService.GetDataOptionAsync(TimKiem, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "OptionData";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            ViewData["Title"] = "Danh sách Option Data";
            return View("Views/Admin/Systems/OptionData/Index.cshtml", model.Data);
        }

        [HttpGet("Systems/OptionData/Create")]
        public IActionResult Create()
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = new OptionData();
            ViewData["Code"] = _code;
            return PartialView("Views/Admin/Systems/OptionData/_FormFields.cshtml", model);
        }

        [HttpPost("Systems/OptionData/Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(OptionData request, string dateStr)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _optionDataService.StoreAsync(request);
            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost("Systems/OptionData/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _optionDataService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "OptionData";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["Code"] = _code;
            return PartialView("Views/Admin/Systems/OptionData/_FormFields.cshtml", model.Data);
        }

        [HttpPost("Systems/OptionData/Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(OptionData request)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _optionDataService.UpdateAsync(request);
            if (model.Status == "error")
            {
                return Json(new { status = "error", message = model.Message });
            }
            return Json(new { status = "success", message = "Cập nhật thành công!" });
        }

        [HttpPost("Systems/OptionData/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                ViewData["Messages"] = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!";
                ViewData["Controller"] = "Auth";
                ViewData["Action"] = "Login";
                return View("Views/Shared/Error.cshtml");
            }
            if (!bool.Parse(FuntionGlobal.GetSsAdmin(_session, "SSA")))
            {
                ViewData["Messages"] = "Bạn đã không có quyền truy cập vào chức năng!";
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            var model = await _optionDataService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "OptionData";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "OptionData");
        }
    }
}
