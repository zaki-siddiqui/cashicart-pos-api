using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cashicart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailUrlToProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "ProductImages");
        }
    }
}
