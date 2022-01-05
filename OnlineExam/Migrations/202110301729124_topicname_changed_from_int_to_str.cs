namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class topicname_changed_from_int_to_str : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Topics", "TopicName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Topics", "TopicName", c => c.Int(nullable: false));
        }
    }
}
