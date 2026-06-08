using System.ComponentModel.DataAnnotations;

namespace BrewPoint.DTOs.Requests
{
    public class UpdateCoffeeRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Coffee name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000, ErrorMessage = "Price must be between $0.01 and $1000.")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public List<int> IngredientIds { get; set; } = new List<int>();
    }
}