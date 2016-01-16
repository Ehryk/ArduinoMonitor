using System.Web;
using System.Web.Optimization;

namespace ArduinoWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-datatables").Include(
                        "~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-flot").Include(
                        "~/Scripts/flot/jquery.flot.js",
                        "~/Scripts/flot/jquery.flot.resize.js",
                        "~/Scripts/flot/jquery.flot.axislabels.js",
                        "~/Scripts/flot/jquery.flot.time.js",
                        "~/Scripts/flot/jquery.flot.crosshair.js"));

            bundles.Add(new ScriptBundle("~/bundles/utilities").Include(
                        "~/Scripts/utilities.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/DataTables/css/css").Include(
                      "~/Content/DataTables/css/jquery.dataTables.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
