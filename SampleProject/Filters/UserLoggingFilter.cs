using NLog;
using System;
using System.Web.Mvc;

namespace SampleProject.Filters
{
    public class UserLoggingFilter : IActionFilter
    {
       
        private const string ScopeKey = "NLogScope";

        
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var username =
                filterContext.HttpContext.User?.Identity?.IsAuthenticated == true
                ? filterContext.HttpContext.User.Identity.Name
                    .Replace("\\", "_")
                    .Replace("/", "_")
                : "Anonymous";

           
            var scope = ScopeContext.PushProperty("Username", username);
            filterContext.HttpContext.Items[ScopeKey] = scope;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            if (filterContext.HttpContext.Items[ScopeKey] is IDisposable scope)
            {
                scope.Dispose();
            }
        }
    }
}