using HealthMonitor.Shared.Models.DTOs.Auth;

namespace HealthMonitor.Client.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
}