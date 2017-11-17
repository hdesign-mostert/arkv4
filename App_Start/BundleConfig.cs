using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Ark
{
    public class BundleConfig
    {
         public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/Scale.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.10.4.custom.min.js",
                        "~/Scripts/moment.js"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //            "~/Scripts/Design/bootstrap*"));

            bundles.Add(new ScriptBundle("~/bundles/pickadate").Include(
                        "~/Scripts/pickadate/picker.js",
                         "~/Scripts/pickadate/picker.date.js",
                          "~/Scripts/pickadate/picker.time.js",
                          "~/Scripts/pickadate/dateStartValidation.js"));

            bundles.Add(new StyleBundle("~/Content/pickadate").Include(
                "~/Scripts/pickadate/themes/default.css",
                "~/Scripts/pickadate/themes/default.date.css",
                "~/Scripts/pickadate/themes/default.time.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout.mapping-latest.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/master.css"));

            //bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            //            "~/Content/themes/base/jquery.ui.core.css",
            //            "~/Content/themes/base/jquery.ui.resizable.css",
            //            "~/Content/themes/base/jquery.ui.selectable.css",
            //            "~/Content/themes/base/jquery.ui.accordion.css",
            //            "~/Content/themes/base/jquery.ui.autocomplete.css",
            //            "~/Content/themes/base/jquery.ui.button.css",
            //            "~/Content/themes/base/jquery.ui.dialog.css",
            //            "~/Content/themes/base/jquery.ui.slider.css",
            //            "~/Content/themes/base/jquery.ui.tabs.css",
            //            "~/Content/themes/base/jquery.ui.datepicker.css",
            //            "~/Content/themes/base/jquery.ui.progressbar.css",
            //            "~/Content/themes/base/jquery.ui.theme.css"));

            //bundles.Add(new StyleBundle("~/Content/pickadate").Include(
            //    "~/Content/themes/picker/default.css",
            //    "~/Content/themes/picker/default.date.css",
            //    "~/Content/themes/picker/default.time.css"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/jqueryuploader").Include(
                "~/Scripts/fileupload/main.js",
                "~/Scripts/fileupload/jquery.fileupload.js",
                "~/Scripts/fileupload/jquery.postmessage-transport.js",
                "~/Scripts/fileupload/jquery.xdr-transport.js",
                "~/Scripts/fileupload/tmpl.min.js",
                "~/Scripts/fileupload/design/gridster.js",
                "~/Scripts/fileupload/load-image.min.js",
                "~/Scripts/fileupload/canvas-to-blob.min.js",
                "~/Scripts/fileupload/jquery.fileupload-process.js",
                "~/Scripts/fileupload/jquery.fileupload-image.js",
                "~/Scripts/fileupload/jquery.fileupload-ui.js",
                "~/Scripts/fileupload/jquery.iframe-transport.js"));


            //BundleTable.Bundles.Add(new StyleBundle("~/Styles/base").Include("~/Content/bootstrap.css",
            //    "~/Content/jquery.fileupload-ui-noscript.css",
            //   "~/Content/jquery.fileupload-ui.css"));
        }
    }
}
