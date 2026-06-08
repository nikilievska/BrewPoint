using BrewPoint.DTOs.Requests;
using BrewPoint.DTOs.Responses;

namespace BrewPoint.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterUserRequest request);
        Task<AuthResponse?> LoginAsync(LoginUserRequest request);
    }
}
