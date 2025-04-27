using ProductService.Models;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace Infrastructure.Data
{
    public class OrderContext(DbContextOptions<OrderContext> options)
        : DbContext(options)
    {
        public DbSet<Order> Orders => Set<Order>();
    }
}
