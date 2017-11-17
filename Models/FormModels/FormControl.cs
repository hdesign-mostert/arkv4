using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ark.Models
{
    public class FormControl
    {
        public string Label { get; set; }
        public string DisplayName { get; set; }
        public List<HtmlAttribute> Attributes { get; set; }
        public string HtmlName { get; set; }
        public IDictionary<string, object> UnobtrusiveValidators { get; set; }

        public FormControl()
        {
            Attributes = new List<HtmlAttribute>();
        }

        public MvcHtmlString AttributeString
        {
            get
            {
                if (Attributes == null)
                    return new MvcHtmlString("");

                Dictionary<string, string> attributes = new Dictionary<string, string>();

                foreach (Ark.Models.HtmlAttribute attribute in this.Attributes)
                {
                    if (attributes.ContainsKey(attribute.Name))
                    {
                        string s = "";
                        if (!string.IsNullOrEmpty(attribute.Value) && HtmlAttribute.CommaSeparatedAttributes.Contains(attribute.Name))
                            s += ",";

                        s += " " + attribute.Value;

                        if (s != "")
                            attributes[attribute.Name] += s;
                    }
                    else
                    {
                        attributes.Add(attribute.Name, attribute.Value);
                    }
                }

                string def = "";

                foreach (var attr in attributes)
                {
                    def += attr.Key + "= \"" + attr.Value + "\" ";
                }

                return new MvcHtmlString(def);
            }
        }

        public string HtmlID
        {
            get
            {
                return Label.Replace(" ", "").ToLower();
            }
        }

        public string VariableName
        {
            get
            {
                return Label.Replace(" ", "").Replace("/","");
            }
        }

        public MvcHtmlString UnobtrusiveAttributeString
        {
            get
            {
                if (UnobtrusiveValidators == null || UnobtrusiveValidators.Count == 0)
                    return new MvcHtmlString("");

                StringBuilder sb = new StringBuilder();

                foreach(string key in UnobtrusiveValidators.Keys)
                {
                    sb.Append(key);
                    sb.Append("=\"");
                    sb.Append(UnobtrusiveValidators[key]);
                    sb.Append("\"");
                }

                return new MvcHtmlString(sb.ToString());
            }
        }
    }
}
