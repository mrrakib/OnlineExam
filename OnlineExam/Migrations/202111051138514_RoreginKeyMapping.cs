namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoreginKeyMapping : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResultSheets", "QuestionId", "dbo.Subjects");
            AddForeignKey("dbo.ResultSheets", "QuestionId", "dbo.MCQQuestions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultSheets", "QuestionId", "dbo.MCQQuestions");
            AddForeignKey("dbo.ResultSheets", "QuestionId", "dbo.Subjects", "Id", cascadeDelete: true);
        }
    }
}
