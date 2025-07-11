using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Domain.Entities
{
    public class ProductImage
    {
        [Key]
        public Guid ProductImageId { get; private set; } = Guid.NewGuid();
        public Guid ProductId { get; private set; }
        public string ImageUrl { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public bool IsPrimary { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;



        public virtual Product Product { get; private set; }

        public ProductImage(Guid productId, string imageUrl)
        {
            ProductId = productId;
            ImageUrl = imageUrl;
        }

        public void SetPrimary() => IsPrimary = true;
        public void UnsetPrimary() => IsPrimary = false;

        public void SetThumbnail(string url)
        {
            ThumbnailUrl = url;
        }

        // EF needs parameterless constructor
        private ProductImage() { }
    }
}
