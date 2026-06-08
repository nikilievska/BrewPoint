using BrewPoint.DTOs.Requests;
using BrewPoint.Models;
using BrewPoint.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace BrewPoint.Tests.Services;

public class AuthServiceTests
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        // UserManager requires a store — substitute the whole thing
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);

        // Minimal JWT config so GenerateJwtToken doesn't throw
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "BrewPointSuperSecretKey1234567890!@#" },
            { "Jwt:Issuer", "BrewPoint" },
            { "Jwt:Audience", "BrewPointUsers" }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _sut = new AuthService(_userManager, _configuration);
    }

    // RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsNull()
    {
        // Arrange - simulate existing user
        _userManager.FindByEmailAsync("existing@test.com")
            .Returns(new ApplicationUser { Email = "existing@test.com" });

        var request = new RegisterUserRequest
        {
            FullName = "Test User",
            Email = "existing@test.com",
            Password = "Test123!"
        };

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        Assert.Null(result);
        await _userManager.DidNotReceive().CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RegisterAsync_WhenCreateFails_ReturnsNull()
    {
        // Arrange
        _userManager.FindByEmailAsync("new@test.com").Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        var request = new RegisterUserRequest
        {
            FullName = "Test User",
            Email = "new@test.com",
            Password = "Test123!"
        };

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_WhenSuccessful_ReturnsAuthResponse()
    {
        // Arrange
        _userManager.FindByEmailAsync("new@test.com").Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), "User")
            .Returns(IdentityResult.Success);
        _userManager.GetRolesAsync(Arg.Any<ApplicationUser>())
            .Returns(new List<string> { "User" });

        var request = new RegisterUserRequest
        {
            FullName = "Test User",
            Email = "new@test.com",
            Password = "Test123!"
        };

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@test.com", result!.Email);
        Assert.Equal("User", result.Role);
        Assert.NotEmpty(result.Token);
    }

    // LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        _userManager.FindByEmailAsync("unknown@test.com").Returns((ApplicationUser?)null);

        var request = new LoginUserRequest
        {
            Email = "unknown@test.com",
            Password = "Test123!"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ReturnsNull()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@test.com", FullName = "Test User" };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "WrongPassword").Returns(false);

        var request = new LoginUserRequest
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WhenValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@test.com", FullName = "Test User" };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "Test123!").Returns(true);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "User" });

        var request = new LoginUserRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result!.Email);
        Assert.Equal("User", result.Role);
        Assert.NotEmpty(result.Token);
    }
}