namespace Ark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleImage",
                c => new
                    {
                        ArticleImageID = c.Int(nullable: false, identity: true),
                        Extension = c.String(),
                        Data = c.Binary(),
                        ArticleID = c.Int(nullable: false),
                        Filename = c.String(),
                    })
                .PrimaryKey(t => t.ArticleImageID);

            CreateTable(
                "dbo.Article",
                c => new
                    {
                        ArticleID = c.Int(nullable: false, identity: true),
                        CategoryID = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        Author = c.String(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        PublishDate = c.DateTime(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        Summary = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleID);

            CreateTable(
                "dbo.ArticleTag",
                c => new
                    {
                        ArticleTagID = c.Int(nullable: false, identity: true),
                        ArticleID = c.Int(nullable: false),
                        TagID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleTagID)
                .ForeignKey("dbo.Article", t => t.ArticleID, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagID, cascadeDelete: true)
                .Index(t => t.ArticleID)
                .Index(t => t.TagID);

            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        TagID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TagID);

            CreateTable(
                "dbo.ArticleCategory",
                c => new
                    {
                        ArticleCategoryID = c.Int(nullable: false, identity: true),
                        ParentCategoryID = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleCategoryID);

            CreateTable(
                "dbo.webpages_Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);

            CreateTable(
                "dbo.TempImage",
                c => new
                    {
                        TempImageID = c.Int(nullable: false, identity: true),
                        CreateDate = c.DateTime(nullable: false),
                        Filename = c.String(),
                        Extension = c.String(),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.TempImageID);

            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);

            CreateTable(
                "dbo.webpages_UsersInRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId });

        }

        public override void Down()
        {
            DropForeignKey("dbo.ArticleTag", "TagID", "dbo.Tag");
            DropForeignKey("dbo.ArticleTag", "ArticleID", "dbo.Article");
            DropIndex("dbo.ArticleTag", new[] { "TagID" });
            DropIndex("dbo.ArticleTag", new[] { "ArticleID" });
            DropTable("dbo.webpages_UsersInRoles");
            DropTable("dbo.UserProfile");
            DropTable("dbo.TempImage");
            DropTable("dbo.webpages_Roles");
            DropTable("dbo.ArticleCategory");
            DropTable("dbo.Tag");
            DropTable("dbo.ArticleTag");
            DropTable("dbo.Article");
            DropTable("dbo.ArticleImage");
        }
    }
}
