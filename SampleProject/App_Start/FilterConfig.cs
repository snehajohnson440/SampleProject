using System.Web;
using System.Web.Mvc;
using SampleProject.Filters;

namespace SampleProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new UserLoggingFilter());
        }
    }
}
