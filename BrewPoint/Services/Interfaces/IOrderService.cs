using BrewPoint.Models;

namespace BrewPoint.Services.Interfaces
{
    public interface IOrderService
    {
        Task PlaceOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        decimal CalculateTotalPrice(decimal basePrice, List<OrderItemIngredient> extras);
    }
}