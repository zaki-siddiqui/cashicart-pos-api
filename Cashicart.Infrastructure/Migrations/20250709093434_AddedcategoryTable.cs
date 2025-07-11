using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cashicart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedcategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "AuditLogId",
                keyValue: new Guid("d52a6e6a-c10d-4e26-9610-1b8ea8804436"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: new Guid("0d7dd35c-d9da-4399-b7b4-8ed60c91814b"));

            migrationBuilder.DeleteData(
                table: "InventoryAdjustments",
                keyColumn: "InventoryAdjustmentId",
                keyValue: new Guid("1989bd2d-a075-4fde-a58b-2360c98406e4"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("bda81a10-e66b-44b3-ad03-e1b627832cb3"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("c3d5c642-e2ac-422c-a819-25b495893c5d"));

            migrationBuilder.DeleteData(
                table: "TransactionItems",
                keyColumn: "TransactionItemId",
                keyValue: new Guid("8a5d120b-beca-47f0-9e63-12ea0e459ff8"));

            migrationBuilder.DeleteData(
                table: "TransactionItems",
                keyColumn: "TransactionItemId",
                keyValue: new Guid("eede4caf-7de9-4db6-ba4d-7bd94bddddeb"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "TransactionId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.InsertData(
                table: "AuditLogs",
                columns: new[] { "AuditLogId", "Action", "CreatedAt", "EntityId", "EntityName", "UserId" },
                values: new object[] { new Guid("d52a6e6a-c10d-4e26-9610-1b8ea8804436"), "CreateProduct", new DateTime(2025, 7, 8, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6986), new Guid("77777777-7777-7777-7777-777777777777"), "Product", new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "CreatedAt", "Email", "IsDeleted", "Name", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("0d7dd35c-d9da-4399-b7b4-8ed60c91814b"), new DateTime(2025, 7, 8, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6776), "john@example.com", false, "John Doe", "123-456-7890", null });

            migrationBuilder.InsertData(
                table: "InventoryAdjustments",
                columns: new[] { "InventoryAdjustmentId", "CreatedAt", "IsDeleted", "ProductId", "Quantity", "Reason", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("1989bd2d-a075-4fde-a58b-2360c98406e4"), new DateTime(2025, 7, 8, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6938), false, new Guid("77777777-7777-7777-7777-777777777777"), 5, "Restock", null, new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "IsDeleted", "Name", "Price", "SKU", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("bda81a10-e66b-44b3-ad03-e1b627832cb3"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 7, 8, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6325), false, "Laptop", 999.99m, "LAP001", 10, new DateTime(2025, 7, 8, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6326) },
                    { new Guid("c3d5c642-e2ac-422c-a819-25b495893c5d"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 7, 8, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6335), false, "Phone", 499.99m, "PHN001", 20, new DateTime(2025, 7, 8, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6336) }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "TransactionId", "CreatedAt", "IsDeleted", "Status", "TotalAmount", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2025, 7, 7, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6823), false, 1, 1499.98m, null, new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "TransactionItems",
                columns: new[] { "TransactionItemId", "ProductId", "Quantity", "TransactionId", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("8a5d120b-beca-47f0-9e63-12ea0e459ff8"), new Guid("88888888-8888-8888-8888-888888888888"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 499.99m },
                    { new Guid("eede4caf-7de9-4db6-ba4d-7bd94bddddeb"), new Guid("77777777-7777-7777-7777-777777777777"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 999.99m }
                });
        }
    }
}
