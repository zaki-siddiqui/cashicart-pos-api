using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cashicart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "AuditLogId",
                keyValue: new Guid("0557db3f-aa36-4320-93c2-e931ea64929f"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: new Guid("8482d5ce-a8be-4510-beaa-b3dfa511fff3"));

            migrationBuilder.DeleteData(
                table: "InventoryAdjustments",
                keyColumn: "InventoryAdjustmentId",
                keyValue: new Guid("c3f72397-b379-4a40-b6d7-ddf9eff11371"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("503eb665-a4cb-40f2-88bb-61f0c3302fc1"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("c9e50e13-e446-4b3c-a7b5-912ec3a38903"));

            migrationBuilder.DeleteData(
                table: "TransactionItems",
                keyColumn: "TransactionItemId",
                keyValue: new Guid("3b6cc23b-0b0d-40bb-8283-6dc9185540f6"));

            migrationBuilder.DeleteData(
                table: "TransactionItems",
                keyColumn: "TransactionItemId",
                keyValue: new Guid("557231c2-2069-46f1-b392-eac6ba0c35d1"));

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AuditLogs",
                columns: new[] { "AuditLogId", "Action", "CreatedAt", "EntityId", "EntityName", "UserId" },
                values: new object[] { new Guid("8c83bb18-1305-4dab-b2b6-f09ef87ed034"), "CreateProduct", new DateTime(2025, 7, 1, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(617), new Guid("77777777-7777-7777-7777-777777777777"), "Product", new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "CreatedAt", "Email", "Name", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("a527a394-9175-4109-a8f2-45c73b32cded"), new DateTime(2025, 7, 1, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(484), "john@example.com", "John Doe", "123-456-7890", null });

            migrationBuilder.InsertData(
                table: "InventoryAdjustments",
                columns: new[] { "InventoryAdjustmentId", "CreatedAt", "ProductId", "Quantity", "Reason", "UserId" },
                values: new object[] { new Guid("f3aee17c-5f96-445b-9ede-cbb37fec0cd6"), new DateTime(2025, 7, 1, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(585), new Guid("77777777-7777-7777-7777-777777777777"), 5, "Restock", new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "Name", "Price", "SKU", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("91a54551-41fa-4bda-a7f9-522e2e9c7393"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 7, 1, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(278), "Phone", 499.99m, "PHN001", 20, new DateTime(2025, 7, 1, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(279) },
                    { new Guid("ae405f2a-3dc0-4176-8afd-04b032ae36bd"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 7, 1, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(257), "Laptop", 999.99m, "LAP001", 10, new DateTime(2025, 7, 1, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(258) }
                });

            migrationBuilder.InsertData(
                table: "TransactionItems",
                columns: new[] { "TransactionItemId", "ProductId", "Quantity", "TransactionId", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("32a95d15-7777-4870-944f-9e5adb89ee66"), new Guid("77777777-7777-7777-7777-777777777777"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 999.99m },
                    { new Guid("85ad40dd-df64-4ef6-9a2e-8f0fd4e6a77d"), new Guid("88888888-8888-8888-8888-888888888888"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 499.99m }
                });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "TransactionId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 14, 55, 59, 139, DateTimeKind.Utc).AddTicks(513));

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "AuditLogId",
                keyValue: new Guid("8c83bb18-1305-4dab-b2b6-f09ef87ed034"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: new Guid("a527a394-9175-4109-a8f2-45c73b32cded"));

            migrationBuilder.DeleteData(
                table: "InventoryAdjustments",
                keyColumn: "InventoryAdjustmentId",
                keyValue: new Guid("f3aee17c-5f96-445b-9ede-cbb37fec0cd6"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("91a54551-41fa-4bda-a7f9-522e2e9c7393"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("ae405f2a-3dc0-4176-8afd-04b032ae36bd"));

            migrationBuilder.DeleteData(
                table: "TransactionItems",
                keyColumn: "TransactionItemId",
                keyValue: new Guid("32a95d15-7777-4870-944f-9e5adb89ee66"));

            migrationBuilder.DeleteData(
                table: "TransactionItems",
                keyColumn: "TransactionItemId",
                keyValue: new Guid("85ad40dd-df64-4ef6-9a2e-8f0fd4e6a77d"));

            migrationBuilder.InsertData(
                table: "AuditLogs",
                columns: new[] { "AuditLogId", "Action", "CreatedAt", "EntityId", "EntityName", "UserId" },
                values: new object[] { new Guid("0557db3f-aa36-4320-93c2-e931ea64929f"), "CreateProduct", new DateTime(2025, 6, 23, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8552), new Guid("77777777-7777-7777-7777-777777777777"), "Product", new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "CreatedAt", "Email", "Name", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("8482d5ce-a8be-4510-beaa-b3dfa511fff3"), new DateTime(2025, 6, 23, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8414), "john@example.com", "John Doe", "123-456-7890", null });

            migrationBuilder.InsertData(
                table: "InventoryAdjustments",
                columns: new[] { "InventoryAdjustmentId", "CreatedAt", "ProductId", "Quantity", "Reason", "UserId" },
                values: new object[] { new Guid("c3f72397-b379-4a40-b6d7-ddf9eff11371"), new DateTime(2025, 6, 23, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8525), new Guid("77777777-7777-7777-7777-777777777777"), 5, "Restock", new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "Name", "Price", "SKU", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("503eb665-a4cb-40f2-88bb-61f0c3302fc1"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 6, 23, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8177), "Laptop", 999.99m, "LAP001", 10, new DateTime(2025, 6, 23, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8178) },
                    { new Guid("c9e50e13-e446-4b3c-a7b5-912ec3a38903"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 6, 23, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8188), "Phone", 499.99m, "PHN001", 20, new DateTime(2025, 6, 23, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8188) }
                });

            migrationBuilder.InsertData(
                table: "TransactionItems",
                columns: new[] { "TransactionItemId", "ProductId", "Quantity", "TransactionId", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("3b6cc23b-0b0d-40bb-8283-6dc9185540f6"), new Guid("88888888-8888-8888-8888-888888888888"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 499.99m },
                    { new Guid("557231c2-2069-46f1-b392-eac6ba0c35d1"), new Guid("77777777-7777-7777-7777-777777777777"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 999.99m }
                });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "TransactionId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                column: "CreatedAt",
                value: new DateTime(2025, 6, 22, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8439));
        }
    }
}
