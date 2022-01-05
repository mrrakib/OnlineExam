namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResultSummariesMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResultSummaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        ExamDate = c.DateTime(nullable: false),
                        ActualMark = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ObtainMark = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ObtainMarkPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxObtainMarks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxObtainMarksPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvgObtainMarks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvgObtainMarksPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ResultSummaries");
        }
    }
}
