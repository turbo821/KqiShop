using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _context;

        public ProductRepository(ProductContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            return product;
        }

        public async Task<int?> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            var success = await _context.SaveChangesAsync();
            if (success > 0)
                return product.Id;

            return null;
        }

        public async Task<int?> UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if(product is null) return null;

            product.Stock += quantity;

            var success = await _context.SaveChangesAsync();

            if (success > 0)
                return product.Stock;

            return null;
            
        }


    }
}
