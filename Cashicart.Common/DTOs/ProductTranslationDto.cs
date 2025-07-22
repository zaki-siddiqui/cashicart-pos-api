
namespace Cashicart.Common.DTOs
{
    public class ProductTranslationDto
    {
        public string Language { get; set; } = "en";
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
