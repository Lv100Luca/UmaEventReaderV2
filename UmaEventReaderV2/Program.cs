// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UmaEventReaderV2;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Extensions;
using UmaEventReaderV2.Services;

// todo: maybe move these to the `WinForms` Project somehow
// use of overlay manager or smth
if (Environment.OSVersion.Version.Major >= 6)
    SetProcessDPIAware();

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var builder = Host.CreateApplicationBuilder(args);

var parsedArgs = Parser.Default.ParseArguments<ProgramArgs>(args);

builder.Services
    .AddScreenshotAreaProvider(parsedArgs)
    .AddUmaEventDbServices()
    .AddSingleton<UmaEventReader>()
    .AddSingleton<IScreenshotProvider, ScreenshotProvider>()
    .AddSingleton<ITextExtractor, TesseractTextExtractor>()
    .AddSingleton<IUmaFrontend, SpectreUmaFrontend>();

var app = builder.Build();

var scope = app.Services.CreateScope();

var umaEventReader = scope.ServiceProvider.GetRequiredService<UmaEventReader>();

await umaEventReader.RunAsync();

return;

[DllImport("user32.dll")]
extern static bool SetProcessDPIAware();