using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.Manages.ThongTinHoSo;
using UI.Security;

namespace UI.Controllers.Admin.Reports
{
    [Route("Reports/BaoCaoTT052025TTBTP")]
    [SetViewDataFilter]
    public class BaoCaoTT052025TTBTPController(
        IHoSoCCCTBaoCaoService hoSoCCCTBaoCaoService
        ) : BaseController
    {
        private static string ViewPath(string viewName) => $"../Admin/Reports/BaoCaoTT052025TTBTP/{viewName}";
        private static string ViewPathSoCongChung(string viewName) => $"../Admin/Reports/BaoCaoTT052025TTBTP/SoCongChung/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public IActionResult Index()
        {
            return View(ViewPath(nameof(Index)));
        }

        [HttpGet(nameof(SoCongChungFormFields))]
        [AuthorizeAction("Create", "BaoCaoTT052025TTBTP", "Create")]
        public IActionResult SoCongChungFormFields()
        {
            var model = new ReportRequestDto();
            DateTime dateNow = DateTime.Now;
            model.NgayBaoCaoTu = new DateTime(dateNow.Year, 1, 1);
            model.NgayBaoCaoDen = dateNow;
            return RenderValidationResult(true, "", model, ViewPathSoCongChung("_FormFields")); ;
        }

        [HttpPost(nameof(SoYeuCauCongChung))]
        [AuthorizeAction(nameof(Index), "BaoCaoTT052025TTBTP", nameof(Index))]
        public async Task<IActionResult> SoYeuCauCongChung(ReportRequestDto request)
        {
            ViewData["Title"] = "Sổ Yêu Cầu Công Chứng";
            var responseValidate = await hoSoCCCTBaoCaoService.ValidateRequestSoCongChungAsync(request);

            if (responseValidate.Status == "error")
            {
                ViewData["Message"] = responseValidate.Message;
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return View(ViewPathSoCongChung(nameof(SoYeuCauCongChung)));
            }

            request.SetCongChung();
            var response = await hoSoCCCTBaoCaoService.GetYeuCauSoCongChungAsync(request);
            return View(ViewPathSoCongChung(nameof(SoYeuCauCongChung)), response.Data);
        }

        [HttpPost(nameof(SoCongChungGiaoDich))]
        [AuthorizeAction(nameof(Index), "BaoCaoTT052025TTBTP", nameof(Index))]
        public async Task<IActionResult> SoCongChungGiaoDich(ReportRequestDto request)
        {
            ViewData["Title"] = "Sổ Công Chứng Giao Dịch";
            var responseValidate = await hoSoCCCTBaoCaoService.ValidateRequestSoCongChungAsync(request);

            if (responseValidate.Status == "error")
            {
                ViewData["Message"] = responseValidate.Message;
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return View(ViewPathSoCongChung(nameof(SoCongChungGiaoDich)));
            }

            request.SetCongChung();
            request.SetHoSoGiay();
            var response = await hoSoCCCTBaoCaoService.GetSoCongChungGiaoDichAsync(request);
            return View(ViewPathSoCongChung(nameof(SoCongChungGiaoDich)), response.Data);
        }

        [HttpPost(nameof(SoCongChungGiaoDichDienTu))]
        [AuthorizeAction(nameof(Index), "BaoCaoTT052025TTBTP", nameof(Index))]
        public async Task<IActionResult> SoCongChungGiaoDichDienTu(ReportRequestDto request)
        {
            ViewData["Title"] = "Sổ Công Chứng Giao Dịch Điện Tử";
            var responseValidate = await hoSoCCCTBaoCaoService.ValidateRequestSoCongChungAsync(request);

            if (responseValidate.Status == "error")
            {
                ViewData["Message"] = responseValidate.Message;
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
                return View(ViewPathSoCongChung(nameof(SoCongChungGiaoDichDienTu)));
            }

            request.SetCongChung();
            request.SetHoSoDienTu();
            var response = await hoSoCCCTBaoCaoService.GetSoCongChungGiaoDichDienTuAsync(request);
            return View(ViewPathSoCongChung(nameof(SoCongChungGiaoDichDienTu)), response.Data);
        }
    }
}
