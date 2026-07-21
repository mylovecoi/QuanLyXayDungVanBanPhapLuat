using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.DTOs.Manages.ThongTinHoSo.ExportData;
using Services.Manages.ThongTinHoSo;
using Services.Settings;
using Services.Settings.DanhMucDungChung.DmHopDong;
using UI.Security;

namespace UI.Controllers.Admin.Reports
{
    [Route("Reports/BaoCaoKhac")]
    [SetViewDataFilter]
    public class BaoCaoKhacController(
        IDanhMucDonViService danhMucDonViService,
        IHoSoCCCTBaoCaoService hoSoCCCTBaoCaoService,
        IDmHopDongService dmHopDongService
        ) : BaseController
    {
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IHoSoCCCTBaoCaoService _hoSoCCCTBaoCaoService = hoSoCCCTBaoCaoService;
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private string ViewPath(string viewName) => $"../Admin/Reports/BaoCaoKhac/{viewName}";

        private async Task InitDataForCreateOrUpdate(ReportRequestDto entity)
        {
            ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucHopDong"] = (await _dmHopDongService.GetListByFilterAsync(new(1000, loaiNghiepVu: null /*true: công chứng*/))).Data ?? new List<DanhMucHopDong>();
            ViewData["TinhNangThanhToan"] = await _danhMucDonViService.GetTinhNangThanhToanStatusAsync();
        }

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            await InitDataForCreateOrUpdate(new());
            return View(ViewPath(nameof(Index)));
        }

        [Authorize]
        [HttpPost(nameof(ExportZip))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportZip(ExportDataRequestDto request)
        {
            var validatResult = await _hoSoCCCTBaoCaoService.ValidateRequestAsync(request);
            string status = validatResult.Status, message = validatResult.Message;
            if (validatResult.Status == "error")
            {
                validatResult.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return ReturnJson(status == "success", message, null, validatResult.ErrorMessages);
            }

            var response = await _hoSoCCCTBaoCaoService.ExportZip(request);
            if (response.Status == "error") return ReturnJson(response.Status == "success", response.Message, null);

            if (response.Data == null) return ReturnJson(false, "Dữ liệu tệp ZIP không hợp lệ.", null, null);

            var memoryStream = (MemoryStream)response.Data;
            return new FileStreamResult(memoryStream, "application/zip")
            {
                FileDownloadName = $"HoSo_{request.NamKetXuat}_{DateTime.Now:yyyyMMddHHmmss}.zip"
            };
        }

        [HttpGet(nameof(GetFormFields))]
        [AuthorizeAction("Create", "BaoCaoKhac", "Create")]
        public async Task<IActionResult> GetFormFields()
        {
            var model = new ReportRequestDto();
            DateTime now = DateTime.Now;
            model.NgayBaoCaoTu = new DateTime(now.Year, now.Month, 1);
            await InitDataForCreateOrUpdate(model);
            return RenderValidationResult(true, "", model, ViewPath("_FormFields")); ;
        }

        [HttpPost(nameof(BaoCaoTongQuanData))]
        [AuthorizeAction(nameof(Index), "BaoCaoKhac", nameof(Index))]
        public async Task<IActionResult> BaoCaoTongQuanData(ReportRequestDto request)
        {
            ViewData["Title"] = "Báo cáo tổng quát hồ sơ";
            var responseValidate = await _hoSoCCCTBaoCaoService.ValidateRequestAsync(request);

            if (responseValidate.Status == "error")
            {
                ViewData["Message"] = responseValidate.Message;
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return View(ViewPath(nameof(BaoCaoTongQuanData)));
            }

            var response = await _hoSoCCCTBaoCaoService.GetBaoCaoThongKeTongQuatDataAsync(request);
            return View(ViewPath(nameof(BaoCaoTongQuanData)), response.Data);
        }

        [HttpPost(nameof(BaoCaoChiTietData))]
        [AuthorizeAction(nameof(Index), "BaoCaoKhac", nameof(Index))]
        public async Task<IActionResult> BaoCaoChiTietData(ReportRequestDto request)
        {
            ViewData["Title"] = "Báo cáo chi tiết hồ sơ";
            var responseValidate = await _hoSoCCCTBaoCaoService.ValidateRequestAsync(request);

            if (responseValidate.Status == "error")
            {
                ViewData["Message"] = responseValidate.Message;
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return View(ViewPath(nameof(BaoCaoChiTietData)));
            }

            var response = await _hoSoCCCTBaoCaoService.GetBaoCaoThongKeChiTietDataAsync(request);
            return View(ViewPath(nameof(BaoCaoChiTietData)), response.Data);
        }

        [HttpPost(nameof(BaoCaoChiPhiTongQuanData))]
        [AuthorizeAction(nameof(Index), "BaoCaoKhac", nameof(Index))]
        public async Task<IActionResult> BaoCaoChiPhiTongQuanData(ReportRequestDto request)
        {
            ViewData["Title"] = "Báo cáo chi phí tổng quát hồ sơ";
            var responseValidate = await _hoSoCCCTBaoCaoService.ValidateRequestAsync(request);

            if (responseValidate.Status == "error")
            {
                ViewData["Message"] = responseValidate.Message;
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return View(ViewPath(nameof(BaoCaoChiPhiTongQuanData)));
            }

            var response = await _hoSoCCCTBaoCaoService.GetBaoCaoThongKeChiPhiTongQuatDataAsync(request);
            return View(ViewPath(nameof(BaoCaoChiPhiTongQuanData)), response.Data);
        }

        [HttpPost(nameof(BaoCaoChiPhiChiTietData))]
        [AuthorizeAction(nameof(Index), "BaoCaoKhac", nameof(Index))]
        public async Task<IActionResult> BaoCaoChiPhiChiTietData(ReportRequestDto request)
        {
            ViewData["Title"] = "Báo cáo chi phí chi tiết hồ sơ";
            var responseValidate = await _hoSoCCCTBaoCaoService.ValidateRequestAsync(request);

            if (responseValidate.Status == "error")
            {
                ViewData["Message"] = responseValidate.Message;
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return View(ViewPath(nameof(BaoCaoChiPhiChiTietData)));
            }

            var response = await _hoSoCCCTBaoCaoService.GetBaoCaoThongKeChiPhiChiTietDataAsync(request);
            return View(ViewPath(nameof(BaoCaoChiPhiChiTietData)), response.Data);
        }

        [HttpGet(nameof(GetSoLuongHoSoTiepNhanTheoThang))]
        [AllowAnonymous]
        public async Task<IActionResult> GetSoLuongHoSoTiepNhanTheoThang(ReportRequestDto request)
        {
            var response = await _hoSoCCCTBaoCaoService.GetSoLuongHoSoTheoThangAsync(true);
            return ReturnJson(response.Status == "success", response.Message, response.Data);
        }


    }
}
