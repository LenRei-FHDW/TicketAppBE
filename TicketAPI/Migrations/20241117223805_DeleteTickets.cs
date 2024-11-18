using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketAPI.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("055f1d69-40bd-4292-8fce-d7e0160c345b"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("6354a870-e836-4731-8717-b44d95fc7e0a"));

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CreaterId", "Description", "ImageName", "IsDeleted", "Name", "Price", "Rating" },
                values: new object[,]
                {
                    { new Guid("40cbb04c-9909-4f8f-b05f-a4531965d8aa"), "user-123", "This is the first sample product.", "sample1.jpg", false, "Sample Product 1", 19.99m, 4.5m },
                    { new Guid("c4da8540-079d-4aa4-9b69-e4a91f1fc14a"), "user-456", "This is the second sample product.", "sample2.jpg", false, "Sample Product 2", 29.99m, 4.0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("40cbb04c-9909-4f8f-b05f-a4531965d8aa"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("c4da8540-079d-4aa4-9b69-e4a91f1fc14a"));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Products",
                type: "varchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CreaterId", "Description", "Discriminator", "ImageName", "IsDeleted", "Name", "Price", "Rating" },
                values: new object[,]
                {
                    { new Guid("055f1d69-40bd-4292-8fce-d7e0160c345b"), "user-456", "This is the second sample product.", "Product", "sample2.jpg", false, "Sample Product 2", 29.99m, 4.0m },
                    { new Guid("6354a870-e836-4731-8717-b44d95fc7e0a"), "user-123", "This is the first sample product.", "Product", "sample1.jpg", false, "Sample Product 1", 19.99m, 4.5m }
                });
        }
    }
}
