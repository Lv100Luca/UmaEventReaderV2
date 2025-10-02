// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UmaEventReaderV2.Console;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSingleton<SpectreUmaFrontend>();

// SignalR Hub
builder.Services.AddSingleton(sp =>
{
    var hubConnection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7252/events") // backend hub
        .WithAutomaticReconnect()
        .Build();

    return hubConnection;
});

var app = builder.Build();

var scope = app.Services.CreateScope();

var frontend = scope.ServiceProvider.GetRequiredService<SpectreUmaFrontend>();

await frontend.RunAsync();

return;