using BrewPoint.Models;

namespace BrewPoint.Services.Interfaces
{
    public interface ICoffeeService
    {
        Task<IEnumerable<Coffee>> GetAllCoffeesAsync();
        Task<Coffee?> GetCoffeeByIdAsync(int id);
        Task CreateCoffeeAsync(Coffee coffee);       
        Task UpdateCoffeeAsync(Coffee coffee);       
        Task DeleteCoffeeAsync(int id);
    }
}