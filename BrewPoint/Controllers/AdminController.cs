using BrewPoint.Models;
using BrewPoint.DTOs.Responses;
using BrewPoint.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BrewPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders.Select(o => MapToResponse(o)));
        }

        [HttpPatch("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _orderService.UpdateOrderStatusAsync(id, request.Status);
            if (!success)
                return NotFound(new { message = $"Order {id} was not found." });

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

    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
    }
}