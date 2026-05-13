namespace BrewPoint.DTOs.Responses
{
    public class CoffeeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public List<IngredientResponse> Ingredients { get; set; } = new List<IngredientResponse>();
    }
}