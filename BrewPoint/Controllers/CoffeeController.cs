using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
using BrewPoint.DTOs.Requests;
using BrewPoint.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeeController : ControllerBase
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeeController(ICoffeeService coffeeService)
        {
            _coffeeService = coffeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coffees = await _coffeeService.GetAllCoffeesAsync();
            var response = coffees.Select(c => MapToResponse(c));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coffee = await _coffeeService.GetCoffeeByIdAsync(id);
            if (coffee == null)
                return NotFound(new { message = $"Coffee with ID {id} was not found." });

            return Ok(MapToResponse(coffee));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCoffeeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var coffee = new Coffee
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                ImagePath = request.ImagePath
            };

            await _coffeeService.CreateCoffeeAsync(coffee);
            return CreatedAtAction(nameof(GetById), new { id = coffee.Id }, MapToResponse(coffee));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCoffeeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest(new { message = "Route ID does not match body ID." });

            var existing = await _coffeeService.GetCoffeeByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Coffee with ID {id} was not found." });

            existing.Name = request.Name;
            existing.Price = request.Price;
            existing.Description = request.Description;
            existing.ImagePath = request.ImagePath;

            await _coffeeService.UpdateCoffeeAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _coffeeService.GetCoffeeByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Coffee with ID {id} was not found." });

            await _coffeeService.DeleteCoffeeAsync(id);
            return NoContent();
        }

        private static CoffeeResponse MapToResponse(Coffee coffee) => new CoffeeResponse
        {
            Id = coffee.Id,
            Name = coffee.Name,
            Price = coffee.Price,
            Description = coffee.Description,
            ImagePath = coffee.ImagePath,
            Ingredients = coffee.CoffeeIngredients?.Select(ci => new IngredientResponse
            {
                Id = ci.Ingredient.Id,
                Name = ci.Ingredient.Name,
                Price = ci.Ingredient.Price
            }).ToList() ?? new List<IngredientResponse>()
        };
    }
}