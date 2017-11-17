using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ark.Models
{
    public partial class ArticleCategory
    {
        public int ArticleCategoryID { get; set; }
        public int ParentCategoryID { get; set; }
        [Required]
        public string Name { get; set; }

        [NotMapped]
        public int ArticleCount { get; set; }
    }
}
