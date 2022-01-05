namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MapppingModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MCQQuestions", "Mark", c => c.Int(nullable: false));
            AddColumn("dbo.MCQQuestions", "IsMCQ", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MCQQuestions", "IsMCQ");
            DropColumn("dbo.MCQQuestions", "Mark");
        }
    }
}
