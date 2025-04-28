using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data
{
    public class OrderContext(DbContextOptions<OrderContext> options)
        : DbContext(options)
    {
        public DbSet<Order> Orders => Set<Order>();
    }
}
