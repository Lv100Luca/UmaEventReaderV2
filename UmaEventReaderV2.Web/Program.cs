using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Extensions;
using UmaEventReaderV2.Services;
using UmaEventReaderV2.Web;
using UmaEventReaderV2.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Register shared services
builder.Services
    .AddUmaEventReaderServices()
    .AddSingleton<EventHubBroadcaster>(); // Registered but not yet instantiated

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7284") // frontend WASM
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Required for SignalR
    });
});

// Web-specific
builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

// Use CORS before mapping hubs/controllers
app.UseCors();

app.MapHub<EventHub>("/events");
app.MapControllers();

// Ensure EventHubBroadcaster is instantiated so it subscribes to events
_ = app.Services.GetRequiredService<EventHubBroadcaster>();

// Kick off UmaEventReader in the background
var reader = app.Services.GetRequiredService<UmaEventReader>();
_ = Task.Run(() => reader.RunAsync(app.Lifetime.ApplicationStopping));

app.Run();