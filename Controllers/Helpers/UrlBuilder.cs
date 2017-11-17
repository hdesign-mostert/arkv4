using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ark.Controllers.Helpers
{
    public static class UrlBuilder
    {
        public static string ResolveUrl(string relativeContentPath)
        {
            Uri contextUri = HttpContext.Current.Request.Url;

            var baseUri = string.Format("{0}://{1}{2}", contextUri.Scheme, contextUri.Host, contextUri.Port == 80 ? string.Empty : ":" + contextUri.Port);

            return string.Format("{0}{1}", baseUri, VirtualPathUtility.ToAbsolute(relativeContentPath));
        }

        public static string CleanUrl(string url)
        {
            if (String.IsNullOrEmpty(url))
                return String.Empty;

            StringBuilder builder = new StringBuilder();

            foreach (Char ch in url)
            {
                if (Char.IsLetterOrDigit(ch))
                    builder.Append(ch);
                else
                    builder.Append("-");
            }

            string s = builder.ToString().Trim('-').ToLower();

            while (s.Contains("--"))
            {
                s = s.Replace("--", "-");
            }

            return s;
        }


    }
}
