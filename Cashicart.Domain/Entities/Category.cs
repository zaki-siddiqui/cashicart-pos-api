using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; private set; }
        public string Name { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        public Category(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name is required.");

            CategoryId = Guid.NewGuid();
            Name = name;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name is required.");

            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
