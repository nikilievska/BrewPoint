namespace BrewPoint.DTOs.Responses
{
    public class ExtraIngredientResponse
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal PriceAtOrder { get; set; }
    }
}
