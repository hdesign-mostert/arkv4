using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ark.Models
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<EnquiryCategory> EnquiryCategories { get; set; }
        public DbSet<EnquirySpam> EnquirySpam { get; set; }
    }
}
