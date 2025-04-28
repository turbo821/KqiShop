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
            var product = await GetByIdAsync(productId);

            if (product is null) return null;

            product.Stock += quantity;

            if (product.Stock < 0) return null;

            var success = await _context.SaveChangesAsync();

            if (success > 0)
                return product.Stock;

            return null;
        }

        public async Task<int?> GetStockAsync(int productId)
        {
            var product = await GetByIdAsync(productId);

            if (product is null) return null;
            
            return product.Stock;
        }
    }
}
