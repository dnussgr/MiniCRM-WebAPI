namespace MiniCRM.Dtos
{
    public class UpdateOrderDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
    }
}
