using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Interfaces;

namespace BrewPoint.Services.Implementations
{
    public class CoffeeService : ICoffeeService
    {
        private readonly ICoffeeRepository _coffeeRepo;
        private readonly IIngredientRepository _ingredientRepo;

        public CoffeeService(ICoffeeRepository coffeeRepo, IIngredientRepository ingredientRepo)
        {
            _coffeeRepo = coffeeRepo;
            _ingredientRepo = ingredientRepo;
        }

        public async Task<IEnumerable<Coffee>> GetAllCoffeesAsync()
        {
            return await _coffeeRepo.GetAllCoffees();
        }

        public async Task<Coffee?> GetCoffeeByIdAsync(int id)
        {
            return await _coffeeRepo.GetCoffeeById(id);
        }

        public async Task CreateCoffeeAsync(Coffee coffee, List<int> ingredientIds)
        {
            // Attach selected ingredients
            coffee.CoffeeIngredients = ingredientIds.Select(id => new CoffeeIngredient
            {
                IngredientId = id
            }).ToList();

            await _coffeeRepo.Create(coffee);
        }

        public async Task UpdateCoffeeAsync(Coffee coffee, List<int> ingredientIds)
        {
            // Replace ingredients
            coffee.CoffeeIngredients = ingredientIds.Select(id => new CoffeeIngredient
            {
                CoffeeId = coffee.Id,
                IngredientId = id
            }).ToList();

            await _coffeeRepo.UpdateCoffee(coffee);
        }

        public async Task DeleteCoffeeAsync(int id)
        {
            await _coffeeRepo.Delete(id);
        }
    }
}