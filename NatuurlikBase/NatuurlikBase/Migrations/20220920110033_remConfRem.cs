using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatuurlikBase.Migrations
{
    public partial class remConfRem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_ConfirmationReminder_ConfirmationReminderId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ConfirmationReminderId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ConfirmationReminderId",
                table: "Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfirmationReminderId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_ConfirmationReminderId",
                table: "Order",
                column: "ConfirmationReminderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_ConfirmationReminder_ConfirmationReminderId",
                table: "Order",
                column: "ConfirmationReminderId",
                principalTable: "ConfirmationReminder",
                principalColumn: "Id");
        }
    }
}
