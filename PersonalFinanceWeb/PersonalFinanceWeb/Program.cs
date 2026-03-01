using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Infrastructure;
using PersonalFinance.Infrastructure.Identity;
using PersonalFinance.Infrastructure.Persistence;
using PersonalFinanceWeb.Client.Services;
using PersonalFinanceWeb.Components;
using PersonalFinanceWeb.Components.Account;
using PersonalFinanceWeb.Middleware;
using PersonalFinanceWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies(options =>
    {
        options.ApplicationCookie?.PostConfigure(cookieOptions =>
        {
            cookieOptions.Events.OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    ctx.Response.ContentType = "application/json";
                    return ctx.Response.WriteAsync("{\"status\":false,\"message\":\"Unauthorized.\"}");
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
            cookieOptions.Events.OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    ctx.Response.ContentType = "application/json";
                    return ctx.Response.WriteAsync("{\"status\":false,\"message\":\"Forbidden.\"}");
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
        });
    });

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<FinanceDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender<ApplicationUser>, SmtpEmailSender>();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IDashboardApiService, ServerDashboardApiService>();
builder.Services.AddScoped<IProfileClientService, ServerProfileClientService>();
builder.Services.AddScoped<IAccountClientService, ServerAccountClientService>();
builder.Services.AddScoped<ICategoryClientService, ServerCategoryClientService>();
builder.Services.AddScoped<ITagClientService, ServerTagClientService>();
builder.Services.AddScoped<ITransactionClientService, ServerTransactionClientService>();
builder.Services.AddScoped<IBudgetClientService, ServerBudgetClientService>();
builder.Services.AddScoped<IGoalClientService, ServerGoalClientService>();
builder.Services.AddScoped<ICurrencyFormatterService, CurrencyFormatterService>();

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapIdentityApi<ApplicationUser>();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(PersonalFinanceWeb.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.Run();
