using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Payola.Intranet.Models.Binders;
using Payola.Intranet.Models.Validators;

namespace Payola.Intranet
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters (GlobalFilterCollection filters)
        {
            filters.Add (new HandleErrorAttribute ());
        }

        public static void RegisterRoutes (RouteCollection routes)
        {
            routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");

            routes.MapRoute (
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }); // Parameter defaults
        }

        protected void Application_Start ()
        {
            AreaRegistration.RegisterAllAreas ();
            RegisterGlobalFilters (GlobalFilters.Filters);
            RegisterRoutes (RouteTable.Routes);

            // Decimal binder to handle culture specific decimal format.
            ModelBinders.Binders.DefaultBinder = new LocalizedDefaultModelBinder ();
            ModelBinders.Binders.Add (typeof (decimal), new DecimalModelBinder ());

            LocalizedDataAnnotationsValidator.RegisterAdapters ();
        }
    }
}