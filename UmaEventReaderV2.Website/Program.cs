using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using UmaEventReaderV2.Website;
using UmaEventReaderV2.Website.Hubs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// MudBlazor
builder.Services.AddMudServices();

builder.Services.AddSingleton<UmaEventHub>();
builder.Services.AddSingleton<WebSettingsHub>();
builder.Services.AddSingleton<WebUmaHub>();

var app = builder.Build();

await app.RunAsync();