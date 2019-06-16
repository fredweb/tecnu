using System.Web;
using System.Web.Optimization;

namespace XNuvem.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/font-awesome-4.4.0/css/font-awesome.css",
                      "~/Content/bootstrap.css",
                      "~/Content/plugins/select2/select2.min.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/imputmask")
               .Include("~/Scripts/jquery.inputmask.bundle.js",
               "~/Scripts/inputmask/inputmask.date.extensions.js",
               "~/Scripts/inputmask/inputmask.extensions.js",
               "~/Scripts/inputmask/inputmask.numeric.extensions.js"));

            bundles.Add(new StyleBundle("~/css/app").Include(                      
                      "~/Content/dist/css/AdminLTE.css",
                      "~/Content/dist/css/skins/_all-skins.css",
                      "~/Content/Site.css"
                      ));
            
            bundles.Add(new StyleBundle("~/css/dataTable").Include(
                "~/Content/plugins/datatables-1.10/datatables.min.css",
                "~/Content/plugins/datatables-1.10/Responsive-2.0.2/css/responsive.dataTables.min.css",
                "~/Content/plugins/datatables-1.10/DataTables-1.10.11/css/dataTables.bootstrap.min.css",
                "~/Content/plugins/datatables-1.10/Responsive-2.0.2/css/responsive.bootstrap.min.css",
                "~/Content/plugins/datatables-1.10/RowReorder-1.1.1/css/rowReorder.bootstrap.min.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/adminlte").Include(
                      "~/Content/plugins/slimScroll/jquery.slimscroll.min.js",
                      "~/Content/plugins/fastclick/fastclick.min.js",
                      "~/Content/dist/js/app.min.js"));

            bundles.Add(new ScriptBundle("~/js/dataTable").Include(
                "~/Content/plugins/datatables-1.10/datatables.min.js",
                "~/Content/plugins/datatables-1.10/Responsive-2.0.2/js/dataTables.responsive.min.js",
                "~/Content/plugins/datatables-1.10/Responsive-2.0.2/js/responsive.bootstrap.min.js",
                "~/Scripts/xnuvem-dt-1.0.1.js"
                ));
        }
    }
}
