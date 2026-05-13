namespace BrewPoint.DTOs.Responses
{
    public class OrderItemResponse
    {
        public int Id { get; set; }
        public int CoffeeId { get; set; }
        public string CoffeeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? SugarType { get; set; }
        public int? SugarQuantity { get; set; }
        public List<ExtraIngredientResponse> Extras { get; set; } = new List<ExtraIngredientResponse>();
    }
}