using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ark.Models
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleCategory> Categories { get; set; }
        public DbSet<ArticleImage> ArticleImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<TempImage> TempImages { get; set; }
    }
}
