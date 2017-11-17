using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ark.App_Code
{
    public static class DSMailTemplateController
    {

        public static string ApplicationPath
        {
            get;
            set;
        }
        public static string GetMailTemplate(string name)
        {

            if (string.IsNullOrEmpty(ApplicationPath) && HttpContext.Current != null)
            {
                ApplicationPath = HttpContext.Current.Request.PhysicalApplicationPath;
            }
            if (!name.ToLower().Contains(".htm"))
                name += ".html";

            string path = ApplicationPath;
            path = Path.Combine(path, "templates");
            path = Path.Combine(path, name);
            string templateText = File.ReadAllText(path);

            return templateText;
        }

        public static string InitMasterWithTemplate(string title, string name, string footer)
        {
            string masterTemplate = DSMailTemplateController.GetMasterTemplate();

            string templ = DSMailTemplateController.GetMailTemplate(name);
            string template = masterTemplate.Replace("{0}", title).Replace("{2}", footer).Replace("{1}", templ);

            return template;
        }

        public static string GetMasterTemplate()
        {
            string path = ApplicationPath;
            path = Path.Combine(path, "templates");
            path = Path.Combine(path, "master.html");

            string masterText = File.ReadAllText(path);

            return masterText;
        }
    }
}
