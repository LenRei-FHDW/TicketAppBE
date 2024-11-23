using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketAPI.Migrations
{
    /// <inheritdoc />
    public partial class CategoryDeleteNotCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("c6853ac1-a002-44a1-aeba-7cfaafc5397b"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("2e923105-a9ba-4702-992f-e76b46184601"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("5cd14692-d023-4266-a239-3e208519c2e8"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("4034556e-5a19-43de-ab2f-455c1d4750ac"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("a1f8383f-370b-4974-a3bc-a32986f28124"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[,]
                {
                    { new Guid("10e62659-a95c-4cc7-ab52-765ef6f92891"), "Kinder" },
                    { new Guid("a30e4991-519d-440b-8d85-7847a1c78e51"), "Saisonticket" },
                    { new Guid("ea90e25d-bee3-4b9b-b617-615a8ce5e3e8"), "Familie" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "CreaterId", "Description", "ImageName", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("6d87b994-b71d-4dbc-9afe-00c522b31798"), new Guid("ea90e25d-bee3-4b9b-b617-615a8ce5e3e8"), new DateTime(2024, 11, 23, 22, 4, 42, 98, DateTimeKind.Utc).AddTicks(7941), "user-123", "This is the first sample product.", "sample1.jpg", false, "Sample Product 1", 19.99m },
                    { new Guid("773ab2a8-f252-4db6-b537-28dd1877da59"), new Guid("a30e4991-519d-440b-8d85-7847a1c78e51"), new DateTime(2024, 11, 23, 22, 4, 42, 98, DateTimeKind.Utc).AddTicks(7957), "user-456", "This is the second sample product.", "sample2.jpg", false, "Sample Product 2", 29.99m }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("10e62659-a95c-4cc7-ab52-765ef6f92891"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("6d87b994-b71d-4dbc-9afe-00c522b31798"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("773ab2a8-f252-4db6-b537-28dd1877da59"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("a30e4991-519d-440b-8d85-7847a1c78e51"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("ea90e25d-bee3-4b9b-b617-615a8ce5e3e8"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[,]
                {
                    { new Guid("4034556e-5a19-43de-ab2f-455c1d4750ac"), "Saisonticket" },
                    { new Guid("a1f8383f-370b-4974-a3bc-a32986f28124"), "Familie" },
                    { new Guid("c6853ac1-a002-44a1-aeba-7cfaafc5397b"), "Kinder" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "CreaterId", "Description", "ImageName", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("2e923105-a9ba-4702-992f-e76b46184601"), new Guid("a1f8383f-370b-4974-a3bc-a32986f28124"), new DateTime(2024, 11, 23, 21, 43, 4, 404, DateTimeKind.Utc).AddTicks(4119), "user-123", "This is the first sample product.", "sample1.jpg", false, "Sample Product 1", 19.99m },
                    { new Guid("5cd14692-d023-4266-a239-3e208519c2e8"), new Guid("4034556e-5a19-43de-ab2f-455c1d4750ac"), new DateTime(2024, 11, 23, 21, 43, 4, 404, DateTimeKind.Utc).AddTicks(4129), "user-456", "This is the second sample product.", "sample2.jpg", false, "Sample Product 2", 29.99m }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");
        }
    }
}
