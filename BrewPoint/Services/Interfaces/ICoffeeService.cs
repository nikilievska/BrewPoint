using BrewPoint.Models;

namespace BrewPoint.Services.Interfaces
{
    public interface ICoffeeService
    {
        Task<IEnumerable<Coffee>> GetAllCoffeesAsync();
        Task<Coffee?> GetCoffeeByIdAsync(int id);
    }
}