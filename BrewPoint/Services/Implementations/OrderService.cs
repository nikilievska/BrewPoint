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
            order.TotalPrice = order.Items.Sum(item =>
                CalculateTotalPrice(item.UnitPrice, item.Extras?.ToList() ?? new()) * item.Quantity
            );
            await _orderRepo.PlaceOrder(order);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllOrders();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _orderRepo.GetOrdersByUserId(userId);
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepo.GetOrderById(orderId);

            if (order == null || order.UserId != userId || order.Status != OrderStatus.Pending)
                return false;

            await _orderRepo.DeleteOrder(orderId);
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepo.GetOrderById(orderId);

            if (order == null)
                return false;

            order.Status = newStatus;
            await _orderRepo.UpdateOrder(order);
            return true;
        }

        public decimal CalculateTotalPrice(decimal basePrice, List<OrderItemIngredient> extras)
        {
            var extrasTotal = extras.Sum(e => e.PriceAtOrder);
            return basePrice + extrasTotal;
        }
    }
}