using BrewPoint.Models;
using System;

namespace BrewPoint.Repositories.Interfaces
{
    public interface ICoffeeRepository
    {
        Task<IEnumerable<Coffee>> GetAllCoffees();
        Task<Coffee> GetCoffeeById(int id);
        //Task<IEnumerable<Coffee>> GetCoffiesByType(int coffeeTypeId);
        Task Create(Coffee coffee);
        Task Delete(int id);
        Task UpdateCoffee(Coffee coffee);
    }
}
