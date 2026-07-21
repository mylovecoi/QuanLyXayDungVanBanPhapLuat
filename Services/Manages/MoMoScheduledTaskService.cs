using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services.Manages
{
    public class MoMoScheduledTaskService(
        IServiceProvider serviceProvider,
        ILogger<MoMoScheduledTaskService> logger) : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromMinutes(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("[SCHEDULED] MoMo Scheduled Task Service đang khởi động");

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("[SCHEDULED] Bắt đầu chu kỳ kiểm tra giao dịch MoMo");

                await CheckPendingPayments(stoppingToken);
                await HandleRetryExceededPayments(stoppingToken);

                logger.LogInformation("[SCHEDULED] Hoàn thành chu kỳ kiểm tra giao dịch MoMo, sẽ kiểm tra lại sau {Minutes} phút", _period.TotalMinutes);

                // Đợi 5 phút trước khi kiểm tra lại
                await Task.Delay(_period, stoppingToken);
            }

            logger.LogInformation("[SCHEDULED] MoMo Scheduled Task Service đã dừng");
        }

        private async Task CheckPendingPayments(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("[SCHEDULED:CHECK_PENDING] Bắt đầu kiểm tra các giao dịch đang xử lý");
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var moMoPaymentService = scope.ServiceProvider.GetRequiredService<IMoMoPaymentService>();

                // Lấy các giao dịch không phải final status hoặc null
                var pendingPayments = await dbContext.MoMoPayments
                    .Where(p => !p.ResultCode.HasValue || MoMoPayment.NON_FINAL_STATUS_CODES.Contains(p.ResultCode.Value))
                    .Where(p => p.RetryCount <= 5)   // Giới hạn số lần thử lại
                    .ToListAsync(cancellationToken);

                logger.LogInformation("[SCHEDULED:CHECK_PENDING] Tìm thấy {Count} giao dịch cần kiểm tra trạng thái", pendingPayments.Count);

                foreach (var payment in pendingPayments)
                {
                    // Đảm bảo không kiểm tra cùng lúc nhiều giao dịch trùng lặp
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    logger.LogInformation("[SCHEDULED:CHECK_PENDING] Kiểm tra giao dịch: OrderId={OrderId}, RequestId={RequestId}",
                        payment.OrderId, payment.RequestId);

                    await moMoPaymentService.QueryPaymentStatusAsync(payment.RequestId, payment.OrderId);

                    // Đợi một khoảng thời gian nhỏ giữa các lần gọi API để tránh quá tải
                    await Task.Delay(1000, cancellationToken);
                }

                logger.LogInformation("[SCHEDULED:CHECK_PENDING] Hoàn thành kiểm tra {Count} giao dịch đang xử lý", pendingPayments.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SCHEDULED:CHECK_PENDING] Lỗi khi kiểm tra các giao dịch đang xử lý");
            }
        }

        private async Task HandleRetryExceededPayments(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("[SCHEDULED:RETRY_EXCEEDED] Bắt đầu xử lý các giao dịch vượt số lần thử lại");
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Lấy các giao dịch đã vượt quá số lần thử lại và không phải final status
                var exceededPayments = await dbContext.MoMoPayments
                    .Where(p => !p.ResultCode.HasValue || !MoMoPayment.FINAL_STATUS_CODES.Contains(p.ResultCode.Value))
                    .Where(p => p.RetryCount > 5 && p.ResultCode != MoMoPayment.RESULT_RETRY_EXCEEDED)  // Các giao dịch đã vượt quá số lần thử lại nhưng chưa được đánh dấu
                    .ToListAsync(cancellationToken);

                logger.LogInformation("[SCHEDULED:RETRY_EXCEEDED] Tìm thấy {Count} giao dịch đã vượt quá số lần thử lại", exceededPayments.Count);

                if (exceededPayments.Count != 0)
                {
                    foreach (var payment in exceededPayments)
                    {
                        // Cập nhật trạng thái giao dịch
                        payment.ResultCode = MoMoPayment.RESULT_RETRY_EXCEEDED;
                        payment.Message = "Giao dịch vượt quá số lần kiểm tra trạng thái thanh toán";
                        payment.UpdatedDate = DateTime.Now;
                        payment.ProcessedDate = DateTime.Now;

                        logger.LogError("[SCHEDULED:RETRY_EXCEEDED] OrderId={OrderId} cho hồ sơ {HoSoId} đã vượt quá 5 lần kiểm tra.",
                            payment.OrderId, payment.HoSoId);
                    }

                    // Lưu các thay đổi vào database
                    await dbContext.SaveChangesAsync(cancellationToken);
                    logger.LogInformation("[SCHEDULED:RETRY_EXCEEDED] Đã cập nhật {Count} giao dịch sang trạng thái vượt quá số lần thử lại",
                        exceededPayments.Count);
                }

                logger.LogInformation("[SCHEDULED:RETRY_EXCEEDED] Hoàn thành xử lý các giao dịch vượt số lần thử lại");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SCHEDULED:RETRY_EXCEEDED] Lỗi khi xử lý các giao dịch vượt quá số lần thử lại");
            }
        }
    }
}