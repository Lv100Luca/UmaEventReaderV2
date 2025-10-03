using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor.Services;
using UmaEventReaderV2.Website;
using UmaEventReaderV2.Website.Hubs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// MudBlazor
builder.Services.AddMudServices();

// SignalR Hub
builder.Services.AddSingleton(sp =>
{
    var hubConnection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7252/events") // backend hub
        .WithAutomaticReconnect()
        .Build();

    return hubConnection;
});

builder.Services.AddSingleton<UmaEventHub>();

var app = builder.Build();

var connection = app.Services.GetRequiredService<HubConnection>();
await connection.StartAsync();

await app.RunAsync();