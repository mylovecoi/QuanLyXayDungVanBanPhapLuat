using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using System.Text;
using System.Text.Json;

namespace UI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoMoPaymentController(
        IMoMoPaymentService moMoPaymentService,
        IBackgroundTaskQueue taskQueue,
        ILogger<MoMoPaymentController> logger) : ControllerBase
    {
        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment([FromForm] Guid hoSoId, [FromForm] string redirectUrl)
        {
            var result = await moMoPaymentService.CreatePaymentAsync(hoSoId, redirectUrl);
            return Ok(result);
        }

        [HttpPost("ipn")]
        public async Task<IActionResult> IpnCallback([FromBody] JsonElement requestElement)
        {
            try
            {
                var rawJson = requestElement.ToString();
                logger.LogInformation("MoMo IPN nhận được: {NewLine}{Request}", Environment.NewLine, Services.Helper.FormatJson(rawJson));          
                var request = JsonSerializer.Deserialize<MoMoIpnRequest>(rawJson, MoMoPaymentService.serializerOptions);

                // Xác thực chữ ký từ MoMo - quan trọng để đảm bảo an toàn
                if (!moMoPaymentService.IsValidSignature(request))
                {
                    logger.LogWarning("Nhận được chữ ký không hợp lệ từ MoMo IPN");
                    return BadRequest();
                }

                // Tạo tên công việc từ OrderId để theo dõi dễ dàng
                string jobName = $"MOMO_IPN_PROCESS_{request!.OrderId}";

                // Thêm task vào queue để xử lý bất đồng bộ
                await taskQueue.QueueBackgroundWorkItemAsync(async (scopedProvider, token) =>
                {
                    var scopedMomoService = scopedProvider.GetRequiredService<IMoMoPaymentService>();
                    await scopedMomoService.ProcessIpnCallbackAsync(request!);      
                }, jobName);

                // Trả về ngay lập tức theo đúng yêu cầu của MoMo
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi xử lý IPN callback: {Error}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("redirect")]
        public IActionResult PaymentRedirect(
            [FromQuery] string? partnerCode,
            [FromQuery] string? orderId,
            [FromQuery] string? requestId,
            [FromQuery] long? amount,
            [FromQuery] string? orderInfo,
            [FromQuery] string? orderType,
            [FromQuery] long? transId,
            [FromQuery] int? resultCode,
            [FromQuery] string? message,
            [FromQuery] long? responseTime,
            [FromQuery] string? extraData,
            [FromQuery] string? signature)
        {
            if (
                string.IsNullOrEmpty(extraData) ||
                string.IsNullOrEmpty(signature) ||
                string.IsNullOrEmpty(orderId) ||
                string.IsNullOrEmpty(orderInfo) ||
                string.IsNullOrEmpty(orderType) ||
                string.IsNullOrEmpty(partnerCode) ||
                string.IsNullOrEmpty(requestId) ||
                message == null || resultCode == null ||
                amount == null || responseTime == null ||
                resultCode == null || transId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string decodedJson = Encoding.UTF8.GetString(Convert.FromBase64String(extraData));
                string redirectUrl = JsonSerializer.Deserialize<string>(decodedJson) ?? string.Empty;
                string trangThaiHoSo = resultCode == 0 ? "XL" : "CTT";
                redirectUrl += (redirectUrl.Contains('?') ? "&" : "?") + $"TrangThaiHoSo={trangThaiHoSo}&resultCode={resultCode}&message={Uri.EscapeDataString(message)}";
                return Redirect(redirectUrl);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}