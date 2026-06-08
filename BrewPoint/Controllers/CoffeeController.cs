using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
using BrewPoint.DTOs.Requests;
using BrewPoint.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var coffees = await _coffeeService.GetAllCoffeesAsync();
            var response = coffees.Select(c => MapToResponse(c));
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var coffee = await _coffeeService.GetCoffeeByIdAsync(id);
            if (coffee == null)
                return NotFound(new { message = $"Coffee with ID {id} was not found." });

            return Ok(MapToResponse(coffee));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

            await _coffeeService.CreateCoffeeAsync(coffee, request.IngredientIds);
            return CreatedAtAction(nameof(GetById), new { id = coffee.Id }, MapToResponse(coffee));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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

            await _coffeeService.UpdateCoffeeAsync(existing, request.IngredientIds);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

        [HttpPost("upload-image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Only jpg, jpeg, png and webp files are allowed." });

            var fileName = Guid.NewGuid().ToString() + extension;
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { imagePath = $"/images/{fileName}" });
        }
    }
}