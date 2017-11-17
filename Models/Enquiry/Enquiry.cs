using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ark.Models
{
    public class Enquiry
    {
        public int EnquiryID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid")]
        [RequiredIf("TelephoneNumber", ErrorMessage = "Telephone number required if Email is omitted")]
        public string Email { get; set; }

        [Display(Name = "Enquiry Related To")]
        public int EnquiryCategoryID { get; set; }

        public DateTime CreateDate { get; set; }

        public string IPAddress { get; set; }

        public bool IsSpam { get; set; }

        [Display(Name = "Telephone Number")]
        [RegularExpression(@"\+{0,1}[\d-()\s]{9,18}", ErrorMessage = "Entered phone format is not valid.")]
        [RequiredIf("Email", ErrorMessage = "Email required if Telephone number is omitted")]
        public string TelephoneNumber { get; set; }
    }
}
