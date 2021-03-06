namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temuseradded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        MobileNo = c.String(),
                        Password = c.String(),
                        Otp = c.String(),
                        OtpKey = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempUsers");
        }
    }
}
