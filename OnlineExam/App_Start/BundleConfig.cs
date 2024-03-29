﻿using System.Web;
using System.Web.Optimization;

namespace OnlineExam
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            #region Theme css
            bundles.Add(new StyleBundle("~/Content/ltecss").Include(
                      "~/bower_components/bootstrap/dist/css/bootstrap.min.css",
                      "~/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/bower_components/Ionicons/css/ionicons.min.css",
                      "~/dist/css/AdminLTE.min.css",
                      "~/dist/css/skins/_all-skins.min.css",
                      "~/bower_components/morris.js/morris.css",
                      "~/bower_components/jvectormap/jquery-jvectormap.css",
                      "~/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css",
                      "~/bower_components/bootstrap-daterangepicker/daterangepicker.css",
                      "~/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css"));
            #endregion

            #region Theme Js
            bundles.Add(new ScriptBundle("~/bundles/ltejquery").Include(
                        "~/bower_components/jquery/dist/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
                      "~/bower_components/jquery-ui/jquery-ui.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/ltejs").Include(
                      "~/bower_components/bootstrap/dist/js/bootstrap.min.js",
                      "~/bower_components/raphael/raphael.min.js",
                      "~/bower_components/morris.js/morris.min.js",
                      "~/bower_components/jquery-sparkline/dist/jquery.sparkline.min.js",
                      "~/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                      "~/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                      "~/bower_components/jquery-knob/dist/jquery.knob.min.js",
                      "~/bower_components/moment/min/moment.min.js",
                      "~/bower_components/bootstrap-daterangepicker/daterangepicker.js",
                      "~/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                      "~/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                      "~/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                      "~/bower_components/fastclick/lib/fastclick.js",
                      "~/dist/js/adminlte.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/dashboard").Include(
                      "~/dist/js/pages/dashboard.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/ltedemo").Include(
                      "~/dist/js/demo.js"
                      ));
            #endregion

            #region Login CSS
            bundles.Add(new StyleBundle("~/Content/ltelogincss").Include(
                      "~/bower_components/bootstrap/dist/css/bootstrap.min.css",
                      "~/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/bower_components/Ionicons/css/ionicons.min.css",
                      "~/dist/css/AdminLTE.min.css",
                      "~/plugins/iCheck/square/blue.css"));
            #endregion

            #region Login JS
            bundles.Add(new ScriptBundle("~/bundles/loginJS").Include(
                      "~/bower_components/bootstrap/dist/js/bootstrap.min.js",
                      "~/plugins/iCheck/icheck.min.js"));
            #endregion
        }
    }
}
