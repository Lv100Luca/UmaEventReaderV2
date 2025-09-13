// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Infrastructure;
using UmaEventReaderV2.Services;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSingleton<UmaDbContext>();
builder.Services.AddSingleton<IUmaEventRepository, UmaEventRepository>();
builder.Services.AddSingleton<IUmaEventService, UmaEventService>();
builder.Services.AddSingleton<UmaEventReader>();

var app = builder.Build();

var scope = app.Services.CreateScope();

var umaEventReader = scope.ServiceProvider.GetRequiredService<UmaEventReader>();

umaEventReader.Run();