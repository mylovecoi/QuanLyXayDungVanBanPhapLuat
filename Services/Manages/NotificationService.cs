using DataAccess;
using Services.Model;
using DataAccess.Entities.Manages;
using Microsoft.EntityFrameworkCore;
using Services.Systems;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using Services.Hubs;

namespace Services.Manages
{
    public interface INotificationService
    {
        Task<CommonResponse> GetNotificationAsync(string Search, int PageSize, int PageCurrent, string Status = "");
        Task<int> CountNotificationAsync();
        Task<List<Notification>> GetLatestNotificationsAsync(int top = 5);
        Task<CommonResponse> StoreAsync(Notification request);
        //Task<CommonResponse> UpdateAsync(Guid guidView, Guid guid);
        Task<Notification> GetNoticationByIdAsync(Guid guid);
        Task MarkAsReadAsync(Guid guid);

        bool ShowNotification(Guid guid, string phanLoai, out string controller, out string action, out object parameter);
    }
    public class NotificationService(ApplicationDbContext dbContext, IAuthService authService,
                                    IRoleActionService roleActionService, IHubContext<RequestHub> hubContext) : INotificationService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;
        private readonly IRoleActionService _roleActionService = roleActionService;
        private readonly IHubContext<RequestHub> _hubContext = hubContext;

        public async Task<CommonResponse> GetNotificationAsync(string Search, int PageSize, int PageCurrent, string Status = "")
        {
            try
            {
                var idDonVi = _authService.GetUserInfo()?.DanhMucDonViId ?? Guid.Empty;
                var isSSA = _authService.GetUserInfo()?.SSA ?? false;

                var query = from noti in _dbContext.Notifications.AsNoTracking()
                            join dmdv in _dbContext.DanhMucDonVis.AsNoTracking()
                                on noti.DonViGui equals dmdv.Id into dmdvGroup
                            from dmdv in dmdvGroup.DefaultIfEmpty()
                            select new Notification
                            {
                                Id = noti.Id,
                                DonViGui = noti.DonViGui,
                                DonViTiepNhan = noti.DonViTiepNhan,
                                DonViDongChuyen = noti.DonViDongChuyen,
                                NoiDung = noti.NoiDung,
                                DonViView = noti.DonViView,
                                ActionNameDanhSach = noti.ActionNameDanhSach,
                                ControllerNameDanhSach = noti.ControllerNameDanhSach,
                                ActionNameXetDuyet = noti.ActionNameXetDuyet,
                                ControllerNameXetDuyet = noti.ControllerNameXetDuyet,
                                CreatedDate = noti.CreatedDate,
                                CreatedBy = noti.CreatedBy,
                                TenDonViGuiThongBao = dmdv.TenDonVi ?? ""
                            };

                if (!isSSA)
                {
                    query = query.Where(x => x.DonViGui == idDonVi || x.DonViTiepNhan == idDonVi || (x.DonViDongChuyen ?? "").Contains(idDonVi.ToString()));
                }

                if (!string.IsNullOrEmpty(Search))
                {
                    var searchLower = Search.ToLower();
                    query = query.Where(x =>
                        (x.TenDonViGuiThongBao ?? "").Contains(searchLower) ||
                        (x.NoiDung ?? "").Contains(searchLower));
                }

                var allData = await query.ToListAsync();

                foreach (var item in allData)
                {
                    item.DaXem = item.DonViView.Contains(idDonVi) == true;
                }

                if (Status == "ChuaXem")
                {
                    allData = [.. allData.Where(x => !x.DaXem)];
                }
                else if (Status == "DaXem")
                {
                    allData = [.. allData.Where(x => x.DaXem)];
                }

                var sortedData = allData.OrderBy(x => x.DaXem).ThenByDescending(x => x.CreatedDate).ToList();

                int totalRecord = sortedData.Count;
                var dataView = sortedData.Skip((PageCurrent - 1) * PageSize).Take(PageSize);

                foreach (var item in dataView)
                {
                    item.RoleDanhSach = _roleActionService.GetRoleByControllerAction(item.ControllerNameDanhSach, item.ActionNameDanhSach);
                    item.RoleXetDuyet = _roleActionService.GetRoleByControllerAction(item.ControllerNameXetDuyet, item.ActionNameXetDuyet);
                }

                return new CommonResponse { Status = "success", Data = dataView, TotalRecord = totalRecord };

            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi lưu dữ liệu. Vui lòng thử lại sau!" };
            }
        }

        public async Task<int> CountNotificationAsync()
        {
            var donViId = _authService.GetUserInfo()?.DanhMucDonViId ?? Guid.Empty;
            if (donViId == Guid.Empty)
            {
                return 0;
            }

            var count = await _dbContext.Notifications
                .Where(n =>
                    (
                        n.DonViGui == donViId ||
                        n.DonViTiepNhan == donViId ||
                        (
                            !string.IsNullOrWhiteSpace(n.DonViDongChuyen) &&
                            n.DonViDongChuyen.Contains(donViId.ToString())
                        )
                    ) &&
                    !n.DonViView.Contains(donViId)
                )
                .CountAsync();

            return count;
        }

