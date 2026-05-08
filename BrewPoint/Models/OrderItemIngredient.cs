using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrewPoint.Models
{
    [PrimaryKey(nameof(OrderItemId), nameof(IngredientId))]
    public class OrderItemIngredient
    {
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceAtOrder { get; set; }
    }
}