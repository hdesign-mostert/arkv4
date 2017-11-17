using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ark.Models
{
    public partial class Article
    {
        public int ArticleID { get; set; }
        [Display(Name="Category")]
        public int CategoryID { get; set; }
        [Required]
        public string Subject { get; set; }
        [UIHint("RichTextEditor")]
        [Required]
        public string Content { get; set; }
        [Required]
        public string Author { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        [Required]
        public string Summary { get; set; }

        [NotMapped]
        public int ArticleImageID { get; set; }
        [NotMapped]
        public string Category { get; set; }

        [NotMapped]
        public string Tags { get; set; }

        [NotMapped]
        public string PublishClass { get; set; }
        [NotMapped]
        public string ReadableDate { get; set; }
        [NotMapped]
        public string ArticleUrl { get; set; }
    }
}
