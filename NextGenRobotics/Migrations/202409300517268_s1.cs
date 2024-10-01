namespace NextGenRobotics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropIndex("dbo.Orders", new[] { "ProductId" });
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailsId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        TotalPrice = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderDetailsId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.Users", "Role", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Products", "Description", c => c.String(maxLength: 500));
            AlterColumn("dbo.Orders", "ShippingAddress", c => c.String(maxLength: 500));
            AlterColumn("dbo.Payments", "UserName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Payments", "CardNo", c => c.String(nullable: false));
            AlterColumn("dbo.Payments", "ExpDate", c => c.String(nullable: false));
            AlterColumn("dbo.Payments", "Address", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Payments", "PaymentMode", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Name", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Users", "Address", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.Orders", "ProductId");
            DropColumn("dbo.Orders", "ShippedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "ShippedDate", c => c.DateTime());
            AddColumn("dbo.Orders", "ProductId", c => c.Int(nullable: false));
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            AlterColumn("dbo.Users", "Address", c => c.String());
            AlterColumn("dbo.Users", "Password", c => c.String());
            AlterColumn("dbo.Users", "UserName", c => c.String());
            AlterColumn("dbo.Users", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Payments", "PaymentMode", c => c.String());
            AlterColumn("dbo.Payments", "Address", c => c.String());
            AlterColumn("dbo.Payments", "ExpDate", c => c.String());
            AlterColumn("dbo.Payments", "CardNo", c => c.String());
            AlterColumn("dbo.Payments", "UserName", c => c.String());
            AlterColumn("dbo.Orders", "ShippingAddress", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Products", "Description", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Users", "Role");
            DropTable("dbo.OrderDetails");
            CreateIndex("dbo.Orders", "ProductId");
            AddForeignKey("dbo.Orders", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
    }
}
