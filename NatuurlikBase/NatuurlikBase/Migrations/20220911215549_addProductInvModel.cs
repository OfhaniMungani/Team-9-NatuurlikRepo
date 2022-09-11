using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatuurlikBase.Migrations
{
    public partial class addProductInvModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductInventory_InventoryItem_InventoryItemId",
                table: "ProductInventory");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInventory_Products_ProductId",
                table: "ProductInventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductInventory",
                table: "ProductInventory");

            migrationBuilder.RenameTable(
                name: "ProductInventory",
                newName: "ProductConfiguration");

            migrationBuilder.RenameIndex(
                name: "IX_ProductInventory_InventoryItemId",
                table: "ProductConfiguration",
                newName: "IX_ProductConfiguration_InventoryItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductConfiguration",
                table: "ProductConfiguration",
                columns: new[] { "ProductId", "InventoryItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConfiguration_InventoryItem_InventoryItemId",
                table: "ProductConfiguration",
                column: "InventoryItemId",
                principalTable: "InventoryItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConfiguration_Products_ProductId",
                table: "ProductConfiguration",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductConfiguration_InventoryItem_InventoryItemId",
                table: "ProductConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConfiguration_Products_ProductId",
                table: "ProductConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductConfiguration",
                table: "ProductConfiguration");

            migrationBuilder.RenameTable(
                name: "ProductConfiguration",
                newName: "ProductInventory");

            migrationBuilder.RenameIndex(
                name: "IX_ProductConfiguration_InventoryItemId",
                table: "ProductInventory",
                newName: "IX_ProductInventory_InventoryItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductInventory",
                table: "ProductInventory",
                columns: new[] { "ProductId", "InventoryItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInventory_InventoryItem_InventoryItemId",
                table: "ProductInventory",
                column: "InventoryItemId",
                principalTable: "InventoryItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInventory_Products_ProductId",
                table: "ProductInventory",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
