// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Infrastructure;
using UmaEventReaderV2.Services;

// todo: maybe move these to the `WinForms` Project somehow
// use of overlay manager or smth
if (Environment.OSVersion.Version.Major >= 6)
    SetProcessDPIAware();

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IScreenshotAreaProvider, StaticScreenshotAreaProvider>();
// builder.Services.AddSingleton<IScreenshotAreaProvider, ScreenshotAreaSelector>();

builder.Services.AddSingleton<UmaDbContext>();
builder.Services.AddSingleton<IUmaEventRepository, UmaEventRepository>();
builder.Services.AddSingleton<IUmaEventService, UmaEventService>();
builder.Services.AddSingleton<UmaEventReader>();

var app = builder.Build();

var scope = app.Services.CreateScope();

var umaEventReader = scope.ServiceProvider.GetRequiredService<UmaEventReader>();

umaEventReader.Run();

return;

[DllImport("user32.dll")]
extern static bool SetProcessDPIAware();