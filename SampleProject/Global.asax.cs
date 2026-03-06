using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SampleProject.Security;
using SampleProject.DAL;
using NLog;
namespace SampleProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            logger.Info("Application Started");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SeedManagers();
            SeedAdmin();
        }
        private void SeedManagers()
        {
            try
            {
                AuthenticationDAL dal = new AuthenticationDAL();
                dal.SeedManagers();
            }
            catch { }
        }
        private void SeedAdmin()
        {
            try
            {
                AuthenticationDAL dal = new AuthenticationDAL();
                dal.SeedAdmin();
            }
            catch { }
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var cookie = Request.Cookies["jwt"];

            if (cookie == null)
                return;

            try
            {
                var principal =
                    SampleProject.Security.JwtHelper
                        .ValidateToken(cookie.Value);

                HttpContext.Current.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;
            }
            catch
            {
                // invalid token 
            }
        }
        protected void Application_EndRequest()
        {
            if (Response.StatusCode == 401)
            {
                Response.ClearContent();
                Response.Redirect("~/Account/Login");
            }
        }
    }
}
