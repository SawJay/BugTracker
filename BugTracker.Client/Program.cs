using BugTracker.Client;
using BugTracker.Client.Services;
using BugTracker.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// add HttpClient as a service
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IProjectDTOService, WASMProjectDTOService>();

builder.Services.AddScoped<ITicketDTOService, WASMTicketDTOService>();

builder.Services.AddScoped<ICompanyDTOService, WASMCompanyDTOService>();

await builder.Build().RunAsync();
