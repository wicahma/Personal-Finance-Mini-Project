using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PersonalFinanceWeb.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

// Named HttpClient for API calls — base address is the Blazor server host.
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

builder.Services.AddScoped<IDashboardApiService, HttpDashboardApiService>();

await builder.Build().RunAsync();
