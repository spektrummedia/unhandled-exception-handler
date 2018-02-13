using System.Web.Mvc;
using Spk.UnhandledExceptionHandlerCore.Filters;

namespace $rootnamespace$
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HttpNotFoundFilter { Order = 1 });
        }
    }
}