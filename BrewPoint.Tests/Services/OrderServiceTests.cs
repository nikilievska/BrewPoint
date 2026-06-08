using BrewPoint.Models;
using BrewPoint.Repositories.Interfaces;
using BrewPoint.Services.Implementations;
using NSubstitute;

namespace BrewPoint.Tests.Services;

public class OrderServiceTests
{
    private readonly IOrderRepository _repo;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _repo = Substitute.For<IOrderRepository>();
        _sut = new OrderService(_repo);
    }

    // GetAllOrdersAsync Test

    [Fact]
    public async Task GetAllOrdersAsync_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = 1, UserId = "user-1", Status = OrderStatus.Pending, TotalPrice = 5.00m },
            new() { Id = 2, UserId = "user-2", Status = OrderStatus.Completed, TotalPrice = 10.00m }
        };
        _repo.GetAllOrders().Returns(orders);

        // Act
        var result = await _sut.GetAllOrdersAsync();

        // Assert
        Assert.Equal(2, result.Count());
        await _repo.Received(1).GetAllOrders();
    }

    // GetOrdersByUserIdAsync Test

    [Fact]
    public async Task GetOrdersByUserIdAsync_ReturnsOnlyThatUsersOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = 1, UserId = "user-1", Status = OrderStatus.Pending, TotalPrice = 5.00m },
            new() { Id = 2, UserId = "user-1", Status = OrderStatus.Completed, TotalPrice = 8.00m }
        };
        _repo.GetOrdersByUserId("user-1").Returns(orders);

        // Act
        var result = await _sut.GetOrdersByUserIdAsync("user-1");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, o => Assert.Equal("user-1", o.UserId));
        await _repo.Received(1).GetOrdersByUserId("user-1");
    }

    // PlaceOrderAsync Tests

    [Fact]
    public async Task PlaceOrderAsync_CalculatesTotalPriceCorrectly()
    {
        // Arrange 
        var order = new Order
        {
            UserId = "user-1",
            Items = new List<OrderItem>
            {
                new()
                {
                    CoffeeId = 1,
                    Quantity = 2,
                    UnitPrice = 3.00m,
                    Extras = new List<OrderItemIngredient>
                    {
                        new() { PriceAtOrder = 0.50m }
                    }
                }
            }
        };

        // Act
        await _sut.PlaceOrderAsync(order);

        // Assert 
        Assert.Equal(7.00m, order.TotalPrice);
        await _repo.Received(1).PlaceOrder(order);
    }

    [Fact]
    public async Task PlaceOrderAsync_NoExtras_CalculatesBasePriceOnly()
    {
        // Arrange
        var order = new Order
        {
            UserId = "user-1",
            Items = new List<OrderItem>
            {
                new()
                {
                    CoffeeId = 1,
                    Quantity = 3,
                    UnitPrice = 2.50m,
                    Extras = new List<OrderItemIngredient>()
                }
            }
        };

        // Act
        await _sut.PlaceOrderAsync(order);

        // Assert 
        Assert.Equal(7.50m, order.TotalPrice);
    }

    // CancelOrderAsync Tests

    [Fact]
    public async Task CancelOrderAsync_WhenPendingAndOwner_ReturnsTrue()
    {
        // Arrange
        var order = new Order { Id = 1, UserId = "user-1", Status = OrderStatus.Pending };
        _repo.GetOrderById(1).Returns(order);

        // Act
        var result = await _sut.CancelOrderAsync(1, "user-1");

        // Assert
        Assert.True(result);
        await _repo.Received(1).DeleteOrder(1);
    }

    [Fact]
    public async Task CancelOrderAsync_WhenMissing_ReturnsFalse()
    {
        // Arrange
        _repo.GetOrderById(999).Returns((Order?)null);

        // Act
        var result = await _sut.CancelOrderAsync(999, "user-1");

        // Assert
        Assert.False(result);
        await _repo.DidNotReceive().DeleteOrder(Arg.Any<int>());
    }

    [Fact]
    public async Task CancelOrderAsync_WhenNotOwner_ReturnsFalse()
    {
        // Arrange
        var order = new Order { Id = 1, UserId = "user-1", Status = OrderStatus.Pending };
        _repo.GetOrderById(1).Returns(order);

        // Act
        var result = await _sut.CancelOrderAsync(1, "different-user");

        // Assert
        Assert.False(result);
        await _repo.DidNotReceive().DeleteOrder(Arg.Any<int>());
    }

    [Fact]
    public async Task CancelOrderAsync_WhenNotPending_ReturnsFalse()
    {
        // Arrange
        var order = new Order { Id = 1, UserId = "user-1", Status = OrderStatus.Preparing };
        _repo.GetOrderById(1).Returns(order);

        // Act
        var result = await _sut.CancelOrderAsync(1, "user-1");

        // Assert
        Assert.False(result);
        await _repo.DidNotReceive().DeleteOrder(Arg.Any<int>());
    }

    // UpdateOrderStatusAsync Tests

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenExists_ReturnsTrue()
    {
        // Arrange
        var order = new Order { Id = 1, UserId = "user-1", Status = OrderStatus.Pending };
        _repo.GetOrderById(1).Returns(order);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(1, OrderStatus.Preparing);

        // Assert
        Assert.True(result);
        Assert.Equal(OrderStatus.Preparing, order.Status);
        await _repo.Received(1).UpdateOrder(order);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenMissing_ReturnsFalse()
    {
        // Arrange
        _repo.GetOrderById(999).Returns((Order?)null);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(999, OrderStatus.Preparing);

        // Assert
        Assert.False(result);
        await _repo.DidNotReceive().UpdateOrder(Arg.Any<Order>());
    }

    // CalculateTotalPrice Tests

    [Fact]
    public void CalculateTotalPrice_WithExtras_ReturnsCorrectTotal()
    {
        // Arrange
        var extras = new List<OrderItemIngredient>
        {
            new() { PriceAtOrder = 0.50m },
            new() { PriceAtOrder = 0.75m }
        };

        // Act
        var result = _sut.CalculateTotalPrice(3.00m, extras);

        // Assert 
        Assert.Equal(4.25m, result);
    }

    [Fact]
    public void CalculateTotalPrice_NoExtras_ReturnsBasePrice()
    {
        // Act
        var result = _sut.CalculateTotalPrice(3.00m, new List<OrderItemIngredient>());

        // Assert
        Assert.Equal(3.00m, result);
    }
}