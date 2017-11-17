using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ark.Models
{
    public class FormDropdownControl : FormControl
    {
        public IEnumerable<SelectListItem> Items { get; set; }
        public string ID { get; set; }
        public string CssClass { get; set; }
        public MvcHtmlString Validation { get; set; }
        public int Count { get; set; }
        public string Message { get; set; }
        public string Value { get; set; }
    }
}
