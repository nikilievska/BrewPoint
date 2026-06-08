using BrewPoint.Data;
using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewPoint.Repositories.Implementations
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly AppDbContext _context;

        public IngredientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ingredient?> GetIngredientById(int id)
        {
            return await _context.Ingredients.FindAsync(id);
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredients()
        {
            return await _context.Ingredients.ToListAsync();
        }
    }
}