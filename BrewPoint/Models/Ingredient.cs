using System.ComponentModel.DataAnnotations;

namespace BrewPoint.Models
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100)]
        public decimal Price { get; set; }

        public ICollection<CoffeeIngredient>? CoffeeIngredients { get; set; }
    }
}
