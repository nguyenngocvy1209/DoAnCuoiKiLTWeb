namespace Documentt.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
