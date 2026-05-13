using System.ComponentModel.DataAnnotations;

namespace BrewPoint.DTOs.Requests
{
    public class PlaceOrderRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}