using System.Security.Claims;
using BrewPoint.Controllers;
using BrewPoint.DTOs.Requests;
using BrewPoint.Models;
using BrewPoint.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace BrewPoint.Tests.Controllers;

public class OrderControllerTests
{
    private readonly IOrderService _service;
    private readonly OrderController _sut;

    public OrderControllerTests()
    {
        _service = Substitute.For<IOrderService>();
        _sut = new OrderController(_service);

        // Simulate a logged-in user with JWT claims because the controller uses [Authorize]
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-1")
        }, "mock"));

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    // GetMyOrders Test

    [Fact]
    public async Task GetMyOrders_ReturnsOkWithOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = 1, UserId = "user-1", Status = OrderStatus.Pending, TotalPrice = 5.00m, Items = new List<OrderItem>() }
        };
        _service.GetOrdersByUserIdAsync("user-1").Returns(orders);

        // Act
        var result = await _sut.GetMyOrders();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    // PlaceOrder Tests

    [Fact]
    public async Task PlaceOrder_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new PlaceOrderRequest
        {
            Items = new List<OrderItemRequest>
            {
                new() { CoffeeId = 1, Quantity = 2, ExtraIngredientIds = new List<int>() }
            }
        };

        // Act
        var result = await _sut.PlaceOrder(request);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        await _service.Received(1).PlaceOrderAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task PlaceOrder_EmptyItems_ReturnsBadRequest()
    {
        // Arrange
        var request = new PlaceOrderRequest
        {
            Items = new List<OrderItemRequest>()
        };

        // Act
        var result = await _sut.PlaceOrder(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        await _service.DidNotReceive().PlaceOrderAsync(Arg.Any<Order>());
    }

    // CancelOrder Test

    [Fact]
    public async Task CancelOrder_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        _service.CancelOrderAsync(1, "user-1").Returns(true);

        // Act
        var result = await _sut.CancelOrder(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CancelOrder_WhenFails_ReturnsBadRequest()
    {
        // Arrange
        _service.CancelOrderAsync(1, "user-1").Returns(false);

        // Act
        var result = await _sut.CancelOrder(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}