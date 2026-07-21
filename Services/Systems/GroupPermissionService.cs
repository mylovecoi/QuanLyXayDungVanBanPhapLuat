using DataAccess;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Systems
{
    public interface IGroupPermissionService
    {
        Task<CommonResponse> GetGroupPermissionsAsync(string Search, int PageSize = 5, int PageCurrent = 1);
        Task<CommonResponse> StoreAsync(GroupPermision groupPermision);
        Task<CommonResponse> EditAsync(Guid Id);
        Task<CommonResponse> UpdateAsync(GroupPermision groupPermision);
        Task<CommonResponse> DeleteAsync(Guid Id);
        Task<List<GroupPermision>> GetAllGroupPermissionsAsync(string Status);
    }
    public class GroupPermissionService : IGroupPermissionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        public GroupPermissionService(ApplicationDbContext dbContext, IPermissionService permissionService)
        {
            _dbContext = dbContext;
            _permissionService = permissionService;
        }

        public async Task<CommonResponse> GetGroupPermissionsAsync(string Search, int PageSize, int PageCurrent)
        {
            try
            {
                var data = _dbContext.GroupsPermision.AsQueryable();
                if (!string.IsNullOrEmpty(Search))
                {
                    data = data.Where(t => t.Name.Contains(Search) || t.Status.Contains(Search));
                }
                var dataView = await data.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToListAsync();
                return new CommonResponse { Status = "success", Data = dataView, TotalRecord = data.Count() };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> StoreAsync(GroupPermision groupPermision)
        {
            if (groupPermision == null)
            {
                return new CommonResponse { Status = "error", Message = "Dữ liệu không hợp lệ" };
            }
            try
            {
                _dbContext.GroupsPermision.Add(groupPermision);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid Id)
        {
            try
            {
                var model = await _dbContext.GroupsPermision.FirstOrDefaultAsync(t => t.Id == Id);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };

                }
                return new CommonResponse { Status = "success", Data = model };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> UpdateAsync(GroupPermision groupPermision)
        {
            if (groupPermision == null)
            {
                return new CommonResponse { Status = "error", Message = "Dữ liệu không hợp lệ" };
            }
            try
            {
                _dbContext.GroupsPermision.Update(groupPermision);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid Id)
        {
            try
            {
                var model = _dbContext.GroupsPermision.FirstOrDefault(t => t.Id == Id);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
                }
                await _permissionService.RemoveRangeByGroupIdAsync(Id);
                _dbContext.GroupsPermision.Remove(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<List<GroupPermision>> GetAllGroupPermissionsAsync(string Status)
        {
            var data = _dbContext.GroupsPermision.Where(t => t.Status == Status);
            return await data.ToListAsync();
        }
    }
}
