using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _context;

        public OrderRepository(OrderContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            var success = await _context.SaveChangesAsync();
            if(success > 0)
                return order.Id;
            return 0;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);

            return order;
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            var order = await GetByIdAsync(orderId);

            if (order is null) return;

            order.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}
