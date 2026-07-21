using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Services.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UI.Security
{
    public class SessionManagementMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionManagementMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity != null && context.User.Identity.IsAuthenticated)
            {
                using (var scope = context.RequestServices.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

                    var username = context.User.FindFirst(ClaimTypes.Name)?.Value;
                    var password = context.User.FindFirst("MaDangNhap")?.Value;
                    var model = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                    if (model != null)
                    {
                        // Lấy thông tin xác thực thông qua IAuthenticationService
                        var authResult = await authService.AuthenticateAsync(context, CookieAuthenticationDefaults.AuthenticationScheme);
                        var expiresUtc = authResult?.Properties?.ExpiresUtc;

                        if (expiresUtc.HasValue && expiresUtc.Value > DateTimeOffset.UtcNow)
                        {
                            // Kiểm tra session có sẵn chưa
                            if (!context.Session.IsAvailable || !context.Session.Keys.Contains("SsAdmin"))
                            {
                                var sessionService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                                sessionService.SetUserInfo(model);
                                await sessionService.SetPermission(model);
                            }
                        }
                        else
                        {
                            // Cookie đã hết hạn - Đăng xuất
                            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            context.Response.Redirect("/Auth/Login");
                            return;
                        }
                    }
                    else
                    {
                        // Không tìm thấy người dùng, đăng xuất
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Response.Redirect("/Auth/Login");
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
