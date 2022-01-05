namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userActiveAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsActive");
        }
    }
}
