using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;  
using Varta.Store.Client;
using Varta.Store.Client.Auth;
using Varta.Store.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();


string apiUrl;

if (builder.HostEnvironment.IsDevelopment())
{
    // ЛОКАЛЬНО (Rider): Указываем порт, на котором запущен API
    // Проверьте launchSettings.json в API проекте, чтобы узнать точный порт!
    apiUrl = "http://localhost:5138"; 
}
else
{
    // DOCKER / PROD: Используем тот же домен, где открыт сайт (Nginx разрулит)
    apiUrl = builder.HostEnvironment.BaseAddress;
}
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(apiUrl) 
});

await builder.Build().RunAsync();