using Azure.Core;
using DataAccess;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Systems
{
    public interface IRoleActionService
    {
        Task<CommonResponse> GetRolesAsync(string Search, int PageSize = 5, int PageCurrent = 1);
        Task<List<RoleAction>> GetAllRolesAsync();
        string GetMenuActiveByControllerAction(string? controller, string? action);
        string GetRoleByControllerAction(string? controller, string? action);
        string GetTitleByControllerAction(string? controller, string? action);
        string GetTableNameByControllerAction(string? controller, string? action);
        Task<CommonResponse> StoreAsync(RoleAction request);
        Task<CommonResponse> EditAsync(Guid guid);
        Task<CommonResponse> UpdateAsync(RoleAction request);
        Task<bool> CheckDuplicateAsync(string role, Guid guid);
        Task<CommonResponse> DeleteAsync(Guid guid);
        Task<CommonResponse> GetRoleActionInfoAsync(Guid guid);
        Task<int> GetSTTSapXep(Guid guidGroup);
    }

    public class RoleActionService : IRoleActionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public RoleActionService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<CommonResponse> GetRolesAsync(string Search, int PageSize, int PageCurrent)
        {
            var query = _dbContext.RoleActions.OrderBy(t => t.Level).ThenBy(t => t.STTSapXep).AsQueryable();

            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(t => t.Role.Contains(Search) || (t.Title != null && t.Title.Contains(Search)));
            }

            // Lấy tất cả các Role khớp với tìm kiếm
            var filteredRoles = await query.AsNoTracking().ToListAsync();

            if (!filteredRoles.Any()) return new CommonResponse
            {
                Status = "success",
                Data = new List<RoleAction>(),
                TotalRecord = 0

            }; // Không có gì khớp thì return ngay

            // Lấy danh sách ID của các role khớp với tìm kiếm
            var roleIds = filteredRoles.Select(r => r.Id).ToHashSet();

            // Lấy toàn bộ danh sách Role để xử lý quan hệ cha - con
            var allRoles = await _dbContext.RoleActions.AsNoTracking()
                                                       .OrderBy(t => t.Level)
                                                       .ThenBy(t => t.STTSapXep)
                                                       .ToListAsync();

            var sortedList = new List<RoleAction>();
            var visited = new HashSet<Guid>(); // Tránh vòng lặp vô hạn

            // Duyệt từng role khớp với tìm kiếm, sau đó lấy các cấp dưới
            foreach (var role in filteredRoles)
            {
                if (visited.Add(role.Id))
                {
                    sortedList.Add(role); // Thêm chính nó
                    GetAllChildren(role.Id, allRoles, sortedList, visited);
                }
            }
            var dataView = sortedList.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToList();
            return new CommonResponse { Status = "success", Data = dataView, TotalRecord = sortedList.Count() };
        }

        public async Task<List<RoleAction>> GetAllRolesAsync()
        {
            var query = _dbContext.RoleActions.Where(t => t.Status == "Kích hoạt").OrderBy(t => t.Level).ThenBy(t => t.STTSapXep).AsQueryable();


            // Lấy tất cả các Role khớp với tìm kiếm
            var filteredRoles = await query.AsNoTracking().ToListAsync();

            if (!filteredRoles.Any()) return new List<RoleAction>(); // Không có gì khớp thì return ngay

            // Lấy danh sách ID của các role khớp với tìm kiếm
            var roleIds = filteredRoles.Select(r => r.Id).ToHashSet();

            // Lấy toàn bộ danh sách Role để xử lý quan hệ cha - con
            var allRoles = await _dbContext.RoleActions.AsNoTracking()
                                                       .Where(t => t.Status == "Kích hoạt")
                                                       .OrderBy(t => t.Level)
                                                       .ThenBy(t => t.STTSapXep)
                                                       .ToListAsync();

            var sortedList = new List<RoleAction>();
            var visited = new HashSet<Guid>(); // Tránh vòng lặp vô hạn

            // Duyệt từng role khớp với tìm kiếm, sau đó lấy các cấp dưới
            foreach (var role in filteredRoles)
            {
                if (visited.Add(role.Id))
                {
                    sortedList.Add(role); // Thêm chính nó
                    GetAllChildren(role.Id, allRoles, sortedList, visited);
                }
            }
            return sortedList;
        }

        private void GetAllChildren(Guid parentId, List<RoleAction> allRoles, List<RoleAction> sortedList, HashSet<Guid> visited)
        {
            var children = allRoles.Where(t => t.RoleGroupId == parentId).OrderBy(t => t.STTSapXep).ToList();

            foreach (var child in children)
            {
                if (visited.Add(child.Id)) // Tránh lặp
                {
                    sortedList.Add(child);
                    GetAllChildren(child.Id, allRoles, sortedList, visited);
                }
            }
        }

        /// <summary>
        /// Lấy mã nghề (MaNghe) từ request hiện tại.
        /// Hỗ trợ cả trường hợp truy vấn trực tiếp từ Query/Route/Form hoặc truy vấn Db qua hoSoId/Mahs/MaHoSo.
        /// </summary>
        private string? GetCurrentMaNghe()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            // 1. Lấy trực tiếp từ Query string (ví dụ: ?MaNghe=...)
            if (httpContext.Request.Query.TryGetValue("MaNghe", out var queryMaNghe))
            {
                return queryMaNghe.ToString();
            }

            // 2. Lấy từ Route values
            if (httpContext.Request.RouteValues.TryGetValue("MaNghe", out var routeMaNghe))
            {
                return routeMaNghe?.ToString();
            }

            // 3. Lấy từ Form data (dành cho POST request)
            if (httpContext.Request.HasFormContentType && httpContext.Request.Form.TryGetValue("MaNghe", out var formMaNghe))
            {
                return formMaNghe.ToString();
            }

            // 4. Nếu không có MaNghe trực tiếp, thử tìm khóa chính hoSoId trong tham số để query DB lấy MaNghe
            string? hoSoIdStr = null;
            if (httpContext.Request.Query.TryGetValue("hoSoId", out var queryHoSoId))
            {
                hoSoIdStr = queryHoSoId.ToString();
            }
            else if (httpContext.Request.RouteValues.TryGetValue("hoSoId", out var routeHoSoId))
            {
                hoSoIdStr = routeHoSoId?.ToString();
            }
            else if (httpContext.Request.HasFormContentType && httpContext.Request.Form.TryGetValue("hoSoId", out var formHoSoId))
            {
                hoSoIdStr = formHoSoId.ToString();
            }

            if (!string.IsNullOrEmpty(hoSoIdStr) && Guid.TryParse(hoSoIdStr, out var hoSoId))
            {
                var dinhGia = _dbContext.DinhGias.FirstOrDefault(t => t.Id == hoSoId);
                if (dinhGia != null)
                {
                    return dinhGia.MaNghe;
                }
            }

            // 5. Thử tìm theo mã hồ sơ (Mahs hoặc MaHoSo) để query DB lấy MaNghe
            string? mahs = null;
            if (httpContext.Request.Query.TryGetValue("Mahs", out var qMahs)) mahs = qMahs.ToString();
            else if (httpContext.Request.RouteValues.TryGetValue("Mahs", out var rMahs)) mahs = rMahs?.ToString();
            else if (httpContext.Request.Query.TryGetValue("MaHoSo", out var qMaHoSo)) mahs = qMaHoSo.ToString();
            else if (httpContext.Request.RouteValues.TryGetValue("MaHoSo", out var rMaHoSo)) mahs = rMaHoSo?.ToString();

            if (!string.IsNullOrEmpty(mahs))
            {
                var dinhGia = _dbContext.DinhGias.FirstOrDefault(t => t.MaHoSo == mahs);
                if (dinhGia != null)
                {
                    return dinhGia.MaNghe;
                }
            }

            return null;
        }

        /// <summary>
        /// Lấy thông tin Permission khớp với Controller, Action và MaNghe (Parameter).
        /// Giải quyết trường hợp nhiều chức năng dùng chung Controller/Action nhưng khác tham số MaNghe.
        /// </summary>
        private Permission? GetMatchedPermission(IEnumerable<Permission> permissions, string? controller, string? action)
        {
            if (permissions == null || !permissions.Any()) return null;

            string? maNghe = GetCurrentMaNghe();
            if (!string.IsNullOrEmpty(maNghe))
            {
                // Ưu tiên so khớp chính xác cả Controller, Action và Parameter trùng với MaNghe
                var matched = permissions.FirstOrDefault(t => t.Controller == controller && t.Action == action && t.Parameter == maNghe);
                if (matched != null) return matched;
            }

            // Fallback: Nếu không tìm thấy hoặc không có MaNghe, lấy bản ghi trùng Controller & Action đầu tiên
            return permissions.FirstOrDefault(t => t.Controller == controller && t.Action == action);
        }

        public string GetMenuActiveByControllerAction(string? controller, string? action)
        {
            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action)) return "menu_home";
            if (action is "Create" or "Edit" or "Store" or "Update") action = "Index";

            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString("Permissions");
            var menuData = string.IsNullOrEmpty(sessionData) ? Enumerable.Empty<Permission>()
                : JsonConvert.DeserializeObject<IEnumerable<Permission>>(sessionData) ?? Enumerable.Empty<Permission>();

            return GetMatchedPermission(menuData, controller, action)?.MenuActive ?? "menu_home";
        }

        public string GetRoleByControllerAction(string? controller, string? action)
        {
            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action)) return string.Empty;
            if (action is "Create" or "Edit" or "Delete" or "Approve" or "Public") action = "Index";

            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString("Permissions");
            var rolesData = string.IsNullOrEmpty(sessionData) ? Enumerable.Empty<Permission>()
                : JsonConvert.DeserializeObject<IEnumerable<Permission>>(sessionData) ?? Enumerable.Empty<Permission>();

            return GetMatchedPermission(rolesData, controller, action)?.Role ?? string.Empty;
        }

        public string GetTitleByControllerAction(string? controller, string? action)
        {
            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action)) return string.Empty;
            if (action is "Create" or "Edit" or "Show") action = "Index";

            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString("Permissions");
            var titleData = string.IsNullOrEmpty(sessionData) ? Enumerable.Empty<Permission>()
                : JsonConvert.DeserializeObject<IEnumerable<Permission>>(sessionData) ?? Enumerable.Empty<Permission>();

            return GetMatchedPermission(titleData, controller, action)?.Title ?? string.Empty;
        }

        public string GetTableNameByControllerAction(string? controller, string? action)
        {
            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action)) return string.Empty;
            if (action is "Create" or "Edit") action = "Index";

            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString("Permissions");
            var titleData = string.IsNullOrEmpty(sessionData) ? Enumerable.Empty<Permission>()
                : JsonConvert.DeserializeObject<IEnumerable<Permission>>(sessionData) ?? Enumerable.Empty<Permission>();

            return GetMatchedPermission(titleData, controller, action)?.Table ?? string.Empty;
        }

        public async Task<CommonResponse> StoreAsync(RoleAction request)
        {
            try
            {
                await _dbContext.RoleActions.AddAsync(request);
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
                var model = await _dbContext.RoleActions.FirstOrDefaultAsync(t => t.Id == guid);
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

        public async Task<CommonResponse> UpdateAsync(RoleAction request)
        {
            try
            {
                var model = await _dbContext.RoleActions.FindAsync(request.Id);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
                }
                model.Role = request.Role;
                model.Title = request.Title;
                model.PhanLoai = request.PhanLoai;
                model.STTSapXep = request.STTSapXep;
                model.Controller = request.PhanLoai == "Detail" ? request.Controller : "";
                model.Action = request.PhanLoai == "Detail" ? request.Action : "";
                model.Table = request.PhanLoai == "Detail" ? request.Table : "";
                model.Parameter = request.Parameter;
                model.UseGroup = request.UseGroup;
                model.Status = request.Status;
                model.Icon = request.Icon;
                _dbContext.RoleActions.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };

            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<bool> CheckDuplicateAsync(string role, Guid guid)
        {
            return await _dbContext.RoleActions.AnyAsync(t => t.Role == role && t.Id != guid);
        }

        public async Task<CommonResponse> DeleteAsync(Guid guid)
        {
            try
            {
                var model = await _dbContext.RoleActions.FindAsync(guid);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
                }
                _dbContext.RoleActions.Remove(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };

            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<int> GetSTTSapXep(Guid guidGroup)
        {
            bool exists = await _dbContext.RoleActions.AnyAsync(t => t.RoleGroupId == guidGroup);

            if (!exists)
            {
                return 1; // No existing RoleActions, return 1
            }

            // Count the number of RoleActions asynchronously
            int count = await _dbContext.RoleActions.CountAsync(t => t.RoleGroupId == guidGroup);
            return count + 1;
        }

        public async Task<CommonResponse> GetRoleActionInfoAsync(Guid guid)
        {
            var data = await _dbContext.RoleActions.FindAsync(guid);
            if (data == null)
            {
                return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
            }
            return new CommonResponse { Status = "success", Data = data };
        }
    }
}
