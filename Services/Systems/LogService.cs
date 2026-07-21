using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Services.Systems
{
    public interface ILogService
    {
        //Task<List<Log>> GetLogsAsync(string TimKiem, int PageSize, int PageCurrent, DateTime? NgayBatDau, DateTime? NgayKetThuc);
        Task<CommonResponse> GetLogsWithFilterAsync(string TimKiem, int PageSize, int PageCurrent, DateTime NgayBatDau, DateTime NgayKetThuc);
        Task Store(Log request);
        Task Clean();
    }
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISystemInfoService _systemInfoService;
        public LogService(ApplicationDbContext dbContext, ISystemInfoService systemInfoService)
        {
            _dbContext = dbContext;
            _systemInfoService = systemInfoService;
        }
 

        public async Task Store(Log request)
        {
            _dbContext.Logs.Add(request);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Clean()
        {
            var systemInfo = await _systemInfoService.GetSystemInfoAsync();
            int logKeepingDays = 30;
            DateTime dateLogRemove = DateTime.Now.AddDays(-logKeepingDays);
            var logs = await _dbContext.Logs.Where(t => t.CreatedDate < dateLogRemove).ToListAsync();
            if (logs.Count > 0)
            {
                _dbContext.Logs.RemoveRange(logs);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<CommonResponse> GetLogsWithFilterAsync(string TimKiem, int PageSize, int PageCurrent, DateTime NgayBatDau, DateTime NgayKetThuc)
        {
            try
            {
                var model = _dbContext.Logs.AsQueryable().AsNoTracking()
                    .Where(l => l.CreatedDate >= NgayBatDau && l.CreatedDate <= NgayKetThuc.AddDays(1).AddTicks(-1));

                if (!string.IsNullOrEmpty(TimKiem))
                {
                    model = model.Where(t => (t.Username != null && t.Username.Contains(TimKiem))
                                            || (t.Url != null && t.Url.Contains(TimKiem))
                                            || (t.Method != null && t.Method.Contains(TimKiem))
                                            || (t.IpAddress != null && t.IpAddress.Contains(TimKiem)));

                }


                model = model.OrderByDescending(l => l.CreatedDate);
                int totalRecord = await model.CountAsync();
                var dataView = await model.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToListAsync();

                return new("success", "Lấy dữ liệu thành công", dataView, totalRecord);
            }
            catch (Exception)
            {
                return new("error", "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!");
            }
        }
    }
}
