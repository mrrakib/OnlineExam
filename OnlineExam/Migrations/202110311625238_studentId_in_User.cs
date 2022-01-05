namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentId_in_User : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "StudentId", c => c.String());
            AlterColumn("dbo.TempUsers", "UserName", c => c.String(nullable: false));
            AlterColumn("dbo.TempUsers", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.TempUsers", "MobileNo", c => c.String(nullable: false));
            AlterColumn("dbo.TempUsers", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TempUsers", "Password", c => c.String());
            AlterColumn("dbo.TempUsers", "MobileNo", c => c.String());
            AlterColumn("dbo.TempUsers", "Email", c => c.String());
            AlterColumn("dbo.TempUsers", "UserName", c => c.String());
            DropColumn("dbo.Users", "StudentId");
        }
    }
}
