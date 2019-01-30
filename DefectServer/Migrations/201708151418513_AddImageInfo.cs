namespace DefectServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Defects", "ImageName", c => c.String());
            AddColumn("dbo.Defects", "ImageBase64", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Defects", "ImageBase64");
            DropColumn("dbo.Defects", "ImageName");
        }
    }
}
