using UI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Systems;
using System.Threading.Tasks;

namespace UI.Security
{
    public class AuthorizeActionAttribute : ActionFilterAttribute
    {
        private readonly string _permission; // Thêm tham số quyền
        private readonly string? _controller;
        private readonly string? _action;

        private string MapActionName(string actionName)
        {
            return actionName switch
            {
                "Store" => "Create",
                "Update" => "Edit",
                "Show" => "Index",               
                "Chuyen" => "Approve",
                "Duyet" => "Approve",
                "HuyDuyet" => "Approve",
                "TraLai" => "Approve",
                "TiepNhan" => "Approve",
                "XacNhan" => "Approve",
                "HoanThanh" => "Approve",
                "CongBo" => "Public",
                "HuyCongBo" => "Public",
                _ => actionName
            };
        }
        private string MapPermissionName(string permission)
        {
            return permission switch
            {
                "Store" => "Create",
                "Update" => "Edit",
                "Show" => "Index",
                _ => permission
            };
        }

        public AuthorizeActionAttribute(string permission, string? controller = null, string? action = null)
        {
            _permission = permission;
            _controller = controller;   // Nếu không truyền, sẽ lấy từ ControllerContext
            _action = action;           // Nếu không truyền, sẽ lấy từ ControllerContext
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;

            var roleActionService = httpContext.RequestServices.GetService<IRoleActionService>();
            var userService = httpContext.RequestServices.GetService<IUserService>();
            if (userService == null)
            {
                HandleUnauthorizedRequest(context, "Lỗi hệ thống! Không tìm thấy dịch vụ xác thực người dùng.");
                return;
            }
            var userSession = await userService.IsUserSessionValidAsync();
            if (userSession == null || userSession.Status == "error")
            {
                HandleUnauthorizedRequest(context, userSession?.Message ?? "Lỗi hệ thống! Không tìm thấy dịch vụ xác thực người dùng.");
                return;
            }
            string controllerName = _controller ?? context.RouteData.Values["controller"]?.ToString() ?? "";
            string actionNameRaw = _action ?? context.RouteData.Values["action"]?.ToString() ?? "";
            string actionName = MapActionName(actionNameRaw); // Chuyển đổi tên action nếu cần
            string permission = MapPermissionName(_permission);
            string? role = roleActionService?.GetRoleByControllerAction(controllerName, actionName);
            if (string.IsNullOrEmpty(role) || !FuntionGlobal.CheckPermission(session, role, permission))
            {
                HandleUnauthorizedRequest(context, $"Bạn không có quyền thực hiện chức năng này! Vui lòng liên hệ quản trị viên!");
                return;
            }
            await next();            
        }

        private void HandleUnauthorizedRequest(ActionExecutingContext context, string message)
        {
            // Kiểm tra nếu action trả về JsonResult thì trả JSON thay vì ViewResult
            var returnType = (context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)?.MethodInfo.ReturnType;
            bool isJsonResult = returnType != null &&
                                (returnType == typeof(JsonResult) || returnType == typeof(Task<JsonResult>) ||
                                 returnType == typeof(object) || returnType == typeof(Task<object>));
            if (isJsonResult)
            {
                context.Result = new JsonResult(new { status = "error", message = message });

            }
            else
            {
                // Mặc định trả về View lỗi
                context.Result = GetErrorView(message, "Home", "Index");
            }
        }
        private ViewResult GetErrorView(string message, string controller, string action)
        {
            return new ViewResult
            {
                ViewName = "Views/Shared/Error.cshtml",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                    new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                    new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary()
                )
                {
                    ["Messages"] = message,
                    ["Controller"] = controller,
                    ["Action"] = action
                }
            };
        }
    }
}
