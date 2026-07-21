using DataAccess;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Systems
{
    public interface IUserService
    {
        Task<CommonResponse> GetUsersAsync(string Search, int PageSize, int PageCurrent, string Level = "");
        Task<CommonResponse> StoreAsync(User request);
        Task<CommonResponse> EditAsync(Guid guid);
        Task<CommonResponse> UpdateAsync(User request);
        Task<CommonResponse> DeleteAsync(Guid guid);
        Task<CommonResponse> ResetPasswordAsync(Guid guid);
        Task<CommonResponse> DuplicateAsync(Guid guidDuplicate, string username, string name, string email);
        Task<User> GetUserByUserNamePasswordAsync(string username, string password);

        Task<CommonResponse> ActiveAsync(Guid guid, string status);
        Task<bool> IsUserlExitAsync(string username, string email);
        Task<bool> IsUserMaillExitAsync(string username, string email);
        Task<CommonResponse> GetUserByUserEmailAsync(string username, string email);
        Task<CommonResponse> IsUserSessionValidAsync();
    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly OTPService _otpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext dbContext, OTPService oTPService, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _otpService = oTPService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> GetUsersAsync(string Search, int PageSize, int PageCurrent, string Level = "")
        {
            var data = _dbContext.Users.Where(t => !t.SSA);
            if (!string.IsNullOrEmpty(Search))
            {
                data = data.Where(t => t.Username.Contains(Search) || t.Name.Contains(Search));
            }
            if (!string.IsNullOrEmpty(Level))
            {
                data = data.Where(t => t.Level == Level);
            }
            var dataView = await data.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToListAsync();
            return new CommonResponse { Status = "success", Data = dataView, TotalRecord = data.Count() };
        }

        public async Task<CommonResponse> StoreAsync(User request)
        {
            try
            {
                if (await _dbContext.Users.AnyAsync(t => t.Username == request.Username || t.Email == request.Email))
                {
                    return new CommonResponse { Status = "error", Message = "Username và Email đã được sử dụng!" };
                }
                request.Password = Helper.BCryptHash(request.Password);
                request.OTPSecretKey = _otpService.GenerateSecretKey();
                _dbContext.Users.Add(request);
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
                var data = await _dbContext.Users.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin" };
                }
                return new CommonResponse { Status = "success", Data = data };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra! Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> UpdateAsync(User request)
        {
            try
            {
                var model = _dbContext.Users.Find(request.Id);
                if (model == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                model.Name = request.Name;
                model.Status = request.Status;
                model.Email = request.Email;
                model.FirstLogin = request.FirstLogin;
                model.GroupPermissionId = request.GroupPermissionId;
                model.Content = request.Content;
                if (!string.IsNullOrEmpty(request.Password))
                {
                    model.Password = Helper.BCryptHash(request.Password);
                }
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid guid)
        {
            try
            {
                var model = _dbContext.Users.Find(guid);
                if (model == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                _dbContext.Remove(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> ResetPasswordAsync(Guid guid)
        {
            try
            {
                var model = _dbContext.Users.FirstOrDefault(t => t.Id == guid);
                if (model == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                model.Password = Helper.GetSystemDefaultPassword();
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> ActiveAsync(Guid guid, string status)
        {
            try
            {
                var model = _dbContext.Users.FirstOrDefault(t => t.Id == guid);
                if (model == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };
                model.Status = status;
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> DuplicateAsync(Guid guidDuplicate, string username, string name, string email)
        {
            try
            {
                var model = _dbContext.Users.FirstOrDefault(t => t.Id == guidDuplicate);
                if (model == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu" };

                var newUser = new User
                {
                    Username = username,
                    Name = name,
                    Email = email,
                    Password = Helper.GetSystemDefaultPassword(),
                    SSA = model.SSA,
                    DanhMucDonViId = model.DanhMucDonViId,
                    OTPSecretKey = _otpService.GenerateSecretKey(),
                    Status = model.Status,
                    FirstLogin = true,
                    LoginCount = 0,
                    Content = model.Content,
                    Menu = model.Menu,
                    Theme = model.Theme,
                    GroupPermissionId = model.GroupPermissionId,

                };
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public async Task<User> GetUserByUserNamePasswordAsync(string username, string password)
        {
            var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == username);
            if (model == null || !BCrypt.Net.BCrypt.Verify(password, model.Password))
                return new User
                { Id = Guid.Empty, Name = "Unknown", Username = "Unknown", Email = "Unknown", Password = "Unknown", OTPSecretKey = "Unknown", Status = "Unknown" };

            return model;
        }

        public async Task<bool> IsUserlExitAsync(string username, string email)
        {
            return await _dbContext.Users.AnyAsync(t =>
                                            t.Username == username || t.Email == email);
        }

        public async Task<bool> IsUserMaillExitAsync(string username, string email)
        {
            return await _dbContext.Users.AnyAsync(t =>
                                            t.Username == username && t.Email == email);
        }

        public async Task<CommonResponse> GetUserByUserEmailAsync(string username, string email)
        {
            var data = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == username && t.Email == email);
            if (data == null)
            {
                return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin tài khoản" };
            }
            return new CommonResponse { Status = "success", Data = data };

        }

        public async Task<CommonResponse> IsUserSessionValidAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null || string.IsNullOrEmpty(httpContext.Session.GetString("SsAdmin")))
            {
                return new CommonResponse { Status = "error", Message = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!" };
            }
            try
            {
                var sessionData = httpContext.Session.GetString("SsAdmin");
                if (string.IsNullOrEmpty(sessionData))
                {
                    return new CommonResponse { Status = "error", Message = "Bạn đã kết thúc phiên làm việc! Bạn cần đăng nhập để tiếp tục!" };
                }
                var userSession = JsonConvert.DeserializeObject<User>(sessionData);
                if (userSession == null || string.IsNullOrEmpty(userSession.Username) || string.IsNullOrEmpty(userSession.Password))
                {
                    return new CommonResponse { Status = "error", Message = "Thông tin đăng nhập không hợp lệ!" };
                }
                var data = await _dbContext.Users.Where(t => t.Username == userSession.Username)
                                .Select(t => new { t.Password, t.Status })
                                .FirstOrDefaultAsync();
                if (data == null || data.Password != userSession.Password)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin tài khoản! Liên hệ quản trị viên!" };
                }
                if (data.Status == "Lock")
                {
                    return new CommonResponse { Status = "error", Message = "Tài khoản đã bị khóa! Liên hệ quản trị viên!" };
                }
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }
    }
}