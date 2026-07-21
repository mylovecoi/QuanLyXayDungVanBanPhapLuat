using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using System.Net.Http.Headers;
using System.Text;

namespace UI.Controllers.Admin
{
    public class SmartCAController : Controller
    {
        private readonly SmartCAService _smartCAService;

        public class SmartCAUser{
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class OtpConfirmRequest
        {
            public required string TranId { get; set; }
            public required string Otp { get; set; }
            public required string AccessToken { get; set; }
        }

        public SmartCAController(SmartCAService smartCAService)
        {
            _smartCAService = smartCAService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAjax([FromBody] SmartCAUser user)
        {
            var token = await _smartCAService.GetAccessTokenAsync(user.Username, user.Password);
            if (string.IsNullOrEmpty(token))
                return Json(new { success = false });

            var credentialId = await _smartCAService.GetCredentialIdAsync(token);
            if (string.IsNullOrEmpty(credentialId))
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                accessToken = token,
                credentialId = credentialId
            });
        }

        [HttpPost]
        public async Task<IActionResult> SignFileAjax(IFormFile file, string accessToken, string credentialId)
        {
            if (file == null || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(credentialId))
                return Json(new { success = false, message = "Thiếu dữ liệu" });

            var result = await _smartCAService.SignFileAsync(file, accessToken, credentialId);

            if (string.IsNullOrEmpty(result.TranId))
                return Json(new { success = false, message = "Không thể ký" });

            // Thử check lại nhiều lần trước khi bỏ cuộc
            for (int i = 0; i < 5; i++)
            {
                var tranInfo = await _smartCAService.CheckTransactionAsync(result.TranId, accessToken);

                // Nếu trả về yêu cầu thông tin OTP 
                if (tranInfo != null && (tranInfo.tranStatusDesc == "WAITING_FOR_OTP" || tranInfo.tranStatusDesc?.Contains("OTP") == true))
                {
                    return Json(new
                    {
                        success = false,
                        requireOtp = true,
                        tranId = result.TranId,
                        message = "Yêu cầu xác thực OTP để hoàn tất ký."
                    });
                }

                if (tranInfo != null && tranInfo.tranStatusDesc == "SUCCESS"
                    && tranInfo.documents?.Any() == true
                    && !string.IsNullOrEmpty(tranInfo.documents[0].dataSigned))
                {
                    var dataSigned = tranInfo.documents[0].dataSigned!;
                    var fileBytes = Convert.FromBase64String(dataSigned);
                    var fileName = "signed_" + tranInfo.documents[0].name;
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FileUpload", "signed-docs");

                    if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

                    await System.IO.File.WriteAllBytesAsync(Path.Combine(savePath, fileName), fileBytes);

                    return Json(new { success = true, message = "/FileUpload/signed-docs/" + fileName, base64 = "data:application/pdf;base64," + dataSigned });
                }

                await Task.Delay(2000); // đợi 2s trước khi check lại
            }

            return Json(new { success = false, message = "Ký chưa hoàn tất sau nhiều lần thử." });
        }

        [HttpPost]
        public async Task<IActionResult> PostOtpAndConfirm([FromBody] OtpConfirmRequest model)
        {
            var result = await _smartCAService.ConfirmOtpAsync(model.TranId, model.Otp, model.AccessToken);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }
    }
}
