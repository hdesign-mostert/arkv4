using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ark.Models
{
    public class NavLinkControl
    {
        public MvcHtmlString LinkClass { get; set; }
        public MvcHtmlString SpanClass { get; set; }
        public MvcHtmlString Label { get; set; }
        public MvcHtmlString Href { get; set; }
        public bool IsActive { get; set; }
    }
}
