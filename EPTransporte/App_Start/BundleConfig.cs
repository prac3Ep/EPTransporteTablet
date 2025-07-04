﻿using System.Web.Optimization;

namespace EPTransporte
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

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
    "~/Scripts/DataTables/jquery.dataTables.js",
    "~/Scripts/DataTables/dataTables.bootstrap4.js",
    "~/Scripts/DataTables/dataTables.responsive.js"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                "~/Content/DataTables/css/dataTables.bootstrap4.css",
                "~/Content/DataTables/css/responsive.bootstrap4.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootbox").Include(
                "~/Scripts/bootbox.js"));
        }
    }
}