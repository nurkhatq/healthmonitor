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
    private readonly ILocalStorageService _localStorage;
    private const string TOKEN_KEY = "auth_token";
    private string? _cachedToken;

    public AuthService(
        HttpClient httpClient, 
        AuthenticationStateProvider authStateProvider,
        NavigationManager navigationManager,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
        _localStorage = localStorage;
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
                    await SetTokenAsync(result.Token);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
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
                    await SetTokenAsync(result.Token);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Clear token from storage
            await _localStorage.RemoveItemAsync(TOKEN_KEY);
            _cachedToken = null;

            // Clear authentication state
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
            
            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            // Wait for any pending state changes
            await Task.Yield();
            
            // Navigate to login
            _navigationManager.NavigateTo("/login", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout error: {ex.Message}");
            throw;
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        if (_cachedToken != null)
            return _cachedToken;

        _cachedToken = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
        return _cachedToken;
    }

    private async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await _localStorage.SetItemAsync(TOKEN_KEY, token);
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task InitializeAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
        }
    }
}

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task InitializeAuthenticationStateAsync();
}