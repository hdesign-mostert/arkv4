namespace Ark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Contact : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Enquiry",
                c => new
                    {
                        EnquiryID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(),
                        EnquiryCategoryID = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        IPAddress = c.String(),
                        IsSpam = c.Boolean(nullable: false),
                        TelephoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.EnquiryID);

            CreateTable(
                "dbo.EnquiryCategory",
                c => new
                    {
                        EnquiryCategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        To = c.String(),
                    })
                .PrimaryKey(t => t.EnquiryCategoryID);

            CreateTable(
                "dbo.EnquirySpam",
                c => new
                    {
                        EnquirySpamID = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(),
                        IpAddress = c.String(),
                        Domain = c.String(),
                    })
                .PrimaryKey(t => t.EnquirySpamID);

        }

        public override void Down()
        {
            DropTable("dbo.EnquirySpam");
            DropTable("dbo.EnquiryCategory");
            DropTable("dbo.Enquiry");
        }
    }
}
