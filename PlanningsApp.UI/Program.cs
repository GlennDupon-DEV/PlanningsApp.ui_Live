using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PlanningsApp.UI;
using PlanningsApp.UI.Models;
using PlanningsApp.UI.Services;
using PlanningsApp.UI.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomLeft;
});

// Add Auth from static ui config
// TODO: Add from dbContext dynamically
using (HttpClient oidcHttp = new())
{
    OidcSettings? oidcConfig = await oidcHttp.GetFromJsonAsync<OidcSettings>(
        "https://planningsapp-pg3-identityserver.azurewebsites.net/api/oidc_config"
    );

    builder.Services.AddOidcAuthentication(options =>
    {
        options.ProviderOptions.Authority = oidcConfig.Authority;
        options.ProviderOptions.ClientId = oidcConfig.ClientId;
        options.ProviderOptions.RedirectUri = oidcConfig.RedirectUri;
        options.ProviderOptions.PostLogoutRedirectUri = oidcConfig.PostLogoutRedirectUri;
        options.ProviderOptions.ResponseType = oidcConfig.ResponseType;
        options.ProviderOptions.DefaultScopes.Clear();

        foreach (var scope in oidcConfig.DefaultScopes)
        {
            options.ProviderOptions.DefaultScopes.Add(scope);
        }
    });
}

// Configure Api Settings
// Load ApiSettings manually
using (HttpClient localHttp = new() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
{
    ApiSettings apiSettings = await localHttp.GetFromJsonAsync<ApiSettings>("appsettings.json");
    builder.Services.AddSingleton<ApiSettings>(apiSettings);
}

builder.Services.AddScoped<IAbsenceService, AbsenceService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ILineService, LineService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ILineWorkpostService, LineWorkpostService>();
builder.Services.AddScoped<IPlanningService, PlanningService>();

await builder.Build().RunAsync();
