namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswerSheetsExamId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnswerSheets", "ExamId", c => c.Int());
            CreateIndex("dbo.AnswerSheets", "ExamId");
            AddForeignKey("dbo.AnswerSheets", "ExamId", "dbo.Exams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnswerSheets", "ExamId", "dbo.Exams");
            DropIndex("dbo.AnswerSheets", new[] { "ExamId" });
            DropColumn("dbo.AnswerSheets", "ExamId");
        }
    }
}
