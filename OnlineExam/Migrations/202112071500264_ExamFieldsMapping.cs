namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExamFieldsMapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultSheets", "ExamId", c => c.Int());
            AddColumn("dbo.ResultSummaries", "ExamId", c => c.Int());
            CreateIndex("dbo.ResultSheets", "ExamId");
            CreateIndex("dbo.ResultSummaries", "ExamId");
            AddForeignKey("dbo.ResultSheets", "ExamId", "dbo.Exams", "Id");
            AddForeignKey("dbo.ResultSummaries", "ExamId", "dbo.Exams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultSummaries", "ExamId", "dbo.Exams");
            DropForeignKey("dbo.ResultSheets", "ExamId", "dbo.Exams");
            DropIndex("dbo.ResultSummaries", new[] { "ExamId" });
            DropIndex("dbo.ResultSheets", new[] { "ExamId" });
            DropColumn("dbo.ResultSummaries", "ExamId");
            DropColumn("dbo.ResultSheets", "ExamId");
        }
    }
}
