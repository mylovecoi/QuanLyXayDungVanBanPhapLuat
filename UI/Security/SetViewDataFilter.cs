using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Services.Systems;

namespace UI.Security
{
    public class SetViewDataFilter : ActionFilterAttribute
    {
        private readonly string? _controller;
        private readonly string? _action;
        public SetViewDataFilter(string? controller = null, string? action = null)
        {
            _controller = controller;
            _action = action;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = _controller ?? context.RouteData.Values["controller"]?.ToString();
            var actionName = _action ?? context.RouteData.Values["action"]?.ToString();

            if (string.IsNullOrEmpty(controllerName) && string.IsNullOrEmpty(actionName)) return;

            var _roleActionService = context.HttpContext.RequestServices.GetService<IRoleActionService>();
            if (_roleActionService == null) return;

            var menuActive = _roleActionService.GetMenuActiveByControllerAction(controllerName, actionName);
            var role = _roleActionService.GetRoleByControllerAction(controllerName, actionName);
            var title = _roleActionService.GetTitleByControllerAction(controllerName, actionName);
            var tableName = _roleActionService.GetTableNameByControllerAction(controllerName, actionName);

            var controller = context.Controller as Controller;
            if (controller != null)
            {
                controller.ViewData["Title"] = title;
                controller.ViewData["MenuActive"] = menuActive;
                controller.ViewData["Role"] = role;
                controller.ViewData["TableName"] = tableName;
            }

        }
    }
}
