using BrewPoint.Controllers;
using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace BrewPoint.Tests.Controllers;

public class AdminControllerTests
{
    private readonly IOrderService _service;
    private readonly AdminController _sut;

    public AdminControllerTests()
    {
        _service = Substitute.For<IOrderService>();
        _sut = new AdminController(_service);
    }

    // GetAllOrders Tests

    [Fact]
    public async Task GetAllOrders_ReturnsOkWithOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = 1, UserId = "user-1", Status = OrderStatus.Pending, TotalPrice = 5.00m, Items = new List<OrderItem>() },
            new() { Id = 2, UserId = "user-2", Status = OrderStatus.Completed, TotalPrice = 10.00m, Items = new List<OrderItem>() }
        };
        _service.GetAllOrdersAsync().Returns(orders);

        // Act
        var result = await _sut.GetAllOrders();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
        await _service.Received(1).GetAllOrdersAsync();
    }

    [Fact]
    public async Task GetAllOrders_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        _service.GetAllOrdersAsync().Returns(new List<Order>());

        // Act
        var result = await _sut.GetAllOrders();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    // UpdateOrderStatus Tests

    [Fact]
    public async Task UpdateOrderStatus_WhenExists_ReturnsNoContent()
    {
        // Arrange
        _service.UpdateOrderStatusAsync(1, OrderStatus.Preparing).Returns(true);

        var request = new UpdateOrderStatusRequest { Status = OrderStatus.Preparing };

        // Act
        var result = await _sut.UpdateOrderStatus(1, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        await _service.Received(1).UpdateOrderStatusAsync(1, OrderStatus.Preparing);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        _service.UpdateOrderStatusAsync(999, OrderStatus.Preparing).Returns(false);

        var request = new UpdateOrderStatusRequest { Status = OrderStatus.Preparing };

        // Act
        var result = await _sut.UpdateOrderStatus(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}