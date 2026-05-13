using System.ComponentModel.DataAnnotations;

namespace BrewPoint.DTOs.Requests
{
    public class OrderItemRequest
    {
        [Required]
        public int CoffeeId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }

        public string? SugarType { get; set; }

        public int? SugarQuantity { get; set; }
        public List<int> ExtraIngredientIds { get; set; } = new List<int>();
    }
}