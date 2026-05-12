using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Interfaces;

namespace BrewPoint.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task PlaceOrderAsync(Order order)
        {
            order.TotalPrice = CalculateTotalPrice(order.TotalPrice, order.SugarQuantity ?? 0);
            await _orderRepo.PlaceOrder(order);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllOrdersAsync();
        }

        public decimal CalculateTotalPrice(decimal basePrice, int sugarQuantity)
        {
            return basePrice + (sugarQuantity * 0.10m); 
        }
    }
}