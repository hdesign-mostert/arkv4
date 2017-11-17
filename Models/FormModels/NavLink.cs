using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ark.Models
{
    class NavLink
    {
        public MvcHtmlString Class { get; set; }
        public MvcHtmlString Label { get; set; }
        public MvcHtmlString Href { get; set; }
    }
}
