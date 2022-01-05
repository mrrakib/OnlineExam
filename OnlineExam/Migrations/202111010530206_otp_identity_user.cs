namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class otp_identity_user : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Otp", c => c.String());
            AddColumn("dbo.Users", "OtpKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "OtpKey");
            DropColumn("dbo.Users", "Otp");
        }
    }
}
