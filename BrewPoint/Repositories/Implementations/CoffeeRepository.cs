using BrewPoint.Data;
using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewPoint.Repositories.Implementations
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly AppDbContext _context;

        public CoffeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coffee>> GetAllCoffees()
        {
            return await _context.Coffees.ToListAsync();
        }

        public async Task<Coffee?> GetCoffeeById(int id)
        {
            return await _context.Coffees
                .Include(c => c.CoffeeIngredients)
                    .ThenInclude(ci => ci.Ingredient)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Create(Coffee coffee)
        {
            _context.Coffees.Add(coffee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCoffee(Coffee coffee)
        {
            _context.Coffees.Update(coffee);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var coffee = await _context.Coffees.FindAsync(id);
            if (coffee != null)
            {
                _context.Coffees.Remove(coffee);
                await _context.SaveChangesAsync();
            }
        }
    }
}