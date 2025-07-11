using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cashicart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAdjustments",
                columns: table => new
                {
                    InventoryAdjustmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdjustments", x => x.InventoryAdjustmentId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionItems",
                columns: table => new
                {
                    TransactionItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionItems", x => x.TransactionItemId);
                    table.ForeignKey(
                        name: "FK_TransactionItems_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                table: "Transactions",
                columns: new[] { "TransactionId", "CreatedAt", "Status", "TotalAmount", "UserId" },
                values: new object[] { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2025, 6, 22, 12, 27, 33, 623, DateTimeKind.Utc).AddTicks(8439), 1, 1499.98m, new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "TransactionItems",
                columns: new[] { "TransactionItemId", "ProductId", "Quantity", "TransactionId", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("3b6cc23b-0b0d-40bb-8283-6dc9185540f6"), new Guid("88888888-8888-8888-8888-888888888888"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 499.99m },
                    { new Guid("557231c2-2069-46f1-b392-eac6ba0c35d1"), new Guid("77777777-7777-7777-7777-777777777777"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 999.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_ProductId",
                table: "InventoryAdjustments",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItems_TransactionId",
                table: "TransactionItems",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "InventoryAdjustments");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "TransactionItems");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
