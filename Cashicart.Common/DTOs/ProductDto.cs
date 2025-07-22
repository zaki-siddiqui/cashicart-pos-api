

namespace Cashicart.Common.DTOs
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string PrimaryImageUrl => ImageUrls?.FirstOrDefault();
        public string? Description { get; set; }
        public List<string> ThumbnailUrls { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();

    }
}