using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Model;
using Services.Systems;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Services.Manages
{
    public class MoMoConfig
    {
        public required string PartnerCode { get; set; }
        public required string AccessKey { get; set; }
        public required string SecretKey { get; set; }
        public required string ApiEndpoint { get; set; }
        public required string IpnUrl { get; set; }
        public required string RedirectUrl { get; set; }
    }

    public class MoMoCreatePaymentResponse
    {
        public required string PartnerCode { get; set; }
        public required string OrderId { get; set; }
        public required string RequestId { get; set; }
        public decimal Amount { get; set; }
        public long ResponseTime { get; set; }
        public int ResultCode { get; set; } = -1;
        public required string Message { get; set; }
        public required string PayUrl { get; set; }
        public required string Signature { get; set; }
    }

    public class MoMoQueryPaymentResponse
    {
        public required string PartnerCode { get; set; }
        public required string OrderId { get; set; }
        public string? RequestId { get; set; }
        public decimal Amount { get; set; }
        public string? ExtraData { get; set; }
        public int ResultCode { get; set; } = -1;
        public required string Message { get; set; }
        public long TransId { get; set; }
        public string? PayType { get; set; }
        public long ResponseTime { get; set; }
        public long LastUpdated { get; set; }
        public dynamic[] RefundTrans { get; set; } = [];
        public string? Signature { get; set; }
    }

    public class MoMoIpnRequest
    {
        public required string PartnerCode { get; set; }
        public required string OrderId { get; set; }
        public required string RequestId { get; set; }
        public decimal Amount { get; set; }
        public required string OrderInfo { get; set; }
        public required string OrderType { get; set; }
        public long TransId { get; set; }
        public int ResultCode { get; set; } = -1;
        public required string Message { get; set; }
        public required string PayType { get; set; }
        public long ResponseTime { get; set; }
        public required string ExtraData { get; set; }
        public required string Signature { get; set; }
    }

    public interface IMoMoPaymentService
    {
        Task<CommonResponse> CreatePaymentAsync(Guid hoSoId, string redirectUrl);
        Task ProcessIpnCallbackAsync(MoMoIpnRequest request);
        Task QueryPaymentStatusAsync(string requestId, string orderId);
        bool IsValidSignature(MoMoIpnRequest? request);
    }

    public class MoMoPaymentService(
        ApplicationDbContext dbContext,
        IOptions<MoMoConfig> momoConfig,
        ILogger<MoMoPaymentService> logger,
        HttpClient httpClient,
        IAuthService authService,
        IBackgroundTaskQueue taskQueue) : IMoMoPaymentService
    {
        private readonly MoMoConfig _momoConfig = momoConfig.Value;
        private const string ContentType = "application/json";
        private const string ApiCreateEndpoint = "/v2/gateway/api/create";
        private const string ApiQueryEndpoint = "/v2/gateway/api/query";
        public static readonly JsonSerializerOptions serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<CommonResponse> CreatePaymentAsync(Guid hoSoId, string redirectUrl)
        {
            try
            {
                logger.LogInformation("[MOMO:CREATE] Bắt đầu tạo yêu cầu thanh toán cho hồ sơ {HoSoId}", hoSoId);

                // 1. Lấy thông tin hồ sơ
                var hoSo = await dbContext.HoSoCCCTs
                    .Include(x => x.HoSoCCCTChiPhis)
                    .SingleOrDefaultAsync(x => x.Id == hoSoId);

                if (hoSo == null)
                {
                    logger.LogWarning("[MOMO:CREATE] Không tìm thấy thông tin hồ sơ {HoSoId}", hoSoId);
                    return new CommonResponse("error", "Không tìm thấy thông tin hồ sơ!");
                }

                if (hoSo.DaThanhToan == true)
                {
                    logger.LogWarning("[MOMO:CREATE] Hồ sơ {HoSoId} đã được thanh toán trước đó", hoSoId);
                    return new CommonResponse("error", "Hồ sơ này đã được thanh toán!");
                }

                // 2. Tính tổng chi phí
                var totalAmount = (decimal)hoSo.HoSoCCCTChiPhis.Sum(x => x.ThanhTien);

                if (totalAmount < 1000 || totalAmount > 50000000)
                {
                    logger.LogWarning("[MOMO:CREATE] Số tiền thanh toán {Amount} không hợp lệ cho hồ sơ {HoSoId}", totalAmount, hoSoId);
                    return new CommonResponse("error", "Số tiền thanh toán phải từ 1.000 VNĐ đến 50.000.000 VNĐ!");
                }

                // 3. Tạo thông tin thanh toán MoMo
                var orderId = Guid.NewGuid();
                var requestId = Guid.NewGuid();
                var orderInfo = $"Hồ sơ {hoSo.MaSoHoSo}";
                var extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(redirectUrl)));
                var requestType = "payWithATM";

                logger.LogInformation("[MOMO:CREATE] Tạo giao dịch với OrderId={OrderId}, RequestId={RequestId}", orderId, requestId);

                // 4. Tạo chữ ký
                var rawHash =
                    $"accessKey={_momoConfig.AccessKey}" +
                    $"&amount={totalAmount}" +
                    $"&extraData={extraData}" +
                    $"&ipnUrl={_momoConfig.IpnUrl}" +
                    $"&orderId={orderId}" +
                    $"&orderInfo={orderInfo}" +
                    $"&partnerCode={_momoConfig.PartnerCode}" +
                    $"&redirectUrl={_momoConfig.RedirectUrl}" +
                    $"&requestId={requestId}" +
                    $"&requestType={requestType}";

                var signature = CreateSignature(rawHash, _momoConfig.SecretKey);

                // 5. Tạo request payload
                var requestPayload = new
                {
                    partnerCode = _momoConfig.PartnerCode,
                    requestId,
                    amount = totalAmount,
                    orderId,
                    orderInfo,
                    redirectUrl = _momoConfig.RedirectUrl,
                    ipnUrl = _momoConfig.IpnUrl,
                    requestType,
                    extraData,
                    lang = "vi",
                    signature
                };

                // 6. Gọi API MoMo
                var requestJson = JsonSerializer.Serialize(requestPayload);
                logger.LogInformation("[MOMO:CREATE] Request payload: {NewLine}{RequestContent}", Environment.NewLine, Helper.FormatJson(requestJson));
                var content = new StringContent(requestJson, Encoding.UTF8, ContentType);
                var response = await httpClient.PostAsync($"{_momoConfig.ApiEndpoint}{ApiCreateEndpoint}", content);

                // 7. Xử lý response
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogInformation("[MOMO:CREATE] Response content: {NewLine}{ResponseContent}", Environment.NewLine, Helper.FormatJson(responseContent));
                response.EnsureSuccessStatusCode();

                var paymentResponse = JsonSerializer.Deserialize<MoMoCreatePaymentResponse>(responseContent, serializerOptions);

                if (paymentResponse == null)
                {
                    logger.LogError("[MOMO:CREATE] Không thể phân tích dữ liệu phản hồi cho hồ sơ {HoSoId}", hoSoId);
                    return new CommonResponse("error", "Không thể phân tích dữ liệu phản hồi từ MoMo!");
                }

                var payUrl = paymentResponse.PayUrl;

                if (payUrl == null)
                {
                    logger.LogError("[MOMO:CREATE] Không nhận được URL thanh toán cho hồ sơ {HoSoId}", hoSoId);
                    return new CommonResponse("error", "Không thể tạo yêu cầu thanh toán!");
                }

                // 8. Lưu thông tin payment vào database
                var payment = new MoMoPayment
                {
                    HoSoId = hoSoId,
                    RequestId = requestId.ToString(),
                    OrderId = orderId.ToString(),
                    Amount = totalAmount,
                    PaymentUrl = payUrl,
                    ExtraData = extraData,
                    RequestSignature = signature,
                    CreatedBy = authService.GetUserInfo()?.Id ?? Guid.Empty,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = authService.GetUserInfo()?.Id ?? Guid.Empty,
                    UpdatedDate = DateTime.Now
                };

                dbContext.MoMoPayments.Add(payment);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("[MOMO:CREATE] Đã tạo thành công yêu cầu thanh toán cho hồ sơ {HoSoId}, OrderId={OrderId}", hoSoId, orderId);

                // 9. Trả về URL thanh toán
                return new CommonResponse("success", "Tạo yêu cầu thanh toán thành công!", new { payUrl });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[MOMO:CREATE] Lỗi khi tạo yêu cầu thanh toán MoMo cho hồ sơ {HoSoId}", hoSoId);
                return new CommonResponse("error", $"Lỗi khi tạo yêu cầu thanh toán: {ex.Message}");
            }
        }

        public async Task ProcessIpnCallbackAsync(MoMoIpnRequest request)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                logger.LogInformation("[MOMO:IPN] Nhận IPN callback: OrderId={OrderId}, RequestId={RequestId}, ResultCode={ResultCode}",
                    request.OrderId, request.RequestId, request.ResultCode);

                // 1. Tìm giao dịch trong database và lock lại trong transaction
                var payment = await dbContext.MoMoPayments
                    .SingleOrDefaultAsync(p => p.OrderId == request.OrderId && p.RequestId == request.RequestId);

                if (payment == null)
                {
                    logger.LogError("[MOMO:IPN] Không tìm thấy giao dịch thanh toán: OrderId={OrderId}, RequestId={RequestId}",
                        request.OrderId, request.RequestId);
                    return;
                }

                // Kiểm tra nếu đã xử lý thành công hoặc đã ở trạng thái final status
                if (payment.ResultCode.HasValue && MoMoPayment.FINAL_STATUS_CODES.Contains(payment.ResultCode.Value))
                {
                    logger.LogWarning("[MOMO:IPN] Giao dịch đã được xử lý trước đó và có trạng thái final status: OrderId={OrderId}, RequestId={RequestId}, ResultCode={ResultCode}",
                        request.OrderId, request.RequestId, payment.ResultCode);
                    return;
                }

                // 2. Cập nhật thông tin từ IPN
                payment.TransId = request.TransId;
                payment.ResultCode = request.ResultCode;
                payment.Message = request.Message;
                payment.PayType = request.PayType;
                payment.ResponseTime = request.ResponseTime;
                payment.ExtraData = request.ExtraData;
                payment.ResponseSignature = request.Signature;
                payment.ProcessedDate = DateTime.Now;
                payment.UpdatedDate = DateTime.Now;

                // 3. Xử lý theo mã kết quả
                if (request.ResultCode == MoMoPayment.RESULT_SUCCESS) // Thành công
                {
                    var hoSo = await dbContext.HoSoCCCTs.SingleOrDefaultAsync(x => x.Id == payment.HoSoId);
                    if (hoSo != null)
                    {
                        hoSo.DaThanhToan = true;
                        hoSo.NgayThanhToan = DateTime.Now;
                        hoSo.Status = "XL";
                    }
                    logger.LogInformation("[MOMO:IPN] Thanh toán thành công cho mã giao dịch: OrderId={OrderId}, hồ sơ {HoSoId}",
                        request.OrderId, payment.HoSoId);
                }
                else
                {
                    // Kiểm tra xem result code có thuộc nhóm final status không
                    bool isFinalStatus = MoMoPayment.FINAL_STATUS_CODES.Contains(request.ResultCode);

                    if (isFinalStatus)
                    {
                        logger.LogInformation("[MOMO:IPN] Giao dịch kết thúc với final status: OrderId={OrderId}, ResultCode={ResultCode}, Message={Message}",
                            request.OrderId, request.ResultCode, request.Message);
                    }
                    else
                    {
                        // Đây là non-final status, cần kiểm tra lại sau
                        payment.RetryCount += 1;
                        logger.LogInformation("[MOMO:IPN] Giao dịch nhận được non-final status: OrderId={OrderId}, ResultCode={ResultCode}, sẽ được đưa vào hàng đợi kiểm tra sau",
                            request.OrderId, request.ResultCode);
                    }
                }

                // 4. Lưu thay đổi
                await dbContext.SaveChangesAsync();

                // 5. Commit transaction
                await transaction.CommitAsync();

                // 6. Thêm vào queue để kiểm tra lại nếu giao dịch có non-final status
                if (payment.ResultCode.HasValue && MoMoPayment.NON_FINAL_STATUS_CODES.Contains(payment.ResultCode.Value))
                {
                    string jobName = $"MOMO_DELAYED_CHECK_{payment.OrderId}";

                    await taskQueue.QueueBackgroundWorkItemAsync(async (scopedProvider, token) =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes(2), token);
                        logger.LogInformation("[QUEUE:{JobName}] Bắt đầu kiểm tra trạng thái giao dịch sau 2 phút", jobName);
                        var scopedMomoService = scopedProvider.GetRequiredService<IMoMoPaymentService>();
                        await scopedMomoService.QueryPaymentStatusAsync(payment.RequestId, payment.OrderId);
                    }, jobName);

                    logger.LogInformation("[MOMO:IPN] Đã đưa giao dịch {OrderId} vào hàng đợi với tên [{JobName}] để kiểm tra sau 2 phút",
                        request.OrderId, jobName);
                }
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                logger.LogError(ex, "[MOMO:IPN] Lỗi xử lý thông báo IPN từ MoMo: OrderId={OrderId}, RequestId={RequestId}",
                    request.OrderId, request.RequestId);
            }
        }

        public async Task QueryPaymentStatusAsync(string requestId, string orderId)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                logger.LogInformation("[MOMO:QUERY] Bắt đầu truy vấn trạng thái thanh toán: RequestId={RequestId}, OrderId={OrderId}",
                    requestId, orderId);

                // Kiểm tra database trước
                var payment = await dbContext.MoMoPayments
                    .SingleOrDefaultAsync(p => p.OrderId == orderId && p.RequestId == requestId);

                if (payment == null)
                {
                    logger.LogError("[MOMO:QUERY] Không tìm thấy thông tin giao dịch để truy vấn: RequestId={RequestId}, OrderId={OrderId}",
                        requestId, orderId);
                    return;
                }

                // Kiểm tra nếu đã xử lý final status, không cần gọi API nữa
                if (payment.ResultCode.HasValue && MoMoPayment.FINAL_STATUS_CODES.Contains(payment.ResultCode.Value))
                {
                    logger.LogInformation("[MOMO:QUERY] Giao dịch đã được xử lý với final status: RequestId={RequestId}, OrderId={OrderId}, Mã kết quả={ResultCode}",
                        requestId, orderId, payment.ResultCode);
                    return;
                }

                // Tạo chữ ký
                var rawHash =
                    $"accessKey={_momoConfig.AccessKey}" +
                    $"&orderId={orderId}" +
                    $"&partnerCode={_momoConfig.PartnerCode}" +
                    $"&requestId={requestId}";

                var signature = CreateSignature(rawHash, _momoConfig.SecretKey);

                // Tạo request payload
                var requestPayload = new
                {
                    partnerCode = _momoConfig.PartnerCode,
                    requestId,
                    orderId,
                    lang = "vi",
                    signature
                };

                // Gọi API MoMo
                var requestJson = JsonSerializer.Serialize(requestPayload);
                logger.LogInformation("[MOMO:QUERY] Request payload: {NewLine}{RequestContent}", Environment.NewLine, Helper.FormatJson(requestJson));
                var content = new StringContent(requestJson, Encoding.UTF8, ContentType);
                var response = await httpClient.PostAsync($"{_momoConfig.ApiEndpoint}{ApiQueryEndpoint}", content);

                // Xử lý response
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogInformation("[MOMO:QUERY] Response content: {NewLine}{ResponseContent}", Environment.NewLine, Helper.FormatJson(responseContent));
                response.EnsureSuccessStatusCode();
                var queryResponse = JsonSerializer.Deserialize<MoMoQueryPaymentResponse>(responseContent, serializerOptions);

                if (queryResponse == null)
                {
                    logger.LogError("[MOMO:QUERY] Không thể phân tích dữ liệu phản hồi từ MoMo: RequestId={RequestId}, OrderId={OrderId}",
                        requestId, orderId);
                    throw new Exception("Không thể phân tích dữ liệu phản hồi từ MoMo");
                }

                // Cập nhật thông tin payment
                payment.TransId = queryResponse.TransId;
                payment.ResultCode = queryResponse.ResultCode;
                payment.Message = queryResponse.Message;
                payment.PayType = queryResponse.PayType;
                payment.ResponseTime = queryResponse.ResponseTime;
                payment.ResponseSignature = queryResponse.Signature;
                payment.ProcessedDate = DateTime.Now;
                payment.UpdatedDate = DateTime.Now;

                // Xử lý theo ResultCode
                if (queryResponse.ResultCode == MoMoPayment.RESULT_SUCCESS) // Thành công
                {
                    var hoSo = await dbContext.HoSoCCCTs.SingleOrDefaultAsync(h => h.Id == payment.HoSoId);
                    if (hoSo == null)
                    {
                        logger.LogError("Không tìm thấy hồ sơ với ID={HoSoId} để cập nhật trạng thái thanh toán", payment.HoSoId);
                        throw new Exception($"Không tìm thấy hồ sơ với ID={payment.HoSoId} để cập nhật trạng thái thanh toán");
                    }
                    hoSo.DaThanhToan = true;
                    hoSo.NgayThanhToan = DateTime.Now;
                    hoSo.Status = "XL";
                }
                else if (payment.ResultCode.HasValue && MoMoPayment.NON_FINAL_STATUS_CODES.Contains(payment.ResultCode.Value))
                {
                    // Đối với các non-final status, tăng bộ đếm retry
                    payment.RetryCount += 1;
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // Nếu vẫn là non-final status, kiểm tra số lần thử lại
                if (payment.ResultCode.HasValue && MoMoPayment.NON_FINAL_STATUS_CODES.Contains(payment.ResultCode.Value))
                {
                    if (payment.RetryCount > 5)
                    {
                        // Đã vượt quá số lần thử lại, cập nhật trạng thái thành RESULT_RETRY_EXCEEDED
                        payment.ResultCode = MoMoPayment.RESULT_RETRY_EXCEEDED;
                        payment.Message = "Giao dịch vượt quá số lần kiểm tra trạng thái thanh toán";
                        payment.UpdatedDate = DateTime.Now;
                        payment.ProcessedDate = DateTime.Now;

                        logger.LogError(
                            "[MOMO:QUERY] OrderId={OrderId} cho hồ sơ {HoSoId} đã vượt quá 5 lần kiểm tra.",
                            payment.OrderId, payment.HoSoId);

                        // Lưu các thay đổi vào database
                        await dbContext.SaveChangesAsync();
                        return;
                    }

                    // Chưa vượt quá số lần thử lại, đưa lại vào hàng đợi
                    string jobName = $"MOMO_RETRY_CHECK_{payment.OrderId}_{payment.RetryCount}";

                    await taskQueue.QueueBackgroundWorkItemAsync(async (scopedProvider, token) =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes(2), token);
                        logger.LogInformation("[QUEUE:{JobName}] Kiểm tra lại trạng thái giao dịch sau 2 phút (lần thử {RetryCount})",
                            jobName, payment.RetryCount + 1);
                        var scopedMomoService = scopedProvider.GetRequiredService<IMoMoPaymentService>();
                        await scopedMomoService.QueryPaymentStatusAsync(payment.RequestId, payment.OrderId);
                    }, jobName);

                    logger.LogInformation("[MOMO:QUERY] Đã đưa OrderId={OrderId} RetryCount={RetryCount} vào hàng đợi với tên [{JobName}] để kiểm tra lại sau",
                        orderId, payment.RetryCount, jobName);
                }
                else
                {
                    logger.LogInformation("[MOMO:QUERY] Giao dịch {OrderId} đã được cập nhật trạng thái thành {ResultCode}: {Message}",
                        orderId, queryResponse.ResultCode, queryResponse.Message);
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "[MOMO:QUERY] Lỗi truy vấn trạng thái thanh toán MoMo: RequestId={RequestId}, OrderId={OrderId}",
                    requestId, orderId);
            }
        }

        private static string CreateSignature(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return Convert.ToHexStringLower(hashBytes);
        }

        public bool IsValidSignature(MoMoIpnRequest? request)
        {
            if (request == null)
            {
                return false;
            }

            // Tạo chuỗi raw signature theo thứ tự các tham số
            string rawSignature =
                $"accessKey={_momoConfig.AccessKey}" +
                $"&amount={request.Amount}" +
                $"&extraData={request.ExtraData}" +
                $"&message={request.Message}" +
                $"&orderId={request.OrderId}" +
                $"&orderInfo={request.OrderInfo}" +
                $"&orderType={request.OrderType}" +
                $"&partnerCode={request.PartnerCode}" +
                $"&payType={request.PayType}" +
                $"&requestId={request.RequestId}" +
                $"&responseTime={request.ResponseTime}" +
                $"&resultCode={request.ResultCode}" +
                $"&transId={request.TransId}";

            // Tạo chữ ký HMAC-SHA256
            var calculatedSignature = CreateSignature(rawSignature, _momoConfig.SecretKey);

            // So sánh với chữ ký từ request
            return calculatedSignature.Equals(request.Signature);
        }
    }
}