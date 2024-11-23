using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesAfterDrop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("78283e2a-2fd7-4a70-996e-7c3c671d02c2"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("56bb280e-d9af-459b-a060-16f77f27e4b1"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("80db26dd-675c-48cf-8d53-fdc48edd3d22"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("9900f295-c3e9-40e5-91ff-4f80ba6f8252"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("e041449b-48f1-4377-b1da-181ebecc948a"));

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    { new Guid("78283e2a-2fd7-4a70-996e-7c3c671d02c2"), "Kinder" },
                    { new Guid("9900f295-c3e9-40e5-91ff-4f80ba6f8252"), "Familie" },
                    { new Guid("e041449b-48f1-4377-b1da-181ebecc948a"), "Saisonticket" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "CreaterId", "Description", "ImageName", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("56bb280e-d9af-459b-a060-16f77f27e4b1"), new Guid("e041449b-48f1-4377-b1da-181ebecc948a"), new DateTime(2024, 11, 23, 21, 41, 47, 615, DateTimeKind.Utc).AddTicks(2532), "user-456", "This is the second sample product.", "sample2.jpg", false, "Sample Product 2", 29.99m },
                    { new Guid("80db26dd-675c-48cf-8d53-fdc48edd3d22"), new Guid("9900f295-c3e9-40e5-91ff-4f80ba6f8252"), new DateTime(2024, 11, 23, 21, 41, 47, 615, DateTimeKind.Utc).AddTicks(2512), "user-123", "This is the first sample product.", "sample1.jpg", false, "Sample Product 1", 19.99m }
                });
        }
    }
}
