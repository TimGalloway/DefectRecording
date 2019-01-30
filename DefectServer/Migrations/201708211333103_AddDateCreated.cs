namespace DefectServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Defects", "DateCreated", c => c.DateTime());
            AddColumn("dbo.Defects", "DateModified", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Defects", "DateModified");
            DropColumn("dbo.Defects", "DateCreated");
        }
    }
}
