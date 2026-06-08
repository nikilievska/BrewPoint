using BrewPoint.Controllers;
using BrewPoint.DTOs.Requests;
using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace BrewPoint.Tests.Controllers;

public class CoffeeControllerTests
{
    private readonly ICoffeeService _service;
    private readonly CoffeeController _sut;

    public CoffeeControllerTests()
    {
        _service = Substitute.For<ICoffeeService>();
        _sut = new CoffeeController(_service);
    }

    // GetAll Test

    [Fact]
    public async Task GetAll_ReturnsOkWithCoffees()
    {
        // Arrange
        var coffees = new List<Coffee>
        {
            new() { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Strong", ImagePath = "" },
            new() { Id = 2, Name = "Latte", Price = 3.50m, Description = "Smooth", ImagePath = "" }
        };
        _service.GetAllCoffeesAsync().Returns(coffees);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    // GetById Tests

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Strong", ImagePath = "" };
        _service.GetCoffeeByIdAsync(1).Returns(coffee);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        _service.GetCoffeeByIdAsync(999).Returns((Coffee?)null);

        // Act
        var result = await _sut.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    // Create Test

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateCoffeeRequest
        {
            Name = "Cappuccino",
            Price = 3.00m,
            Description = "Frothy",
            ImagePath = ""
        };

        // Act
        var result = await _sut.Create(request);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_sut.GetById), created.ActionName);
        await _service.Received(1).CreateCoffeeAsync(Arg.Any<Coffee>());
    }

    // Update Test

    [Fact]
    public async Task Update_WhenExists_ReturnsNoContent()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Strong", ImagePath = "" };
        _service.GetCoffeeByIdAsync(1).Returns(coffee);

        var request = new UpdateCoffeeRequest
        {
            Id = 1,
            Name = "Updated Espresso",
            Price = 3.00m,
            Description = "Strong",
            ImagePath = ""
        };

        // Act
        var result = await _sut.Update(1, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        await _service.Received(1).UpdateCoffeeAsync(Arg.Any<Coffee>());
    }

    [Fact]
    public async Task Update_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        _service.GetCoffeeByIdAsync(999).Returns((Coffee?)null);

        var request = new UpdateCoffeeRequest
        {
            Id = 999,
            Name = "Ghost Coffee",
            Price = 3.00m,
            Description = "Does not exist",
            ImagePath = ""
        };

        // Act
        var result = await _sut.Update(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        await _service.DidNotReceive().UpdateCoffeeAsync(Arg.Any<Coffee>());
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateCoffeeRequest { Id = 2, Name = "Latte", Price = 3.50m, Description = "", ImagePath = "" };

        // Act
        var result = await _sut.Update(1, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // Delete Test

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContent()
    {
        // Arrange
        var coffee = new Coffee { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Strong", ImagePath = "" };
        _service.GetCoffeeByIdAsync(1).Returns(coffee);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        await _service.Received(1).DeleteCoffeeAsync(1);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        _service.GetCoffeeByIdAsync(999).Returns((Coffee?)null);

        // Act
        var result = await _sut.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        await _service.DidNotReceive().DeleteCoffeeAsync(Arg.Any<int>());
    }
}