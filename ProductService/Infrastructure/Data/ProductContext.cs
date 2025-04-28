
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data
{
    public class ProductContext(DbContextOptions<ProductContext> options)
        : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }
}
