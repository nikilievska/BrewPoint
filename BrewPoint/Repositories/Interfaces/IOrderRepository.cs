using BrewPoint.Models;

namespace BrewPoint.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task PlaceOrder(Order order);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetOrdersByUserId(string userId);
        Task<Order?> GetOrderById(int orderId);
        Task UpdateOrder(Order order);
        Task DeleteOrder(int orderId);

    }
}
