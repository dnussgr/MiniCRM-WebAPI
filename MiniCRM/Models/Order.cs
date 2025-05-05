using System.ComponentModel.DataAnnotations;

namespace MiniCRM.Models
{
    /// <summary>
    /// Represents a customer order in the CRM system
    /// </summary>
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public bool IsCanceled { get; set; } = false;
        public DateTime? CanceledAt { get; set; }


        /// <summary>
        /// Foreign key - ID of Customer who placed the order
        /// </summary>
        public int CustomerId { get; set; }


        /// <summary>
        /// Navigation property - the customer who placed the order
        /// </summary>
        public Customer? Customer { get; set; }
    }
}
