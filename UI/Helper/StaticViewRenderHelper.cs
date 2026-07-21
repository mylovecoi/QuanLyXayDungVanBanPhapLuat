using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UI.Helper
{
    public static class StaticViewRenderHelper
    {
        public static string RenderRazorViewToString(Controller controller, string viewName, object model)
        {
            try
            {
                controller.ViewData.Model = model;
                using (var sw = new StringWriter())
                {
                    IViewEngine? viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                    ViewEngineResult viewResult = viewEngine!.GetView("", viewName, false);

                    if (!viewResult.Success || viewResult.View == null)
                    {
                        viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                        if (!viewResult.Success || viewResult.View == null)
                        {
                            throw new InvalidOperationException($"Không tìm thấy view: {viewName}");
                        }
                    }

                    ViewContext viewContext = new ViewContext(
                            controller.ControllerContext,
                            viewResult.View,
                            controller.ViewData,
                            controller.TempData,
                            sw,
                            new HtmlHelperOptions());
                    viewResult.View.RenderAsync(viewContext);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi render view {viewName}: {ex.Message}");
                return string.Empty; // Tránh crash ứng dụng
            }
        }

        public static async Task<string> RenderPartialViewToStringAsync(Controller controller, string viewPath, object model)
        {
            controller.ViewData.Model = model;

            using var writer = new StringWriter();

            var viewEngine = controller.HttpContext.RequestServices
                .GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            var viewResult = viewEngine!.GetView(null, viewPath, false);

            if (!viewResult.Success || viewResult.View == null)
            {
                viewResult = viewEngine.FindView(controller.ControllerContext, viewPath, false);
                if (!viewResult.Success || viewResult.View == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy view: {viewPath}");
                }
            }

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
        }
    }
}