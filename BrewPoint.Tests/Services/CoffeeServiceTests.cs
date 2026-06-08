using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Implementations;
using NSubstitute;

namespace BrewPoint.Tests.Services;

public class CoffeeServiceTests
{
    private readonly ICoffeeRepository _repo;
    private readonly IIngredientRepository _ingredientRepo;
    private readonly CoffeeService _sut;

    public CoffeeServiceTests()
    {
        _repo = Substitute.For<ICoffeeRepository>();
        _ingredientRepo = Substitute.For<IIngredientRepository>();
        _sut = new CoffeeService(_repo, _ingredientRepo);
    }

    // GetAllCoffeesAsync Tests

    [Fact]
    public async Task GetAllCoffeesAsync_ReturnsAllCoffees()
    {
        // Arrange
        var coffees = new List<Coffee>
        {
            new() { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Strong", ImagePath = "" },
            new() { Id = 2, Name = "Latte", Price = 3.50m, Description = "Smooth", ImagePath = "" }
        };
        _repo.GetAllCoffees().Returns(coffees);

        // Act
        var result = await _sut.GetAllCoffeesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        await _repo.Received(1).GetAllCoffees();
    }

    [Fact]
    public async Task GetAllCoffeesAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        _repo.GetAllCoffees().Returns(new List<Coffee>());

        // Act
        var result = await _sut.GetAllCoffeesAsync();

        // Assert
        Assert.Empty(result);
    }

    // GetCoffeeByIdAsync Tests

    [Fact]
    public async Task GetCoffeeByIdAsync_WhenExists_ReturnsCoffee()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Strong", ImagePath = "" };
        _repo.GetCoffeeById(1).Returns(coffee);

        // Act
        var result = await _sut.GetCoffeeByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Espresso", result!.Name);
        await _repo.Received(1).GetCoffeeById(1);
    }

    [Fact]
    public async Task GetCoffeeByIdAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        _repo.GetCoffeeById(999).Returns((Coffee?)null);

        // Act
        var result = await _sut.GetCoffeeByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    // CreateCoffeeAsync Tests

    [Fact]
    public async Task CreateCoffeeAsync_CallsRepositoryCreate()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Cappuccino", Price = 3.00m, Description = "Frothy", ImagePath = "" };
        var ingredientIds = new List<int> { 1, 2 };

        // Act
        await _sut.CreateCoffeeAsync(coffee, ingredientIds);

        // Assert - repo was called exactly once
        await _repo.Received(1).Create(coffee);
    }

    [Fact]
    public async Task CreateCoffeeAsync_AttachesIngredients()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Cappuccino", Price = 3.00m, Description = "Frothy", ImagePath = "" };
        var ingredientIds = new List<int> { 1, 2, 3 };

        // Act
        await _sut.CreateCoffeeAsync(coffee, ingredientIds);

        // Assert - ingredients were attached to the coffee
        Assert.Equal(3, coffee.CoffeeIngredients!.Count);
    }

    // UpdateCoffeeAsync Tests

    [Fact]
    public async Task UpdateCoffeeAsync_CallsRepositoryUpdate()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Updated Espresso", Price = 3.00m, Description = "Strong", ImagePath = "" };
        var ingredientIds = new List<int> { 1 };

        // Act
        await _sut.UpdateCoffeeAsync(coffee, ingredientIds);

        // Assert
        await _repo.Received(1).UpdateCoffee(coffee);
    }

    [Fact]
    public async Task UpdateCoffeeAsync_ReplacesIngredients()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Strong", ImagePath = "" };
        var ingredientIds = new List<int> { 5, 6 };

        // Act
        await _sut.UpdateCoffeeAsync(coffee, ingredientIds);

        // Assert - ingredients replaced with new ones
        Assert.Equal(2, coffee.CoffeeIngredients!.Count);
        Assert.All(coffee.CoffeeIngredients, ci => Assert.Equal(1, ci.CoffeeId));
    }

    // DeleteCoffeeAsync Test

    [Fact]
    public async Task DeleteCoffeeAsync_CallsRepositoryDelete()
    {
        // Act
        await _sut.DeleteCoffeeAsync(1);

        // Assert
        await _repo.Received(1).Delete(1);
    }
}