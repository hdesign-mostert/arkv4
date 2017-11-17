using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ark.Models
{
    public class EnquirySpam
    {
        public int EnquirySpamID { get; set; }
        public string EmailAddress { get; set; }
        public string IpAddress { get; set; }
        public string Domain { get; set; }
    }
}
