using System.ComponentModel.DataAnnotations;

namespace MiniCRM.Models
{
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

        // Foreign Key
        public int CuctomerId { get; set; }

        // Navigation Property
        public Customer? Customer { get; set; }
    }
}
