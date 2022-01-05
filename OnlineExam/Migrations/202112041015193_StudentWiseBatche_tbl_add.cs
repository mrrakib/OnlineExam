namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentWiseBatche_tbl_add : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentWiseBatches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BatchId = c.Int(nullable: false),
                        StudentId = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .Index(t => t.BatchId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentWiseBatches", "BatchId", "dbo.Batches");
            DropIndex("dbo.StudentWiseBatches", new[] { "BatchId" });
            DropTable("dbo.StudentWiseBatches");
        }
    }
}
