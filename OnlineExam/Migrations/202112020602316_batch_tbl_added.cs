namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class batch_tbl_added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Batches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BatchName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BatchWiseSubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BatchId = c.Int(nullable: false),
                        SubjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.BatchId)
                .Index(t => t.SubjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BatchWiseSubjects", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.BatchWiseSubjects", "BatchId", "dbo.Batches");
            DropIndex("dbo.BatchWiseSubjects", new[] { "SubjectId" });
            DropIndex("dbo.BatchWiseSubjects", new[] { "BatchId" });
            DropTable("dbo.BatchWiseSubjects");
            DropTable("dbo.Batches");
        }
    }
}
