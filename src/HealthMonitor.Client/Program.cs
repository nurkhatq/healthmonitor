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
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

// Configure HttpClient
builder.Services.AddScoped(sp => 
{
    var handler = sp.GetRequiredService<CustomAuthorizationMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:5165")
    };
    
    return client;
});

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();