using ProductService.Models;

namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<Product> Product { get; set; } = null!;
        public OrderStatus Status { get; set; }
    }
}
