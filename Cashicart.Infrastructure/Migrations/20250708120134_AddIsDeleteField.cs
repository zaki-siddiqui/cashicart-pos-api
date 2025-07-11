using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cashicart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleteField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InventoryAdjustments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InventoryAdjustments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                table: "TransactionItems",
                columns: new[] { "TransactionItemId", "ProductId", "Quantity", "TransactionId", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("8a5d120b-beca-47f0-9e63-12ea0e459ff8"), new Guid("88888888-8888-8888-8888-888888888888"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 499.99m },
                    { new Guid("eede4caf-7de9-4db6-ba4d-7bd94bddddeb"), new Guid("77777777-7777-7777-7777-777777777777"), 1, new Guid("99999999-9999-9999-9999-999999999999"), 999.99m }
                });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "TransactionId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 7, 12, 1, 33, 721, DateTimeKind.Utc).AddTicks(6823), false, null });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Name",
                table: "Customers",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Name",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Name",
                table: "Customers");

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

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InventoryAdjustments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InventoryAdjustments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
        }
    }
}
