namespace MiniCRM.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
    }
}
