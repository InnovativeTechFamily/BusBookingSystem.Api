using BusBookingSystem.Api.Models.DTOs.Auth;

namespace BusBookingSystem.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
    }
}