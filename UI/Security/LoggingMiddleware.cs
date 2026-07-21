using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Systems;
using System.Text;

namespace UI.Security
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;


        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var logService = context.RequestServices.GetRequiredService<ILogService>();
            //Xóa bớt log
            await logService.Clean();

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var url = context.Request.Path.HasValue && context.Request.Path != "/"
                    ? context.Request.Path.Value
                    : "Trang chủ";
            var method = context.Request.Method;

            var username = context.User.Identity?.IsAuthenticated == true
                       ? context.User.Identity.Name
                       : "Anonymous"; // Nếu chưa đăng nhập, gán "Anonymous"
            Guid userId = Guid.TryParse(context.User.FindFirst("UserId")?.Value, out var parsedId)
                        ? parsedId
                        : Guid.Empty; // Nếu chưa đăng nhập, gán Guid.Empty

            // Lấy tên Controller & Action từ RouteData
            var routeValues = context.GetRouteData()?.Values;
            var controllerName = routeValues?["controller"]?.ToString() ?? "Unknown";
            var actionName = routeValues?["action"]?.ToString() ?? "Unknown";

            if (userId == Guid.Empty)
            {
                await _next(context);
                return;
            }

            if (string.IsNullOrEmpty(controllerName))
            {
                await _next(context);
                return;
            }
            string[] excludedPaths = { "assets", "FileUpload", "lib" };
            if (excludedPaths.Any(path => url.Contains(path, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Nếu là POST, lưu thêm nội dung body (chỉ khi cần thiết)
            string? requestBody = null;
            context.Request.EnableBuffering(); // Cho phép đọc lại body                   
            if (method == "POST" && context.Request.ContentLength > 0)
            {
                if (!controllerName.Equals("Auth", StringComparison.OrdinalIgnoreCase))
                {
                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }
                    context.Request.Body.Position = 0; // Reset để request không bị mất dữ liệu
                }
            }
            else if (method == "GET" && context.Request.QueryString.HasValue)
            {
                requestBody = context.Request.QueryString.Value;
            }

            string actionDescription = "";
            if (controllerName.Contains("KeKhai", StringComparison.OrdinalIgnoreCase) || controllerName.Contains("DoanhNghiep", StringComparison.OrdinalIgnoreCase))
            {
                actionDescription = "[Kê khai đăng ký giá] ";
            }
            else if (controllerName.Contains("DinhGia", StringComparison.OrdinalIgnoreCase))
            {
                actionDescription = "[Định giá] ";
            }
            else if (controllerName.Contains("GiaThiTruong", StringComparison.OrdinalIgnoreCase))
            {
                actionDescription = "[Giá thị trường] ";
            }

            if (!string.IsNullOrEmpty(actionDescription))
            {
                string maNghe = context.Request.Query["MaNghe"].ToString();
                if (string.IsNullOrEmpty(maNghe) && routeValues != null && routeValues.ContainsKey("MaNghe"))
                {
                    maNghe = routeValues["MaNghe"]?.ToString() ?? "";
                }
                if (string.IsNullOrEmpty(maNghe) && !string.IsNullOrEmpty(requestBody))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(requestBody, @"[?&""']?MaNghe[?&""']?\s*[:=]\s*""?([^""&'\s,}]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        maNghe = match.Groups[1].Value;
                    }
                }

                string tenNghe = "";
                if (!string.IsNullOrEmpty(maNghe))
                {
                    try
                    {
                        var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
                        var dm = dbContext.DanhMucKinhDoanhs.AsNoTracking().FirstOrDefault(x => x.MaNghe == maNghe);
                        if (dm != null)
                        {
                            tenNghe = dm.TenNghe ?? "";
                        }
                    }
                    catch (Exception)
                    {
                        // Bỏ qua lỗi DB nếu có để tránh ảnh hưởng đến luồng request chính
                    }
                }

                string actionNameFriendly = actionName switch
                {
                    "Index" => "Xem danh sách / Trang chủ",
                    "Create" => "Tạo mới",
                    "Store" => "Lưu mới",
                    "Edit" => "Chỉnh sửa",
                    "Update" => "Cập nhật",
                    "Destroy" => "Xóa bỏ",
                    "Delete" => "Xóa bỏ",
                    "Show" => "Xem chi tiết",
                    "XetDuyet" => "Xét duyệt hồ sơ",
                    "Chuyen" => "Chuyển hồ sơ",
                    "TraLai" => "Trả lại hồ sơ",
                    "CongBo" => "Công bố",
                    "HuyCongBo" => "Hủy công bố",
                    _ => actionName
                };
                string ngheSuffix = !string.IsNullOrEmpty(tenNghe) ? $" - Nghề: {tenNghe}" : "";
                actionDescription += $"Thực hiện hành động: {actionNameFriendly}{ngheSuffix} (Chức năng: {controllerName})";
                requestBody = string.IsNullOrEmpty(requestBody) ? actionDescription : $"{actionDescription} | Dữ liệu: {requestBody}";
            }

            var data = new Log
            {
                Username = username,
                IpAddress = ipAddress,
                Url = url,
                Method = method,
                Request = requestBody,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
            };

            // Lưu log vào DB
            await logService.Store(data);

            await _next(context);
        }
    }
}
