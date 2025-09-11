using System.Web;
using System.Web.Optimization;

namespace HitecCM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/js/bundle.js",
                        "~/Content/js/scripts.js",
                        "~/Content/js/charts/gd-analytics.js",
                        "~/Content/js/libs/jqvmap.js",
                        "~/Content/js/crop-functionality.js"
        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/dashlite.css",
                      "~/Content/css/theme.css",
                      "~/Content/css/crop-modal.css"));
        }
    }
}
