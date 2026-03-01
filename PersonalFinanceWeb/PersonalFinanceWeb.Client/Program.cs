using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PersonalFinanceWeb.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

builder.Services.AddScoped<IDashboardApiService, HttpDashboardApiService>();
builder.Services.AddScoped<IProfileClientService, HttpProfileClientService>();
builder.Services.AddScoped<IAccountClientService, HttpAccountClientService>();
builder.Services.AddScoped<ICategoryClientService, HttpCategoryClientService>();
builder.Services.AddScoped<ITagClientService, HttpTagClientService>();
builder.Services.AddScoped<ITransactionClientService, HttpTransactionClientService>();
builder.Services.AddScoped<IBudgetClientService, HttpBudgetClientService>();
builder.Services.AddScoped<IGoalClientService, HttpGoalClientService>();

await builder.Build().RunAsync();
