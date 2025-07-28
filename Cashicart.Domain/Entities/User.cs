using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; private set; } = Guid.NewGuid();
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? PhoneNumber { get; private set; }
        public UserRole Role { get; private set; }
        public Guid? BusinessId { get; private set; } // For multi-tenancy
        public bool IsActive { get; private set; } = true;
        public bool EmailConfirmed { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        // Navigation properties
        public virtual Business? Business { get; private set; }

        public User()
        {
            // Parameterless constructor for EF Core
        }

        public User(string email, string passwordHash, string firstName, string lastName, 
                   UserRole role, Guid? businessId = null, string? phoneNumber = null)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new DomainException("Valid email address is required.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash is required.");
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name is required.");

            Email = email.ToLowerInvariant();
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            BusinessId = businessId;
            PhoneNumber = phoneNumber;
        }

        public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name is required.");

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException("Password hash is required.");

            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string FullName => $"{FirstName} {LastName}";
    }

    public enum UserRole
    {
        SuperAdmin = 0,     // System administrator
        BusinessOwner = 1,  // Business owner
        Manager = 2,        // Store manager
        Cashier = 3,        // Point of sale operator
        Employee = 4        // General employee
    }
}