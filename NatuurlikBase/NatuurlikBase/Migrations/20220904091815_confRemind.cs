using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatuurlikBase.Migrations
{
    public partial class confRemind : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfirmationReminderId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfirmationReminder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Days = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationReminder", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_ConfirmationReminder_ConfirmationReminderId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "ConfirmationReminder");

            migrationBuilder.DropIndex(
                name: "IX_Order_ConfirmationReminderId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ConfirmationReminderId",
                table: "Order");
        }
    }
}
