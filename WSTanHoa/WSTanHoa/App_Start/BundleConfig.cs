using System.Web;
using System.Web.Optimization;

namespace WSTanHoa
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/cssNOTMASTER").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Lib/lightbox2.2.11.3/css/lightbox.css",
                      "~/Lib/bootstrap-datepicker-1.9.0/css/bootstrap-datepicker.css"));

            bundles.Add(new StyleBundle("~/Content/cssNOTMASTER2").Include(
                      "~/Content/bootstrap.css",
                      "~/Lib/lightbox2.2.11.3/css/lightbox.css",
                      "~/Lib/bootstrap-datepicker-1.9.0/css/bootstrap-datepicker.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Template/SBAdmin2/css/sb-admin-2.css",
                     "~/Content/Site.css",
                     "~/Lib/lightbox2.2.11.3/css/lightbox.css",
                     "~/Lib/bootstrap-datepicker-1.9.0/css/bootstrap-datepicker.css"));

            bundles.Add(new StyleBundle("~/Content/cssDoanTCT2022").Include(
                     "~/Content/bootstrap.css",
                     "~/Lib/lightbox2.2.11.3/css/lightbox.css",
                     "~/Lib/bootstrap-datepicker-1.9.0/css/bootstrap-datepicker.css"));

            bundles.Add(new ScriptBundle("~/bundles/zalo").Include(
                      "~/Scripts/zalo.js",
                      "~/Scripts/zalochat.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Lib/bootstrap-datepicker-1.9.0/js/bootstrap-datepicker.js",
                      "~/Lib/bootstrap-datepicker-1.9.0/locales/bootstrap-datepicker.vi.min.js",
                      "~/Scripts/globalscript.js"));

            bundles.Add(new ScriptBundle("~/bundles/lightbox").Include(
                     "~/Lib/lightbox2.2.11.3/js/lightbox-plus-jquery.js"));
        }
    }
}
