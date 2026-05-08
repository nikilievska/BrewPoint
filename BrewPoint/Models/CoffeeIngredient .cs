using Microsoft.EntityFrameworkCore;

namespace BrewPoint.Models
{
    [PrimaryKey(nameof(CoffeeId), nameof(IngredientId))]
    public class CoffeeIngredient
    {
        public int CoffeeId { get; set; }
        public Coffee Coffee { get; set; } = null!;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
    }
}