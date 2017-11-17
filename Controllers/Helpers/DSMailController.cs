using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Ark.App_Code
{
    public static class DSMailController
    {
        public static bool SendMail(string recipient, string subject, string email, string body,string name)
        {
            string template = DSMailTemplateController.GetMailTemplate("generic");

            string emailFrom = WebConfigurationManager.AppSettings["EnquiryEmail"];
            string projectName = WebConfigurationManager.AppSettings["EnquiryLnE"];

            template = template.Replace("{0}", name).Replace("{1}", email).Replace("{2}", subject).Replace("{3}", body);

            bool useSmtp = string.IsNullOrEmpty(WebConfigurationManager.AppSettings["SmtpHost"]);

            if (!useSmtp)
            {
                MailController.SendMail(emailFrom, recipient, "", "", subject, template);

                return true;
            }
            else
            {
                int leadID = LnE.LeadManager.SendLead(recipient, emailFrom, "", "", subject, template, projectName);

                if (leadID > 0)
                    return true;
                else
                    return false;

            }
        }

    }
}
