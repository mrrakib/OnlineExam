namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswerOptionWithQuestionForeginKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnswerSheets", "IsMCQ", c => c.Boolean(nullable: false));
            CreateIndex("dbo.AnswerSheets", "QuestionId");
            AddForeignKey("dbo.AnswerSheets", "QuestionId", "dbo.MCQQuestions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnswerSheets", "QuestionId", "dbo.MCQQuestions");
            DropIndex("dbo.AnswerSheets", new[] { "QuestionId" });
            DropColumn("dbo.AnswerSheets", "IsMCQ");
        }
    }
}
