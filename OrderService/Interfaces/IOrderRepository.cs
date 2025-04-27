
using OrderService.Models;

namespace OrderService.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> AddAsync(Order order);
        Task UpdateStatusAsync(int orderId, OrderStatus status);
        Task<Order?> GetByIdAsync(int id);
    }
}
