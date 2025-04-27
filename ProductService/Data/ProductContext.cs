using ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Data
{
    public class ProductContext(DbContextOptions<ProductContext> options)
        : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }
}
