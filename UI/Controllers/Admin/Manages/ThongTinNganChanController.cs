using DataAccess.Entities.Manages;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Settings;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class ThongTinNganChanController(
       IThongTinNganChanService thongTinNganChanService,
       IAttachedFileService attachedFileService,
       IDanhMucDonViService danhMucDonViService) : Controller
    {
        [HttpGet("Manages/ThongTinNganChan")]
        [AuthorizeAction("Index")]
        public async Task<IActionResult> Index(string TimKiem = "", int PageSize = 5, int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;
            var model = await thongTinNganChanService.GetThongTinNganChansAsync(TimKiem, PageSize, PageCurrent);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent);
            return View("Views/Admin/Manages/ThongTinNganChan/Index.cshtml", model.Data);
        }

        [HttpGet("Manages/ThongTinNganChan/Create")]
        [AuthorizeAction("Create")]
        public async Task<IActionResult> Create()
        {
            await attachedFileService.RemoveDatarRedundantAsync();
            var model = new ThongTinNganChan
            {
                Id = Guid.NewGuid(),
                NgayQuyetDinh = DateTime.Now,
                NgayApDung = DateTime.Now,
                TrangThai = TrangThaiNganChan.ApDung
            };
            ViewData["FromCreate"] = true;
            return View("Views/Admin/Manages/ThongTinNganChan/Create.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAction("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(ThongTinNganChan request)
        {
            request.DonViBanHanhId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);
            await attachedFileService.UpdateRangeStatus(request.Id, "ThongTinNganChan");
            var model = await thongTinNganChanService.StoreAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index", "ThongTinNganChan");
        }

        [HttpGet("Manages/ThongTinNganChan/Edit")]
        [AuthorizeAction("Edit")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await thongTinNganChanService.EditAsync(Id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            var attachedFiles = await attachedFileService.GetAttachedFilesAsync(Id, "ThongTinNganChan", "", 5, 1);
            if (model.Data == null)
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }

            model.Data.DSHopDongDinhKem = attachedFiles?.Data ?? new List<AttachedFile>();
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(attachedFiles?.TotalRecord ?? 0, "", 5, 1);
            return View("Views/Admin/Manages/ThongTinNganChan/Edit.cshtml", model.Data);
        }

        [HttpPost]
        [AuthorizeAction("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ThongTinNganChan request)
        {
            request.DonViBanHanhId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);
            var model = await thongTinNganChanService.UpdateAsync(request);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            await attachedFileService.UpdateRangeStatus(request.Id, "ThongTinNganChan");
            return RedirectToAction("Index", "ThongTinNganChan");
        }

        [HttpPost]
        [AuthorizeAction("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id_delete)
        {
            var model = await thongTinNganChanService.DeleteAsync(id_delete);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            await attachedFileService.RemoveRangeByGroupId(id_delete);
            return RedirectToAction("Index", "ThongTinNganChan");
        }

        [HttpGet("Manages/ThongTinNganChan/Show")]
        [AuthorizeAction("Show")]
        public async Task<IActionResult> Show(Guid Id)
        {
            var model = await thongTinNganChanService.EditAsync(Id);
            var attachedFiles = await attachedFileService.GetAllAttachedFilesAsync(Id, "ThongTinNganChan");
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            if (model.Data == null)
            {
                ViewData["Messages"] = model.Message;
                return View("Views/Shared/Error.cshtml");
            }
            model.Data.DSHopDongDinhKem = attachedFiles ?? [];
            return View("Views/Admin/Manages/ThongTinNganChan/Show.cshtml", model.Data);
        }

        [HttpGet("Manages/ThongTinNganChan/Search")]
        //[AuthorizeAction("Search")]
        public async Task<IActionResult> Search(
            string SoQuyetDinh = "",
            string CoQuanBanHanh = "",
            Guid? DonViBanHanhId = null,
            int? NamQuyetDinh = null,
            int? NamApDung = null,
            TrangThaiNganChan? TrangThai = null,
            string ThongTinTaiSan = "",
            string SoQuyetDinhDung = "",
            string CoQuanDung = "",
            int? NamQuyetDinhDung = null,
            int? NamApDungDung = null,
            string TimKiem ="",
            int PageSize = 10,
            int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var model = await thongTinNganChanService.SearchThongTinNganChansAsync(
                SoQuyetDinh,
                CoQuanBanHanh,
                DonViBanHanhId,
                NamQuyetDinh,
                NamApDung,
                TrangThai,
                ThongTinTaiSan,
                SoQuyetDinhDung,
                CoQuanDung,
                NamQuyetDinhDung,
                NamApDungDung,
                TimKiem,
                PageSize,
                PageCurrent);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["DanhMucDonVi"] = await danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DonViBanHanhId"] = DonViBanHanhId;
            ViewData["SoQuyetDinh"] = SoQuyetDinh;
            ViewData["CoQuanBanHanh"] = CoQuanBanHanh;
            ViewData["NamQuyetDinh"] = NamQuyetDinh;
            ViewData["NamApDung"] = NamApDung;
            ViewData["TrangThai"] = TrangThai;
            ViewData["ThongTinTaiSan"] = ThongTinTaiSan;
            ViewData["SoQuyetDinhDung"] = SoQuyetDinhDung;
            ViewData["CoQuanDung"] = CoQuanDung;
            ViewData["NamQuyetDinhDung"] = NamQuyetDinhDung;
            ViewData["NamApDungDung"] = NamApDungDung;
            ViewData["TimKiem"] = TimKiem;

            var pageInfo = FuntionGlobal.GetPageInfo(model.TotalRecord, TimKiem, PageSize, PageCurrent, model.Data);
            ViewData["PageInfo"] = pageInfo;
            return View("Views/Admin/Manages/ThongTinNganChan/Search.cshtml", pageInfo);
        }
    }
}
