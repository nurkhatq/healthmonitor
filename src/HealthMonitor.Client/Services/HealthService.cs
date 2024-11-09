using System.Net.Http.Json;
using HealthMonitor.Shared.Models;
using HealthMonitor.Shared.Models.DTOs.Health;

namespace HealthMonitor.Client.Services;

public interface IHealthService
{
    Task<HealthSummary> GetSummaryAsync();
    Task<HealthMetrics> AddMetricAsync(HealthMetricRequest request);
    Task DeleteMetricAsync(int id);
}

public class HealthService : IHealthService
{
    private readonly HttpClient _httpClient;

    public HealthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthSummary> GetSummaryAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<HealthSummary>("api/health/summary");
            return response ?? new HealthSummary();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting health summary: {ex.Message}");
            return new HealthSummary();
        }
    }

    public async Task<HealthMetrics> AddMetricAsync(HealthMetricRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/health", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<HealthMetrics>();
            return result ?? throw new Exception("Failed to create metric");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding metric: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteMetricAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/health/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting metric: {ex.Message}");
            throw;
        }
    }
}