namespace StoreComplete.WebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ititial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "AmmoutPurchased", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "AmmoutPurchased");
        }
    }
}
