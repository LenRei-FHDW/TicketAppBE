using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAPI.Migrations
{
    /// <inheritdoc />
    public partial class OrderAndTicketFinalFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_AspNetUsers_ApplicationUserId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_ApplicationUserId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "OrderItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "OrderItem",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ApplicationUserId",
                table: "OrderItem",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_AspNetUsers_ApplicationUserId",
                table: "OrderItem",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
