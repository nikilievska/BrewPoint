using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrewPoint.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int CoffeeId { get; set; }
        public Coffee Coffee { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        public string? SugarType { get; set; }
        public int? SugarQuantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public ICollection<OrderItemIngredient> Extras { get; set; } = new List<OrderItemIngredient>();
    }
}