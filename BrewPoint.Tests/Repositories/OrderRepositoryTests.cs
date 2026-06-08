using BrewPoint.Data;
using BrewPoint.Models;
using BrewPoint.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace BrewPoint.Tests.Repositories;

public class OrderRepositoryTests : IDisposable
{
    private readonly AppDbContext _ctx;
    private readonly OrderRepository _sut;

    public OrderRepositoryTests()
    {
        // Unique DB name per test for full isolation
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"orders-{Guid.NewGuid()}")
            .Options;

        _ctx = new AppDbContext(options);

        var user = new ApplicationUser
        {
            Id = "user-1",
            FullName = "Test User",
            Email = "test@brewpoint.local",
            UserName = "test@brewpoint.local"
        };
        _ctx.Users.Add(user);

        //Seed
        _ctx.Orders.AddRange(
            new Order
            {
                Id = 1,
                UserId = "user-1",
                Status = OrderStatus.Pending,
                TotalPrice = 5.00m,
                Items = new List<OrderItem>()
            },
            new Order
            {
                Id = 2,
                UserId = "user-1",
                Status = OrderStatus.Completed,
                TotalPrice = 10.00m,
                Items = new List<OrderItem>()
            });
        _ctx.SaveChanges();

        _sut = new OrderRepository(_ctx);
    }


    [Fact]
    public async Task GetAllOrders_ReturnsAllSeededOrders()
    {
        var result = await _sut.GetAllOrders();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetOrdersByUserId_ReturnsOnlyThatUsersOrders()
    {
        var result = await _sut.GetOrdersByUserId("user-1");

        Assert.Equal(2, result.Count());
        Assert.All(result, o => Assert.Equal("user-1", o.UserId));
    }

    [Fact]
    public async Task GetOrdersByUserId_WhenNoOrders_ReturnsEmpty()
    {
        var result = await _sut.GetOrdersByUserId("nonexistent-user");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOrderById_WhenExists_ReturnsOrder()
    {
        var seeded = _ctx.Orders.First(o => o.Status == OrderStatus.Pending);

        var result = await _sut.GetOrderById(seeded.Id);

        Assert.NotNull(result);
        Assert.Equal("user-1", result!.UserId);
    }

    [Fact]
    public async Task GetOrderById_WhenMissing_ReturnsNull()
    {
        var result = await _sut.GetOrderById(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task PlaceOrder_PersistsTheOrder()
    {
        var order = new Order
        {
            Id = 3,
            UserId = "user-1",
            Status = OrderStatus.Pending,
            TotalPrice = 7.50m,
            Items = new List<OrderItem>()
        };

        await _sut.PlaceOrder(order);

        Assert.Equal(3, _ctx.Orders.Count());
    }

    [Fact]
    public async Task UpdateOrder_PersistsTheChange()
    {
        var seeded = _ctx.Orders.First(o => o.Status == OrderStatus.Pending);
        seeded.Status = OrderStatus.Preparing;

        await _sut.UpdateOrder(seeded);

        var updated = _ctx.Orders.First(o => o.Id == seeded.Id);
        Assert.Equal(OrderStatus.Preparing, updated.Status);
    }

    [Fact]
    public async Task DeleteOrder_RemovesTheOrder()
    {
        var seeded = _ctx.Orders.First(o => o.Status == OrderStatus.Pending);

        await _sut.DeleteOrder(seeded.Id);

        Assert.Single(_ctx.Orders);
        Assert.DoesNotContain(_ctx.Orders, o => o.Status == OrderStatus.Pending);
    }

    [Fact]
    public async Task DeleteOrder_WhenMissing_DoesNotThrow()
    {
        var exception = await Record.ExceptionAsync(() => _sut.DeleteOrder(999));

        Assert.Null(exception);
    }

    // Clean up after each test
    public void Dispose() => _ctx.Dispose();
}