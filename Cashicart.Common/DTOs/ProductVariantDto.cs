

namespace Cashicart.Common.DTOs
{
    public class ProductVariantDto
    {
        public Guid? VariantId { get; set; }
        public string SKU { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
