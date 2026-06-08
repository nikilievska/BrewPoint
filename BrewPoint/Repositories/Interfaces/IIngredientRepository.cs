using BrewPoint.Models;

namespace BrewPoint.Repositories.Interfaces
{
    public interface IIngredientRepository
    {
        Task<Ingredient?> GetIngredientById(int id);
        Task<IEnumerable<Ingredient>> GetAllIngredients();
    }
}