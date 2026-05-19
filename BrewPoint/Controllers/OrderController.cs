using BrewPoint.Models;
using BrewPoint.DTOs.Requests;
using BrewPoint.DTOs.Responses;
using BrewPoint.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BrewPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            if (request.Items == null || !request.Items.Any())
                return BadRequest(new { message = "An order must contain at least one item." });

            var order = new Order
            {
                UserId = userId,
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
            return CreatedAtAction(nameof(GetMyOrders), null, MapToResponse(order));
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders.Select(o => MapToResponse(o)));
        }

        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

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