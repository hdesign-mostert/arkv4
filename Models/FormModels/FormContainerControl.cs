using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models
{
    public class FormContainerControl : FormControl
    {

        public string Method { get; set; }
        public string Action { get; set; }
        public bool UseKnockout { get; set; }

        public FormContainerControl(string name, string action)
        {
            this.Label=name;
            this.Action= action;
            this.Method = "POST";
            this.UseKnockout = false;
        }

        public FormContainerControl(string name, string action, string method)
        {
            this.Label=name;
            this.Action= action;
            this.Method = method;
            this.UseKnockout = false;
        }

        public FormContainerControl(string name, string action , bool useKnockout)
        {
            this.Label=name;
            this.Action= action;
            this.Method = "POST";
            this.UseKnockout = useKnockout;
        }

        public FormContainerControl(string name, string action, string method, bool useKnockout)
        {
            this.Label=name;
            this.Action= action;
            this.Method = method;
            this.UseKnockout = useKnockout;
        }
    }
}
