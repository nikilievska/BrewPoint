using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Interfaces;

namespace BrewPoint.Services.Implementations
{
    public class CoffeeService : ICoffeeService
    {
        private readonly ICoffeeRepository _coffeeRepo;

        public CoffeeService(ICoffeeRepository coffeeRepo)
        {
            _coffeeRepo = coffeeRepo;
        }

        public async Task<IEnumerable<Coffee>> GetAllCoffeesAsync()
        {
            return await _coffeeRepo.GetAllCoffees();
        }

        public async Task<Coffee?> GetCoffeeByIdAsync(int id)
        {
            return await _coffeeRepo.GetCoffeeById(id);
        }

        public async Task CreateCoffeeAsync(Coffee coffee)
        {
            await _coffeeRepo.Create(coffee);
        }

        public async Task UpdateCoffeeAsync(Coffee coffee)
        {
            await _coffeeRepo.UpdateCoffee(coffee);
        }

        public async Task DeleteCoffeeAsync(int id)
        {
            await _coffeeRepo.Delete(id);
        }
    }
}