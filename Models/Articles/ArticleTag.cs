using System;
using System.Collections.Generic;

namespace Ark.Models
{
    public partial class ArticleTag
    {
        public int ArticleTagID { get; set; }
        public virtual Article Article { get; set; }
        public int ArticleID { get; set; }
        public virtual Tag Tag { get; set; }
        public int TagID { get; set; }
    }
}
