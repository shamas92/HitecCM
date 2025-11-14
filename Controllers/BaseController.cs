using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HitecCM.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if user is logged in
            if (Session["UserID"] == null)
            {
                // Redirect to login page if not authenticated
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                        { "controller", "Account" },
                        { "action", "Login" }
                    });
                return;
            }

            // Check if user is DSA and trying to access restricted pages
            var role = Session["Role"] as string;
            var controller = filterContext.RouteData.Values["controller"]?.ToString();
            var action = filterContext.RouteData.Values["action"]?.ToString();

            // DSA role restrictions
            if (string.Equals(role, "DSA", StringComparison.OrdinalIgnoreCase))
            {
                // Check if trying to access Home/Index
                if (string.Equals(controller, "Home", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase))
                {
                    // Redirect DSA users from Home/Index to Events/Index
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary {
                            { "controller", "Events" },
                            { "action", "Index" }
                        });
                    return;
                }

                // Check if accessing other restricted controllers
                bool isAllowedController = string.Equals(controller, "Events", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(controller, "EventPhotos", StringComparison.OrdinalIgnoreCase);

                if (!isAllowedController)
                {
                    // Redirect to Access Denied page
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/AccessDenied.cshtml"
                    };
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}