using DataAccess;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Systems
{
    public interface ISystemInfoService
    {
        Task<SystemInfo> GetSystemInfoAsync();
        Task<CommonResponse> SaveChangeAsync(SystemInfo request);
    }
    public class SystemInfoService : ISystemInfoService
    {
        private readonly ApplicationDbContext _dbContext;
        public SystemInfoService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SystemInfo> GetSystemInfoAsync()
        {            
            return await _dbContext.SystemInfo.OrderBy(t => t.Id).FirstOrDefaultAsync() ?? new SystemInfo();
        }

      
        public async Task<CommonResponse> SaveChangeAsync(SystemInfo request)
        {
            try
            {
                if(request.Id == Guid.Empty)
                {
                     _dbContext.SystemInfo.Add(request);                  
                }
                else
                {
                    var data = _dbContext.SystemInfo.Find(request.Id);
                    if(data == null)
                    {
                        return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cập nhật! Vui lòng thử lại sau" };
                    }
                    data.AppName = request.AppName;
                    data.MfgDate = request.MfgDate;
                    data.Copyright = request.Copyright;
                    data.ExpDate = request.ExpDate;
                    data.LoginLock = request.LoginLock;
                    data.Train = request.Train;
                    _dbContext.SystemInfo.Update(data);
                }
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi! Vui lòng thử lại sau" };
            }
        }
    }
}
