using System.Web.Optimization;

namespace SDO
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.cookie-1.4.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/KENDO/js/kendo.all.min.js",
                "~/Scripts/KENDO/extend/kendo.*"));

            bundles.Add(new ScriptBundle("~/bundles/sdo").Include("~/Scripts/SDO/SDO.*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/themes/base/jquery").Include("~/Content/themes/base/jquery-ui.min.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/bootstrap-grid.min.css",
                "~/Content/bootstrap-reboot.min.css"));

            bundles.Add(new StyleBundle("~/Content/styles/kendo").Include(
                "~/Content/styles/kendo.common.min.css",
                "~/Content/styles/kendo.default.min.css"));

            bundles.Add(new StyleBundle("~/Content/basic").Include("~/Content/site.css"));
        }
    }
}
