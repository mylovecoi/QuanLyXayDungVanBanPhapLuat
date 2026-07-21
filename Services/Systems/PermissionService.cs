using DataAccess;
using DataAccess.Entities.Systems;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Systems
{
    public interface IPermissionService
    {
        Task<CommonResponse> GetPermissionsByGroupIdAsync(Guid guid, string? Search = null, int PageSize = 5, int PageCurrent = 1);
        Task<CommonResponse> GetAllPermissionsByGroupIdAsync(Guid guid);
        Task<CommonResponse> StorePermissionsAsync(string? group, Guid guid);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(Permission request);
        Task RemoveDatarRedundantAsync();
        Task UpdateStatusByGroupIdAsync(Guid guid);
        Task RemoveRangeByGroupIdAsync(Guid guidGroup);
    }

    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _dbContext;

        public PermissionService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommonResponse> GetPermissionsByGroupIdAsync(Guid guid, string? Search, int PageSize = 5, int PageCurrent = 1)
        {
            var model = from per in _dbContext.Permission.Where(t => t.GroupPermissionId == guid)
                        join role in _dbContext.RoleActions on per.RoleActionId equals role.Id
                        select new Permission
                        {
                            Id = per.Id,
                            GroupPermissionId = per.GroupPermissionId,
                            RoleActionId = per.RoleActionId,
                            RoleActionGroupId = role.RoleGroupId,
                            Status = per.Status,
                            Index = per.Index,
                            Create = per.Create,
                            Edit = per.Edit,
                            Delete = per.Delete,
                            Approve = per.Approve,
                            Public = per.Public,
                            PhanLoai = role.PhanLoai,
                            Level = role.Level,
                            STTSapXep = role.STTSapXep,
                            Title = role.Title,
                            Role = role.Role,
                            Table = role.Table,
                            Controller = role.Controller,
                            Action = role.Action,
                            Icon = role.Icon,
                        }; // Đảm bảo truy vấn dữ liệu trước            

            if (!string.IsNullOrEmpty(Search))
            {
                model = model.Where(t => t.Title != null && t.Title.Contains(Search) || t.Role != null && t.Role.Contains(Search));
            }
            var allPermissions = await model.ToListAsync();
            var rootPermissions = await model.Where(t => t.Level == 0).OrderBy(t => t.STTSapXep).ToListAsync();
            if (!rootPermissions.Any())
            {
                return new CommonResponse { Data = new List<Permission>() };
            }
            List<Permission> list_per = new List<Permission>();
            foreach (var item in rootPermissions)
            {
                list_per.Add(item);

                if (item.PhanLoai == "Group" && item.Index)
                {
                    Recursive(list_per, allPermissions, item.RoleActionId, false);
                }
            }
            var dataView = list_per.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToList();
            return new CommonResponse { Status = "success", Data = dataView, TotalRecord = list_per.Count() };
        }

        public async Task<CommonResponse> GetAllPermissionsByGroupIdAsync(Guid guid)
        {
            var groupPer = _dbContext.GroupsPermision.FirstOrDefault(t => t.Id == guid);
            if (groupPer == null || groupPer.Status == "Dừng kích hoạt")
            {
                return new CommonResponse { Data = new List<Permission>() };
            }
            var model = from per in _dbContext.Permission.Where(t => t.GroupPermissionId == guid)
                        join role in _dbContext.RoleActions.Where(r => r.Status == "Kích hoạt") on per.RoleActionId equals role.Id
                        select new Permission
                        {
                            Id = per.Id,
                            GroupPermissionId = per.GroupPermissionId,
                            RoleActionId = per.RoleActionId,
                            RoleActionGroupId = role.RoleGroupId,
                            Status = per.Status,
                            Index = per.Index,
                            Create = per.Create,
                            Edit = per.Edit,
                            Delete = per.Delete,
                            Approve = per.Approve,
                            Public = per.Public,
                            PhanLoai = role.PhanLoai,
                            Level = role.Level,
                            STTSapXep = role.STTSapXep,
                            Title = role.Title,
                            Role = role.Role,
                            Table = role.Table,
                            Controller = role.Controller,
                            Action = role.Action,
                            Icon = role.Icon,
                        }; // Đảm bảo truy vấn dữ liệu trước

            var allPermissions = await model.ToListAsync();
            var rootPermissions = await model.Where(t => t.Level == 0).OrderBy(t => t.STTSapXep).ToListAsync();
            if (!rootPermissions.Any())
            {
                return new CommonResponse { Data = new List<Permission>() };
            }
            List<Permission> list_per = new List<Permission>();
            foreach (var item in rootPermissions)
            {
                if (item.Index == false) continue;

                list_per.Add(item);

                if (item.PhanLoai == "Group" && item.Index)
                {
                    Recursive(list_per, allPermissions, item.RoleActionId, true);
                }
            }
            return new CommonResponse { Data = list_per };
        }

        private void Recursive(List<Permission> list_per, List<Permission> dataPer, Guid RoleActionId, bool isMenu = false)
        {
            var subPermissions = dataPer.Where(t => t.RoleActionGroupId == RoleActionId).OrderBy(t => t.STTSapXep).ToList();

            foreach (var item in subPermissions)
            {
                if (isMenu == true && item.Index == false)
                    continue;

                list_per.Add(item);

                if (item.PhanLoai == "Group" && item.Index)
                {
                    Recursive(list_per, dataPer, item.RoleActionId, isMenu);
                }
            }
        }

        public async Task<CommonResponse> StorePermissionsAsync(string? group, Guid guid)
        {
            if (string.IsNullOrEmpty(group))
                return new CommonResponse { Status = "error", Message = "Dữ liệu không đúng định dạng!" };
            try
            {

                var data = await _dbContext.RoleActions.Where(t => t.Status == "Kích hoạt")
                                                        .Where(t => t.UseGroup != null && t.UseGroup.Contains(group)).ToListAsync();
                if (!data.Any())
                {
                    return new CommonResponse { Status = "error", Message = "Không có dữ liệu quyền hợp lệ để thêm!" };
                }
                List<Permission> list_add = data.Select(item => new Permission
                {
                    GroupPermissionId = guid,
                    RoleActionId = item.Id,
                    Index = true,
                    Create = item.PhanLoai != "Group" && !item.Role.Contains("Log"),
                    Edit = item.PhanLoai != "Group" && !item.Role.Contains("Log"),
                    Delete = item.PhanLoai != "Group" && !item.Role.Contains("Log"),
                    Approve = item.PhanLoai != "Group" && !item.Role.Contains("Log") && !item.Role.Contains("Systems") && !item.Role.Contains("Settings"),
                    Public = item.PhanLoai != "Group" && !item.Role.Contains("Log") && !item.Role.Contains("Systems") && !item.Role.Contains("Settings"),
                    Status = "CXD",
                }).ToList();
                _dbContext.Permission.AddRange(list_add);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid guid)
        {
            try
            {
                var model = await _dbContext.Permission.SingleOrDefaultAsync(x => x.Id == guid);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
                }

                var roleAction = await _dbContext.RoleActions.SingleOrDefaultAsync(t => t.Id == model.RoleActionId);
                model.Title = roleAction?.Title ?? "";
                model.PhanLoai = roleAction?.PhanLoai ?? "";
                model.Role = roleAction?.Role ?? "";

                return new CommonResponse { Status = "success", Data = model };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> UpdateAsync(Permission request)
        {
            try
            {
                var model = await _dbContext.Permission.FirstOrDefaultAsync(t => t.Id == request.Id);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
                }
                model.Index = request.Index;
                model.Create = request.Index == true ? request.Create : false;
                model.Edit = request.Index == true ? request.Edit : false;
                model.Delete = request.Index == true ? request.Delete : false;
                model.Approve = request.Index == true ? request.Approve : false;
                model.Public = request.Index == true ? request.Public : false;
                _dbContext.Permission.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task RemoveDatarRedundantAsync()
        {
            _dbContext.Permission.RemoveRange(await _dbContext.Permission.Where(t => t.Status == "CXD").ToListAsync());
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStatusByGroupIdAsync(Guid guidGroup)
        {
            await _dbContext.Permission
                .Where(t => t.GroupPermissionId == guidGroup)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Status, "XD"));
        }

        public async Task RemoveRangeByGroupIdAsync(Guid guidGroup)
        {
            await _dbContext.Permission
                    .Where(t => t.GroupPermissionId == guidGroup)
                    .ExecuteDeleteAsync();
        }
    }
}
