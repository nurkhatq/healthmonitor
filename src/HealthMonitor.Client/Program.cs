using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using HealthMonitor.Client;
using HealthMonitor.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHealthService, HealthService>();

// Configure HttpClient with base address
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5165")
    };

    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    return httpClient;
});

builder.Services.AddAuthorizationCore();

var host = builder.Build();

// Add global error handling
await host.RunAsync();