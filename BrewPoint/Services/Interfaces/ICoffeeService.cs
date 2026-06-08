using BrewPoint.Models;

namespace BrewPoint.Services.Interfaces
{
    public interface ICoffeeService
    {
        Task<IEnumerable<Coffee>> GetAllCoffeesAsync();
        Task<Coffee?> GetCoffeeByIdAsync(int id);
        Task CreateCoffeeAsync(Coffee coffee, List<int> ingredientIds);
        Task UpdateCoffeeAsync(Coffee coffee, List<int> ingredientIds);
        Task DeleteCoffeeAsync(int id);
    }
}