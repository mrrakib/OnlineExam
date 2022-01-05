namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExplanationIsAnswers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MCQQuestionOptions", "IsAnswer", c => c.Boolean(nullable: false));
            AddColumn("dbo.MCQQuestions", "Explanation", c => c.String());
            DropColumn("dbo.MCQQuestionOptions", "Answer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MCQQuestionOptions", "Answer", c => c.Boolean(nullable: false));
            DropColumn("dbo.MCQQuestions", "Explanation");
            DropColumn("dbo.MCQQuestionOptions", "IsAnswer");
        }
    }
}
