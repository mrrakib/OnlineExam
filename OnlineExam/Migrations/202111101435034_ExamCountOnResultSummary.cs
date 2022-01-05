namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExamCountOnResultSummary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultSummaries", "ExamCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResultSummaries", "ExamCount");
        }
    }
}
