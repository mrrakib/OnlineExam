namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adding_exam_feature : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExamName = c.String(maxLength: 150),
                        BatchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .Index(t => t.BatchId);
            
            CreateTable(
                "dbo.ExamWiseQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExamId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exams", t => t.ExamId, cascadeDelete: true)
                .ForeignKey("dbo.MCQQuestions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.ExamId)
                .Index(t => t.QuestionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExamWiseQuestions", "QuestionId", "dbo.MCQQuestions");
            DropForeignKey("dbo.ExamWiseQuestions", "ExamId", "dbo.Exams");
            DropForeignKey("dbo.Exams", "BatchId", "dbo.Batches");
            DropIndex("dbo.ExamWiseQuestions", new[] { "QuestionId" });
            DropIndex("dbo.ExamWiseQuestions", new[] { "ExamId" });
            DropIndex("dbo.Exams", new[] { "BatchId" });
            DropTable("dbo.ExamWiseQuestions");
            DropTable("dbo.Exams");
        }
    }
}
