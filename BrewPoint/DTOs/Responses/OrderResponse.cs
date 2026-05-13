using BrewPoint.Models;

namespace BrewPoint.DTOs.Responses
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }
}