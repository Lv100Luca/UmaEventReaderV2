using UmaEventReaderV2.Services;
using UmaEventReaderV2.Web;
using UmaEventReaderV2.Web.Extensions;
using UmaEventReaderV2.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddUmaEventReaderServices()
    .AddSingleton<EventHubBroadcaster>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7284")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();

app.MapHub<BackendEventHub>("/events");
app.MapHub<BackendSettingsHub>("/settings");
app.MapHub<BackendUmaHub>("/umas");
app.MapHub<BackendConnectionStatusHub>("/status");

app.MapControllers();

_ = app.Services.GetRequiredService<EventHubBroadcaster>();

app.Run();