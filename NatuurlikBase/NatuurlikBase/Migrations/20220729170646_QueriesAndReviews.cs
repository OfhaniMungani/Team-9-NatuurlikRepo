using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatuurlikBase.Migrations
{
    public partial class QueriesAndReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderQueryDescription",
                table: "OrderQuery",
                newName: "Description");

            migrationBuilder.CreateTable(
                name: "OrderReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ReviewReasonId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    LoggedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderReview_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderReview_ReviewReason_ReviewReasonId",
                        column: x => x.ReviewReasonId,
                        principalTable: "ReviewReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderReview_OrderId",
                table: "OrderReview",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReview_ReviewReasonId",
                table: "OrderReview",
                column: "ReviewReasonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderReview");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "OrderQuery",
                newName: "OrderQueryDescription");
        }
    }
}
