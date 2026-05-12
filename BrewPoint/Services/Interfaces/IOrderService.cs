using BrewPoint.Models;

namespace BrewPoint.Services.Interfaces
{
    public interface IOrderService
    {
        Task PlaceOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        decimal CalculateTotalPrice(decimal basePrice, int sugarQuantity);
    }
}