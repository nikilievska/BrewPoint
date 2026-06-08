using BrewPoint.Data;
using BrewPoint.Models;
using BrewPoint.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace BrewPoint.Tests.Repositories;

public class CoffeeRepositoryTests : IDisposable
{
    private readonly AppDbContext _ctx;
    private readonly CoffeeRepository _sut;

    public CoffeeRepositoryTests()
    {
        // Unique DB name per test for full isolation
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"coffees-{Guid.NewGuid()}")
            .Options;

        _ctx = new AppDbContext(options);

        // Seed 
        _ctx.Coffees.AddRange(
            new Coffee
            {
                Id = 1,
                Name = "Espresso",
                Price = 2.50m,
                Description = "Strong and bold",
                ImagePath = ""
            },
            new Coffee
            {
                Id = 2,
                Name = "Latte",
                Price = 3.50m,
                Description = "Smooth and creamy",
                ImagePath = ""
            });
        _ctx.SaveChanges();

        _sut = new CoffeeRepository(_ctx);
    }

    [Fact]
    public async Task GetAllCoffees_ReturnsAllSeededCoffees()
    {
        var result = await _sut.GetAllCoffees();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Espresso");
        Assert.Contains(result, c => c.Name == "Latte");
    }

    [Fact]
    public async Task GetCoffeeById_WhenExists_ReturnsCoffee()
    {
        var seeded = _ctx.Coffees.First(c => c.Name == "Espresso");

        var result = await _sut.GetCoffeeById(seeded.Id);

        Assert.NotNull(result);
        Assert.Equal("Espresso", result!.Name);
    }

    [Fact]
    public async Task GetCoffeeById_WhenMissing_ReturnsNull()
    {
        var result = await _sut.GetCoffeeById(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_PersistsTheCoffee()
    {
        var coffee = new Coffee
        {
            Id = 3,
            Name = "Cappuccino",
            Price = 3.00m,
            Description = "Frothy and rich",
            ImagePath = ""
        };

        await _sut.Create(coffee);

        Assert.Equal(3, _ctx.Coffees.Count());
        Assert.Contains(_ctx.Coffees, c => c.Name == "Cappuccino");
    }

    [Fact]
    public async Task UpdateCoffee_PersistsTheChange()
    {
        var seeded = _ctx.Coffees.First(c => c.Name == "Espresso");
        seeded.Name = "Double Espresso";
        seeded.Price = 4.00m;

        await _sut.UpdateCoffee(seeded);

        var updated = _ctx.Coffees.First(c => c.Id == seeded.Id);
        Assert.Equal("Double Espresso", updated.Name);
        Assert.Equal(4.00m, updated.Price);
    }

    [Fact]
    public async Task Delete_RemovesTheCoffee()
    {
        var seeded = _ctx.Coffees.First(c => c.Name == "Espresso");

        await _sut.Delete(seeded.Id);

        Assert.Single(_ctx.Coffees);
        Assert.DoesNotContain(_ctx.Coffees, c => c.Name == "Espresso");
    }

    [Fact]
    public async Task Delete_WhenMissing_DoesNotThrow()
    {
        var exception = await Record.ExceptionAsync(() => _sut.Delete(999));

        Assert.Null(exception);
    }

    // Clean up after each test
    public void Dispose() => _ctx.Dispose();
}