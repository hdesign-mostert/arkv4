using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Ark.Models
{
    [NotMapped]
    public class ListModel
    {
        public object List { get; set; }
        public int Total { get; set; }
    }
}
