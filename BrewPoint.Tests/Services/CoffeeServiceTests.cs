using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Implementations;
using NSubstitute;

namespace BrewPoint.Tests.Services;

public class CoffeeServiceTests
{
    private readonly ICoffeeRepository _repo;
    private readonly CoffeeService _coffeeService;

    public CoffeeServiceTests()
    {
        _repo = Substitute.For<ICoffeeRepository>();
        _coffeeService = new CoffeeService(_repo);
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
        var result = await _coffeeService.GetAllCoffeesAsync();

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
        var result = await _coffeeService.GetAllCoffeesAsync();

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
        var result = await _coffeeService.GetCoffeeByIdAsync(1);

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
        var result = await _coffeeService.GetCoffeeByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    // CreateCoffeeAsync Test

    [Fact]
    public async Task CreateCoffeeAsync_CallsRepositoryCreate()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Cappuccino", Price = 3.00m, Description = "Frothy", ImagePath = "" };

        // Act
        await _coffeeService.CreateCoffeeAsync(coffee);

        // Assert 
        await _repo.Received(1).Create(coffee);
    }

    // UpdateCoffeeAsync Test

    [Fact]
    public async Task UpdateCoffeeAsync_CallsRepositoryUpdate()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Updated Espresso", Price = 3.00m, Description = "Strong", ImagePath = "" };

        // Act
        await _coffeeService.UpdateCoffeeAsync(coffee);

        // Assert
        await _repo.Received(1).UpdateCoffee(coffee);
    }

    // DeleteCoffeeAsync Test

    [Fact]
    public async Task DeleteCoffeeAsync_CallsRepositoryDelete()
    {
        // Act
        await _coffeeService.DeleteCoffeeAsync(1);

        // Assert
        await _repo.Received(1).Delete(1);
    }
}