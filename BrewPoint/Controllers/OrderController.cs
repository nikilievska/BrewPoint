using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
using BrewPoint.DTOs.Requests;
using BrewPoint.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Items == null || !request.Items.Any())
                return BadRequest(new { message = "An order must contain at least one item." });

            var order = new Order
            {
                UserId = request.UserId,
                Items = request.Items.Select(i => new OrderItem
                {
                    CoffeeId = i.CoffeeId,
                    Quantity = i.Quantity,
                    SugarType = i.SugarType,
                    SugarQuantity = i.SugarQuantity,
                    Extras = i.ExtraIngredientIds.Select(eid => new OrderItemIngredient
                    {
                        IngredientId = eid
                    }).ToList()
                }).ToList()
            };

            await _orderService.PlaceOrderAsync(order);
            return CreatedAtAction(nameof(GetMyOrders), new { userId = order.UserId }, MapToResponse(order));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMyOrders(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { message = "User ID is required." });

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders.Select(o => MapToResponse(o)));
        }

        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { message = "User ID is required." });

            var success = await _orderService.CancelOrderAsync(id, userId);
            if (!success)
                return BadRequest(new { message = "Order could not be cancelled. It may not exist, belong to you, or be in a cancellable state." });

            return NoContent();
        }

        private static OrderResponse MapToResponse(Order order) => new OrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            UserFullName = order.User?.FullName ?? string.Empty,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            TotalPrice = order.TotalPrice,
            Items = order.Items?.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                CoffeeId = i.CoffeeId,
                CoffeeName = i.Coffee?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SugarType = i.SugarType,
                SugarQuantity = i.SugarQuantity,
                Extras = i.Extras?.Select(e => new ExtraIngredientResponse
                {
                    IngredientId = e.IngredientId,
                    IngredientName = e.Ingredient?.Name ?? string.Empty,
                    PriceAtOrder = e.PriceAtOrder
                }).ToList() ?? new List<ExtraIngredientResponse>()
            }).ToList() ?? new List<OrderItemResponse>()
        };
    }
}