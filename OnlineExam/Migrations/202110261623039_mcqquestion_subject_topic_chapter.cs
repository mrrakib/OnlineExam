namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mcqquestion_subject_topic_chapter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chapters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectId = c.Int(nullable: false),
                        ChapterName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subjects", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MCQQuestionOptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MCQQuestionId = c.Int(nullable: false),
                        OptionName = c.String(nullable: false),
                        Answer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MCQQuestions", t => t.MCQQuestionId, cascadeDelete: true)
                .Index(t => t.MCQQuestionId);
            
            CreateTable(
                "dbo.MCQQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionName = c.String(nullable: false),
                        TopicId = c.Int(nullable: false),
                        ChapterId = c.Int(nullable: false),
                        SubjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chapters", t => t.ChapterId, cascadeDelete: false)
                .ForeignKey("dbo.Subjects", t => t.SubjectId, cascadeDelete: false)
                .ForeignKey("dbo.Topics", t => t.TopicId, cascadeDelete: false)
                .Index(t => t.TopicId)
                .Index(t => t.ChapterId)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.Topics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChapterId = c.Int(nullable: false),
                        TopicName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chapters", t => t.ChapterId, cascadeDelete: true)
                .Index(t => t.ChapterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MCQQuestions", "TopicId", "dbo.Topics");
            DropForeignKey("dbo.Topics", "ChapterId", "dbo.Chapters");
            DropForeignKey("dbo.MCQQuestions", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.MCQQuestionOptions", "MCQQuestionId", "dbo.MCQQuestions");
            DropForeignKey("dbo.MCQQuestions", "ChapterId", "dbo.Chapters");
            DropForeignKey("dbo.Chapters", "SubjectId", "dbo.Subjects");
            DropIndex("dbo.Topics", new[] { "ChapterId" });
            DropIndex("dbo.MCQQuestions", new[] { "SubjectId" });
            DropIndex("dbo.MCQQuestions", new[] { "ChapterId" });
            DropIndex("dbo.MCQQuestions", new[] { "TopicId" });
            DropIndex("dbo.MCQQuestionOptions", new[] { "MCQQuestionId" });
            DropIndex("dbo.Chapters", new[] { "SubjectId" });
            DropTable("dbo.Topics");
            DropTable("dbo.MCQQuestions");
            DropTable("dbo.MCQQuestionOptions");
            DropTable("dbo.Subjects");
            DropTable("dbo.Chapters");
        }
    }
}
