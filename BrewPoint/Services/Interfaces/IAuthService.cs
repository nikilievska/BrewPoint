using BrewPoint.DTOs.Requests;
using BrewPoint.DTOs.Responses;
using Microsoft.AspNetCore.Identity.Data;

namespace BrewPoint.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterUserRequest request);
        Task<AuthResponse?> LoginAsync(LoginUserRequest request);
    }
}
