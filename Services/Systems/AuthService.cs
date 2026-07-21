using Microsoft.AspNetCore.Http;
using DataAccess.Entities.Systems;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using DataAccess.Entities.KeKhaiDangKyGia;
using Newtonsoft.Json;
using System.Text;
using DataAccess;
using Services.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace Services.Systems
{
    public interface IAuthService
    {
        Task<CommonResponse> CheckIsUser(string username, string password);
        bool CheckOTP(string username, string password, string otp, long? clientUnixTimestamp = null);
        Task Sigin(string username, string password);
        void SetUserInfo(User request);
        Task SetPermission(User request);
        User? GetUserInfo();
        string GenerateToken(string username, string password);
        string GenerateTokenForAgent(string agentId);
        Task LogoutAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly IRoleActionService _roleActionService;
        private readonly IPermissionService _permissionService;
        private readonly OTPService _otpService;
        private readonly IConfiguration _configuration;

        public AuthService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext,
                            IRoleActionService roleActionService, IPermissionService permissionService, OTPService otpService, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _roleActionService = roleActionService;
            _permissionService = permissionService;
            _otpService = otpService;
            _configuration = configuration;
        }

        public async Task<CommonResponse> CheckIsUser(string username, string password)
        {
            try
            {
                var systemInfo = await _dbContext.SystemInfo.OrderBy(t => t.Id).FirstOrDefaultAsync();
                int loginLimit = systemInfo?.LoginLock ?? 5;
                var model = await _dbContext.Users.FirstOrDefaultAsync(t => t.Username == username || t.Email == username);

                if (model == null)
                {
                    return new CommonResponse("error", "Tài khoản và mật khẩu không đúng!");
                }
                if (model.Status == "Khóa")
                {
                    return new CommonResponse("error", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên!");
                }
                if (model.Status == "Chờ kích hoạt")
                {
                    return new CommonResponse("error", "Tài khoản của bạn chưa được kích hoạt. Vui lòng liên hệ Quản trị viên!");
                }
                if (!BCrypt.Net.BCrypt.Verify(password, model.Password))
                {
                    model.LoginCount++; // Cập nhật số lần đăng nhập sai

                    if (model.LoginCount >= loginLimit)
                    {
                        model.Status = "Khóa"; // Khóa tài khoản nếu vượt quá giới hạn
                        return new CommonResponse("error", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên!");
                    }

                    _dbContext.Users.Update(model);
                    await _dbContext.SaveChangesAsync();
                    return new CommonResponse("error", "Tài khoản và mật khẩu không đúng!");

                }
                return new CommonResponse("success", "Đăng nhập thành công!");
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }

        public bool CheckOTP(string username, string password, string otp, long? clientUnixTimestamp = null)
        {
            try
            {
                var model = _dbContext.Users.FirstOrDefault(t => t.Username == username && t.Status == "Kích hoạt");
                if (model == null || !BCrypt.Net.BCrypt.Verify(password, model.Password)) return false;
                // Nếu clientUnixTimestamp null thì lấy giờ server
                var timestamp = clientUnixTimestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return _otpService.ValidateOtp(model.OTPSecretKey, otp, timestamp);
            }
            catch
            {
                return false;
            }
        }

        public void SetUserInfo(User request)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return; // Exit if HttpContext is not available

            // Serialize and store model in session
            var jsonString = JsonConvert.SerializeObject(request);
            context.Session.SetString("SsAdmin", jsonString); // Sử dụng SetString thay vì Set để tránh lỗi byte[]
        }

        public async Task SetPermission(User request)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return; // Exit if HttpContext is not available

            List<Permission> data = new List<Permission>();
            if (request.SSA || request.Level == "Doanh nghiệp")
            {
                var roles = await _roleActionService.GetAllRolesAsync();
                if (roles != null) // Kiểm tra null
                {
                    if (request.Level == "Doanh nghiệp")
                    {
                        roles = roles.Where(r => r.UseGroupList.Contains("DN")).ToList();
                    }
                    foreach (var role in roles)
                    {
                        data.Add(new Permission
                        {
                            PhanLoai = role.PhanLoai,
                            Level = role.Level,
                            STTSapXep = role.STTSapXep,
                            Title = role.Title,
                            Role = role.Role,
                            MenuActive = role.Role.Replace(".", "_"),
                            RoleActionId = role.Id,
                            RoleActionGroupId = role.RoleGroupId,
                            Controller = role.Controller,
                            Action = role.Action,
                            Parameter = role.Parameter,
                            Table = role.Table,
                            Icon = role.Icon,
                            Index = true,
                            Create = true,
                            Edit = true,
                            Delete = true,
                            Approve = true,
                            Public = true,
                        });
                    }
                }
            }
            else
            {
                var roles = await _permissionService.GetAllPermissionsByGroupIdAsync(request.GroupPermissionId);
                data.AddRange(roles.Data);
            }

            // Lấy danh sách ngành nghề động
            var dataKinhDoanhAll = await _dbContext.DanhMucKinhDoanhs
                .Where(t => t.TheoDoi == "TD" && t.LoaiGia == "KKG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();

            List<DanhMucKinhDoanh> dataKinhDoanh;
            if (request.Level == "Doanh nghiệp")
            {
                var listMaNghe = await _dbContext.DoanhNghiepLvKds
                    .Where(t => t.DoanhNghiepQuanLyId == request.DoanhNghiepId && t.TrangThai != "CXD")
                    .Select(t => t.MaNghe)
                    .Distinct()
                    .ToListAsync();

                var childItems = dataKinhDoanhAll.Where(t => listMaNghe.Contains(t.MaNghe)).ToList();
                var parentRoles = childItems.Where(t => !string.IsNullOrEmpty(t.RoleGoc)).Select(t => t.RoleGoc).Distinct().ToList();

                dataKinhDoanh = dataKinhDoanhAll.Where(t => 
                    listMaNghe.Contains(t.MaNghe) || 
                    (t.Level == 0 && parentRoles.Contains(t.Role))
                ).ToList();
            }
            else
            {
                dataKinhDoanh = dataKinhDoanhAll;
            }

            var dynamicPermissions = new List<Permission>();
            foreach (var item in data)
            {
                if (item.Role != null && (item.Role.Equals("KeKhaiDangKyGia.ThongTinHoSo", StringComparison.OrdinalIgnoreCase) || item.Role.EndsWith(".KeKhaiDangKyGia.ThongTinHoSo", StringComparison.OrdinalIgnoreCase)))
                {
                    item.PhanLoai = "Group";
                    foreach (var nghe in dataKinhDoanh)
                    {
                        var parentId = item.RoleActionId;
                        if (nghe.Level > 0 && !string.IsNullOrEmpty(nghe.RoleGoc))
                        {
                            var parentNghe = dataKinhDoanh.FirstOrDefault(p => p.Role == nghe.RoleGoc);
                            if (parentNghe != null)
                            {
                                parentId = parentNghe.Id;
                            }
                        }

                        dynamicPermissions.Add(new Permission
                        {
                            GroupPermissionId = item.GroupPermissionId,
                            RoleActionId = nghe.Id,
                            RoleActionGroupId = parentId,
                            Status = item.Status,
                            PhanLoai = nghe.PhanLoai,
                            Level = (item.Level + 1 + nghe.Level), // Điều chỉnh level khớp với cấp phân nhánh
                            STTSapXep = nghe.STTSapXep,
                            Title = nghe.TenNghe,
                            Role = item.Role + "." + nghe.Role,
                            MenuActive = (item.Role + "." + nghe.Role).Replace(".", "_"),
                            Controller = "KeKhaiDangKyGia",
                            Action = "Index",
                            Parameter = nghe.MaNghe,
                            Index = true,
                            Create = true,
                            Edit = true,
                            Delete = true,
                            Approve = true,
                            Public = true,
                        });
                    }
                }
            }
            data.AddRange(dynamicPermissions);

            // Lấy danh sách ngành nghề động cho Định giá
            var dataDinhGia = await _dbContext.DanhMucKinhDoanhs
                .Where(t => t.TheoDoi == "TD" && t.LoaiGia == "DG")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();

            var dynamicDinhGiaPermissions = new List<Permission>();
            foreach (var item in data)
            {
                if (item.Role != null && (item.Role.Equals("DinhGia.ThongTinHoSo", StringComparison.OrdinalIgnoreCase) || item.Role.EndsWith(".DinhGia.ThongTinHoSo", StringComparison.OrdinalIgnoreCase)))
                {
                    item.PhanLoai = "Group";

                    foreach (var nghe in dataDinhGia)
                    {
                        var parentId = item.RoleActionId;
                        if (!string.IsNullOrEmpty(nghe.RoleGoc))
                        {
                            var parentNghe = dataDinhGia.FirstOrDefault(p => p.Role == nghe.RoleGoc);
                            if (parentNghe != null)
                            {
                                parentId = parentNghe.Id;
                            }
                        }

                        int ngheLevel = nghe.Level;
                        if (!string.IsNullOrEmpty(nghe.Role) && nghe.Role.Contains("."))
                        {
                            ngheLevel = nghe.Role.Split('.').Length - 1;
                        }

                        dynamicDinhGiaPermissions.Add(new Permission
                        {
                            GroupPermissionId = item.GroupPermissionId,
                            RoleActionId = nghe.Id,
                            RoleActionGroupId = parentId,
                            Status = item.Status,
                            PhanLoai = nghe.PhanLoai,
                            Level = (item.Level + 1 + ngheLevel),
                            STTSapXep = nghe.STTSapXep,
                            Title = nghe.TenNghe,
                            Role = item.Role + "." + nghe.Role,
                            MenuActive = (item.Role + "." + nghe.Role).Replace(".", "_"),
                            Controller = "DinhGia",
                            Action = "Index",
                            Parameter = nghe.MaNghe,
                            Index = true,
                            Create = true,
                            Edit = true,
                            Delete = true,
                            Approve = true,
                            Public = true,
                        });
                    }
                }
            }
            data.AddRange(dynamicDinhGiaPermissions);

            // Serialize and store model in session
            var jsonString = JsonConvert.SerializeObject(data);
            context.Session.SetString("Permissions", jsonString); // Sử dụng SetString thay vì Set
        }

        public async Task SetCookie(User request)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return; // Exit if HttpContext is not available

            // Create claims and set cookie authentication
            var claims = new List<Claim>{
                                            new Claim(ClaimTypes.Name, request.Username),
                                            new Claim("MaDangNhap", request.Password),
                                            new Claim("UserId", request.Id.ToString())
                                        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }

        public async Task Sigin(string username, string password)
        {

            var model = _dbContext.Users.FirstOrDefault(t => t.Username == username && t.Status == "Kích hoạt");
            if (model != null && BCrypt.Net.BCrypt.Verify(password, model.Password))
            {
                model.LoginCount = 0;
                model.FirstLogin = false;
                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync();
                this.SetUserInfo(model);
                await this.SetPermission(model);
                await this.SetCookie(model); // Thêm await để đảm bảo cookie được set trước khi thoát hàm
            }
        }

        public User? GetUserInfo()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            // Lấy dữ liệu từ session
            if (context.Session.TryGetValue("SsAdmin", out byte[]? bytes))
            {
                // Chuyển byte[] thành JSON string
                var jsonString = System.Text.Encoding.UTF8.GetString(bytes);

                // Deserialize JSON thành object User
                return JsonConvert.DeserializeObject<User>(jsonString);
            }

            return null; // Nếu không tìm thấy session
        }

        public string GenerateToken(string username, string password)
        {
            // Kiểm tra đầu vào, tránh null hoặc chuỗi rỗng
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty.");
            }

            string strToken = "Invalid User";
            var model = _dbContext.Users.FirstOrDefault(t => t.Username == username && t.Status == "Kích hoạt");
            if (model != null && BCrypt.Net.BCrypt.Verify(password, model.Password))
            {
                var secretKey = _configuration["JwtSettings:SecretKey"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                var expires = Convert.ToDouble(_configuration["JwtSettings:Expires"]);

                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    return strToken;
                }
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.Username ?? string.Empty),
                        new Claim("UserId", model.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(expires),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = issuer,
                    Audience = audience
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                strToken = tokenHandler.WriteToken(token);
            }
            return strToken;
        }

        public string GenerateTokenForAgent(string agentId)
        {
            // Tạo token cho agent không cần xác thực người dùng
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expires = Convert.ToDouble(_configuration["JwtSettings:Expires"]);
            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                return "Invalid Token";
            }
            var claims = new[]
            {
                new Claim("AgentId", agentId),
            };
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Session.Clear(); // Xóa toàn bộ Session
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
