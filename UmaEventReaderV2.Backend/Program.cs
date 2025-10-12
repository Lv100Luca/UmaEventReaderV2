using System.Windows.Forms;
using Humanizer;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using UmaEventReaderV2.Backend;
using UmaEventReaderV2.Backend.Extensions;
using UmaEventReaderV2.Backend.Hubs;
using UmaEventReaderV2.Common.Models;


Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, provider, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.With<ShortSourceContextEnricher>()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} - {ServiceName}] {Message:lj}{NewLine}{Exception}",
            theme: AnsiConsoleTheme.Literate
        ).Destructure.ByTransforming<UmaEventReaderSettings>(settings => new
        {
            FilteredCharacter = settings.FilteredCharacter?.FullName ?? "None",
            Interval = settings.ScanInterval.Humanize(),
            Area = settings.EventArea,
        });
});

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

var app = builder.Build();

app.UseCors();

app.MapHub<BackendEventHub>("/events");
app.MapHub<BackendSettingsHub>("/settings");
app.MapHub<BackendUmaHub>("/umas");
app.MapHub<BackendConnectionStatusHub>("/status");

_ = app.Services.GetRequiredService<EventHubBroadcaster>();

app.Run();