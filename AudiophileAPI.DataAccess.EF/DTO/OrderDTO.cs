namespace AudiophileAPI.DTO
{
    public class OrderDTO
    {

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public string ShippingAddress { get; set; } = null!;
    }
}
