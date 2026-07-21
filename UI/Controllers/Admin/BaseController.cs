using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UI.Helper;

namespace UI.Controllers.Admin
{
    [Authorize]
    public class BaseController : Controller
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.ModelState.Clear();
            await base.OnActionExecutionAsync(context, next);
        }

        protected JsonResult RenderValidationResult(bool isValid, string message, object model, string strViewPath = "")
        {
            return Json(new
            {
                isValid,
                message,
                html = isValid && string.IsNullOrEmpty(strViewPath)
                    ? string.Empty
                    : StaticViewRenderHelper.RenderRazorViewToString(this, strViewPath, model)
            });
        }

        protected JsonResult ReturnJson(bool isValid, string message, object? data = null, Dictionary<string, string>? errors = null)
        {
            return Json(new
            {
                isValid,
                message,
                data,
                errors
            });
        }
    }
}
