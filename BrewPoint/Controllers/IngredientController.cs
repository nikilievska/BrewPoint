using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepo;

        public IngredientController(IIngredientRepository ingredientRepo)
        {
            _ingredientRepo = ingredientRepo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _ingredientRepo.GetAllIngredients();
            return Ok(ingredients.Select(i => new
            {
                id = i.Id,
                name = i.Name,
                price = i.Price,
              
            }));
        }
    }
}