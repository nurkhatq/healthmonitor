using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using HealthMonitor.Shared.Models.DTOs.Auth;

namespace HealthMonitor.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public AuthService(
        HttpClient httpClient, 
        AuthenticationStateProvider authStateProvider,
        NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { email, password });
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result?.Token != null)
                {
                    ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result?.Token != null)
                {
                    ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
            _navigationManager.NavigateTo("/login", true);
        }
        catch
        {
            // Обработка ошибок
        }
    }
}