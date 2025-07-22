using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;


namespace Cashicart.Domain.Entities
{
    public class ProductTranslation
    {
        [Key]
        public Guid TranslationId { get; private set; } = Guid.NewGuid();

        [Required]
        public Guid ProductId { get; private set; }

        [Required]
        [MaxLength(5)]
        public string Language { get; private set; } = "en";  // e.g., "en", "fr", "ur"

        [Required]
        public string Name { get; private set; }

        public string? Description { get; private set; }

        public virtual Product Product { get; private set; }

        public ProductTranslation() { }

        
        public ProductTranslation(Guid productId, string language, string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(language)) throw new DomainException("Language is required.");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");

            
            ProductId = productId;
            Language = language.ToLower();
            Name = name;
            Description = description;
        }

        public void UpdateTranslation(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Translation name is required.");

            Name = name;
            Description = description;
        }
    }
}
