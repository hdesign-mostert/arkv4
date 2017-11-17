using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models
{
    public class HtmlAttribute
    {
        public static List<string> CommaSeparatedAttributes = new List<string>()
        {
            "data-bind"
        };

        public HtmlAttribute(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
