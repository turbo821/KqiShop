
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class ProductRepository
    {
        private readonly ShopContext _context;

        public ProductRepository(ShopContext context)
        {
            _context = context;
        }


    }
}
