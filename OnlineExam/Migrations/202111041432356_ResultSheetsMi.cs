namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResultSheetsMi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResultSheets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        StudentId = c.Int(nullable: false),
                        ActualMark = c.Double(nullable: false),
                        ObtainMark = c.Double(nullable: false),
                        CorrectOptionCount = c.Int(nullable: false),
                        IsMCQ = c.Boolean(nullable: false),
                        ExamCount = c.Int(nullable: false),
                        ExamDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subjects", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultSheets", "QuestionId", "dbo.Subjects");
            DropIndex("dbo.ResultSheets", new[] { "QuestionId" });
            DropTable("dbo.ResultSheets");
        }
    }
}
