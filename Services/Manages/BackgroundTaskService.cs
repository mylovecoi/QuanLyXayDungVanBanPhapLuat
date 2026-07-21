using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Services.Manages
{
    public delegate ValueTask BackgroundWorkItem(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    // IBackgroundTaskQueue interface
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(BackgroundWorkItem workItem, string jobName);
        ValueTask<(BackgroundWorkItem WorkItem, string JobName)> DequeueAsync(CancellationToken cancellationToken);
    }

    // BackgroundTaskQueue implementation
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<(BackgroundWorkItem WorkItem, string JobName)> _queue;

        public BackgroundTaskQueue(IConfiguration configuration)
        {
            var options = new BoundedChannelOptions(configuration.GetValue<int>("QueueCapacity"))
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<(BackgroundWorkItem, string)>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(BackgroundWorkItem workItem, string jobName)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            await _queue.Writer.WriteAsync((workItem, jobName));
        }

        public async ValueTask<(BackgroundWorkItem WorkItem, string JobName)> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }

    // QueuedHostedService để xử lý background tasks
    public class QueuedHostedService(
        IBackgroundTaskQueue taskQueue,
        IServiceProvider serviceProvider,
        ILogger<QueuedHostedService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("[QUEUE] Background Task Queue Service đang khởi động");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var (workItem, jobName) = await taskQueue.DequeueAsync(stoppingToken);

                    logger.LogInformation("[QUEUE:{JobName}] Bắt đầu xử lý công việc", jobName);

                    using var scope = serviceProvider.CreateScope();
                    try
                    {
                        await workItem(scope.ServiceProvider, stoppingToken);
                        logger.LogInformation("[QUEUE:{JobName}] Hoàn thành xử lý công việc", jobName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "[QUEUE:{JobName}] Lỗi xảy ra khi thực thi công việc", jobName);
                    }
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("[QUEUE] Đã hủy xử lý công việc do service đang dừng");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[QUEUE] Lỗi không xác định khi xử lý công việc từ queue");
                }
            }

            logger.LogInformation("[QUEUE] Background Task Queue Service đã dừng");
        }
    }
}