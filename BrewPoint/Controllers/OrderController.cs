using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
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
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (order.Items == null || !order.Items.Any())
                return BadRequest(new { message = "An order must contain at least one item." });

            await _orderService.PlaceOrderAsync(order);
            return CreatedAtAction(nameof(GetMyOrders), new { userId = order.UserId }, order);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMyOrders(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { message = "User ID is required." });

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
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
    }
}