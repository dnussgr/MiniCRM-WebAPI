using System.ComponentModel.DataAnnotations;

namespace MiniCRM.Models
{
    /// <summary>
    /// Represents a customer in the CRM system
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// ID acts as primary key
        /// </summary>
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// UTC timestamp of customer creation
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property - all orders related to this customer
        /// </summary>
        public ICollection<Order>? Orders { get; set; }
    }
}
