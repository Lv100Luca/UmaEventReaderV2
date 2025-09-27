using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.WebClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Build the HubConnection here
builder.Services.AddSingleton<HubConnection>(sp =>
{
    var hubConnection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7252/events") // backend hub
        .WithAutomaticReconnect()
        .Build();

    return hubConnection;
});

await builder.Build().RunAsync();