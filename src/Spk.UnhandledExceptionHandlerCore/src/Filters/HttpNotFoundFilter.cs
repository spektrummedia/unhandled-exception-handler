using System.Web.Mvc;

namespace Spk.UnhandledExceptionHandlerCore.Filters
{
    public class HttpNotFoundFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result.GetType() != typeof(HttpNotFoundResult))
                return;

            filterContext.HttpContext.ClearError();
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 404;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            filterContext.HttpContext.Response.TransmitFile("~/Views/Error/NotFound.html");
        }
    }
}