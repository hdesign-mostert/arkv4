using System;
using System.Collections.Generic;

namespace Ark.Models
{
    public partial class TempImage
    {
        public int TempImageID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public byte[] Data { get; set; }
    }
}
