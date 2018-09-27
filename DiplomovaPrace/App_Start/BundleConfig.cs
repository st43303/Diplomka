using System.Web;
using System.Web.Optimization;

namespace DiplomovaPrace
{
    public class BundleConfig
    {
        // Další informace o sdružování najdete na webu https://go.microsoft.com/fwlink/?LinkId=301862.
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Použijte k vývoji a k získání informací vývojovou verzi produktu Modernizr. Až budete
            // Připraveno na produkční prostředí. Použijte nástroj pro sestavení na webu https://modernizr.com a vyberte jenom testy, které potřebujete.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/main.js",
                "~/Scripts/plugins.js",
                //"~/Scripts/widgets.js",
                //"~/Scripts/dashboard.js",
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/normalize.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/themify-icons.css",
                      "~/Content/flag-icon.min.css",
                      "~/Content/cs-skin-elastic.css",
                      "~/Content/lib/vector-map/jqvmap.min.css",
                      "~/Content/style.css"));
        }
    }
}
