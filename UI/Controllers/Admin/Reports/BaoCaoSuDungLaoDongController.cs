using Microsoft.AspNetCore.Mvc;
using Services.DTOs.BaoCaoKhac;
using Services.Settings;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Reports
{
    [Route("Reports/BaoCaoTT052025TTBTP/BaoCaoSuDungLaoDong")]
    [SetViewDataFilter]
    public class BaoCaoSuDungLaoDongController(
        IDanhMucCanBoService danhMucCanBoService,
        IDanhMucDonViService donViService) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Index(BaoCaoSuDungLaoDongRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("Views/Admin/Reports/BaoCaoTT052025TTBTP/BaoCaoSuDungLaoDong/Index.cshtml", null);
            }

            // Lấy current user ID từ session sử dụng FuntionGlobal
            var currentUserId = FuntionGlobal.GetSsAdminId(HttpContext.Session);
            if (currentUserId == Guid.Empty)
            {
                ViewData["ErrorMessage"] = "Không thể xác định người dùng hiện tại";
                return View("Views/Admin/Reports/BaoCaoTT052025TTBTP/BaoCaoSuDungLaoDong/Index.cshtml", null);
            }

            var response = await danhMucCanBoService.GetBaoCaoSuDungLaoDongAsync(request, currentUserId);

            if (response.Status == "error")
            {
                ViewData["ErrorMessage"] = response.Message;
                return View("Views/Admin/Reports/BaoCaoTT052025TTBTP/BaoCaoSuDungLaoDong/Index.cshtml", null);
            }

            // Truyền dữ liệu request cho JavaScript để sử dụng khi download
            ViewBag.RequestData = request;

            return View("Views/Admin/Reports/BaoCaoTT052025TTBTP/BaoCaoSuDungLaoDong/Index.cshtml", response.Data);
        }

        [HttpPost(nameof(GetFormFields))]
        public async Task<IActionResult> GetFormFields()
        {
            var model = new BaoCaoSuDungLaoDongRequest();
            await InitDataForFormFields(model);
            return RenderValidationResult(true, "", model, "Views/Admin/Reports/BaoCaoTT052025TTBTP/BaoCaoSuDungLaoDong/_FormFields.cshtml");
        }

        private async Task InitDataForFormFields(BaoCaoSuDungLaoDongRequest model)
        {
            // Lấy thông tin user hiện tại và đơn vị mặc định từ session
            var currentUserId = FuntionGlobal.GetSsAdminId(HttpContext.Session);
            var currentDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);

            if (currentUserId == Guid.Empty || currentDonViId == Guid.Empty)
            {
                throw new Exception("Không thể xác định người dùng hiện tại hoặc đơn vị");
            }

            // Chỉ set đơn vị mặc định và quyển số mẫu
            model.DonViId = currentDonViId;
            model.QuyenSo = $"001/{DateTime.Now.Year}-SDLĐ";

            // Load danh sách đơn vị cho dropdown
            var donViResponse = await donViService.GetDanhMucDonViAsync("", 1000, 1);
            if (donViResponse.Status == "success")
            {
                ViewData["DanhMucDonVi"] = donViResponse.Data;
            }
        }

        [HttpPost("DownloadReport")]
        public async Task<IActionResult> DownloadReport([FromBody] BaoCaoSuDungLaoDongRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Lấy current user ID từ session
                var currentUserId = FuntionGlobal.GetSsAdminId(HttpContext.Session);
                if (currentUserId == Guid.Empty)
                {
                    return Json(new { success = false, message = "Không thể xác định người dùng hiện tại" });
                }

                // Đường dẫn đến file mẫu báo cáo
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports", "TP-CC-36.docx");

                if (!System.IO.File.Exists(templatePath))
                {
                    return Json(new { success = false, message = "Không tìm thấy file mẫu báo cáo" });
                }

                // Gọi service để xuất báo cáo
                var response = await danhMucCanBoService.ExportBaoCaoSuDungLaoDongToWordAsync(request, currentUserId, templatePath);

                if (response.Status != "success" || response.Data == null)
                {
                    return Json(new { success = false, message = response.Message ?? "Có lỗi xảy ra khi tạo báo cáo" });
                }

                if (response.Data is not byte[] fileBytes)
                {
                    return Json(new { success = false, message = "Dữ liệu file không hợp lệ" });
                }

                var fileName = $"BaoCao_SuDungLaoDong_{DateTime.Now:yyyyMMdd_HHmmss}.docx";

                // Trả về file để download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }
    }
}
