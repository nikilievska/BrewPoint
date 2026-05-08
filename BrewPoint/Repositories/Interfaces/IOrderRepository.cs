using BrewPoint.Models;

namespace BrewPoint.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task PlaceOrder(Order order);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetOrdersByUserId(string userId);
        Task DeleteOrder(int orderId);

    }
}
