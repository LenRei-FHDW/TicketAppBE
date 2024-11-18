using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddExampleProductsTry2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "ApplicationUserId", "Description", "ImageName", "IsDeleted", "Name", "Price", "Rating" },
                values: new object[,]
                {
                    { new Guid("c9721778-5331-4b35-8c09-639f6648a55a"), "beb35cf8-25e7-4370-96f4-583819414643", "This is the second sample product.", "sample2.jpg", false, "Sample Product 2", 29.99m, 4.0m },
                    { new Guid("f65b227e-da5a-4af5-acee-049b87bc96d6"), "beb35cf8-25e7-4370-96f4-583819414643", "This is the first sample product.", "sample1.jpg", false, "Sample Product 1", 19.99m, 4.5m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("c9721778-5331-4b35-8c09-639f6648a55a"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("f65b227e-da5a-4af5-acee-049b87bc96d6"));
        }
    }
}
