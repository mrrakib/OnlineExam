namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialUserDesign : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ResultSheets", "StudentId", c => c.String());
            AlterColumn("dbo.ResultSummaries", "StudentId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResultSummaries", "StudentId", c => c.Int(nullable: false));
            AlterColumn("dbo.ResultSheets", "StudentId", c => c.Int(nullable: false));
        }
    }
}
