using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
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
            return Ok(coffees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coffee = await _coffeeService.GetCoffeeByIdAsync(id);
            if (coffee == null)
                return NotFound(new { message = $"Coffee with ID {id} was not found." });

            return Ok(coffee);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Coffee coffee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _coffeeService.CreateCoffeeAsync(coffee);
            return CreatedAtAction(nameof(GetById), new { id = coffee.Id }, coffee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Coffee coffee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != coffee.Id)
                return BadRequest(new { message = "Route ID does not match body ID." });

            var existing = await _coffeeService.GetCoffeeByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Coffee with ID {id} was not found." });

            await _coffeeService.UpdateCoffeeAsync(coffee);
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
    }
}