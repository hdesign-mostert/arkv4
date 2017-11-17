using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ark.Models
{
    public partial class ArticleImage
    {
        [Required(ErrorMessage="An Image is required.")]
        public int ArticleImageID { get; set; }
        public string Extension { get; set; }
        [UIHint("FileUpload")]
        public byte[] Data { get; set; }
        public int ArticleID { get; set; }
        public string Filename { get; set; }

        [NotMapped]
        public int TempID { get; set; }
        [NotMapped]
        public int FileCount { get; set; }
    }
}
