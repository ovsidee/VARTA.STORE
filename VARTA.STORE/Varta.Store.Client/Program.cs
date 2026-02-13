using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Varta.Store.Client;
using Varta.Store.Client.Auth;
using Varta.Store.Client.Services;
using System.Globalization;
using Microsoft.Extensions.Localization;

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
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IServerTagService, ServerTagService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();


string apiUrl;

if (builder.HostEnvironment.IsDevelopment())
{
    apiUrl = "http://localhost:5138";
}
else
{
    apiUrl = builder.HostEnvironment.BaseAddress;
}
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var host = builder.Build();

var culture = new CultureInfo("uk");

try
{
    var localStorage = host.Services.GetRequiredService<ISyncLocalStorageService>();
    var cultureResult = localStorage.GetItem<string>("culture");

    if (!string.IsNullOrWhiteSpace(cultureResult))
    {
        culture = new CultureInfo(cultureResult);
    }
}
catch (Exception)
{
    // Fallback to default culture if generic error occurs (e.g. JS not ready)
    // In production, might want to log this
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();