        public async Task<List<Notification>> GetLatestNotificationsAsync(int top = 5)
        {
            var idDonVi = _authService.GetUserInfo()?.DanhMucDonViId ?? Guid.Empty;
            var isSSA = _authService.GetUserInfo()?.SSA ?? false;
            if (idDonVi == Guid.Empty && !isSSA)
            {
                return [];
            }

            var query = from noti in _dbContext.Notifications.AsNoTracking()
                        join dmdv in _dbContext.DanhMucDonVis.AsNoTracking()
                            on noti.DonViGui equals dmdv.Id into dmdvGroup
                        from dmdv in dmdvGroup.DefaultIfEmpty()
                        select new Notification
                        {
                            Id = noti.Id,
                            DonViGui = noti.DonViGui,
                            DonViTiepNhan = noti.DonViTiepNhan,
                            DonViDongChuyen = noti.DonViDongChuyen,
                            NoiDung = noti.NoiDung,
                            DonViView = noti.DonViView,
                            ActionNameDanhSach = noti.ActionNameDanhSach,
                            ControllerNameDanhSach = noti.ControllerNameDanhSach,
                            ActionNameXetDuyet = noti.ActionNameXetDuyet,
                            ControllerNameXetDuyet = noti.ControllerNameXetDuyet,
                            CreatedDate = noti.CreatedDate,
                            CreatedBy = noti.CreatedBy,
                            TenDonViGuiThongBao = dmdv.TenDonVi ?? ""
                        };

            if (!isSSA)
            {
                query = query.Where(x => x.DonViGui == idDonVi || x.DonViTiepNhan == idDonVi || (x.DonViDongChuyen ?? "").Contains(idDonVi.ToString()));
            }

            var data = await query
                .OrderBy(x => x.DonViView.Contains(idDonVi))
                .ThenByDescending(x => x.CreatedDate)
                .Take(top)
                .ToListAsync();

            foreach (var item in data)
            {
                item.DaXem = item.DonViView.Contains(idDonVi);
                item.UrlDanhSach = BuildNotificationUrl(item.Id, "DanhSach");
                item.UrlXetDuyet = BuildNotificationUrl(item.Id, "XetDuyet");
                item.RoleDanhSach = _roleActionService.GetRoleByControllerAction(item.ControllerNameDanhSach, item.ActionNameDanhSach);
                item.RoleXetDuyet = _roleActionService.GetRoleByControllerAction(item.ControllerNameXetDuyet, item.ActionNameXetDuyet);
            }

            return data;
        }

        public async Task<CommonResponse> StoreAsync(Notification request)
        {
            try
            {
                _dbContext.Notifications.Add(request);
                await _dbContext.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
                return new CommonResponse { Status = "success", Message = "Lưu dữ liệu thành công!" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi lưu dữ liệu thông báo. Vui lòng thử lại sau!" };
            }
        }

        //public async Task<CommonResponse> UpdateAsync(Guid guidView, Guid guid)
        //{
        //    try
        //    {
        //        var data = await _dbContext.Notifications.FindAsync(guid);
        //        if (data == null)
        //        {
        //            return new CommonResponse { Status = "error", Message = "Không tìm thấy dữ liệu thông báo cần cập nhật!" };
        //        }
        //        // Nếu DonViView là null hoặc rỗng thì gán giá trị mới
        //        if (string.IsNullOrEmpty(data.DonViView))
        //        {
        //            data.DonViView = guidView.ToString() + ",";
        //        }
        //        // Nếu DonViView đã chứa guidView thì không thêm lại
        //        else if (!data.DonViView.Split(',').Contains(guidView.ToString()))
        //        {
        //            data.DonViView += guidView.ToString() + ",";
        //        }
        //        await _dbContext.SaveChangesAsync();
        //        return new CommonResponse { Status = "success", Message = "Lưu dữ liệu thông báo thành công!" };
        //    }
        //    catch
        //    {
        //        return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi lưu dữ liệu thông báo. Vui lòng thử lại sau!" };
        //    }
        //}

        public async Task<Notification> GetNoticationByIdAsync(Guid guid)
        {
            return await _dbContext.Notifications.FindAsync(guid) ?? throw new Exception("Không tìm thấy thông báo!");
        }

        public async Task MarkAsReadAsync(Guid guid)
        {
            var model = await GetNoticationByIdAsync(guid);
            var donViId = _authService.GetUserInfo()?.DanhMucDonViId ?? Guid.Empty;
            var isSSA = _authService.GetUserInfo()?.SSA ?? false;

            if (donViId != Guid.Empty && !model.DonViView.Contains(donViId))
            {
                model.DonViView.Add(donViId);
            }

            _dbContext.Notifications.Update(model);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
        }

        public bool ShowNotification(Guid guid, string phanLoai, out string controller, out string action, out object parameter)
        {
            var data = _dbContext.Notifications.Find(guid);
            if (data == null)
            {
                controller = "Manages";
                action = "Notifications";
                parameter = new object();
                return false;
            }
            if (phanLoai == "DanhSach")
            {
                controller = data.ControllerNameDanhSach ?? "";
                action = data.ActionNameDanhSach ?? "";
                parameter = Helper.ConvertStringToDictionary(data.ParameterDanhSach ?? "");
            }
            else
            {
                controller = data.ControllerNameXetDuyet ?? "";
                action = data.ActionNameXetDuyet ?? "";
                parameter = Helper.ConvertStringToDictionary(data.ParameterXetDuyet ?? "");
            }
            return true;
        }

        private static string BuildNotificationUrl(Guid id, string phanLoai)
        {
            return $"/Manages/Notification/Show?Id={id}&PhanLoai={phanLoai}";
        }
    }
}
