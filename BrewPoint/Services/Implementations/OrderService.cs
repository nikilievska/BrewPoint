using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Interfaces;

namespace BrewPoint.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICoffeeRepository _coffeeRepo;
        private readonly IIngredientRepository _ingredientRepo;

        public OrderService(IOrderRepository orderRepo,
                            ICoffeeRepository coffeeRepo,
                            IIngredientRepository ingredientRepo)
        {
            _orderRepo = orderRepo;
            _coffeeRepo = coffeeRepo;
            _ingredientRepo = ingredientRepo;
        }

        public async Task PlaceOrderAsync(Order order)
        {
            foreach (var item in order.Items)
            {
                // Get coffee base price
                var coffee = await _coffeeRepo.GetCoffeeById(item.CoffeeId);
                if (coffee == null) continue;

                decimal extrasTotal = 0;

                // Get each extra ingredient price
                foreach (var extra in item.Extras ?? new List<OrderItemIngredient>())
                {
                    var ingredient = await _ingredientRepo.GetIngredientById(extra.IngredientId);
                    if (ingredient != null)
                    {
                        extra.PriceAtOrder = ingredient.Price;
                        extrasTotal += ingredient.Price;
                    }
                }

                // Set unit price = coffee base + extras
                item.UnitPrice = coffee.Price + extrasTotal;
            }

            // Total = sum of (unitPrice × quantity) for all items
            order.TotalPrice = order.Items.Sum(item => item.UnitPrice * item.Quantity);

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