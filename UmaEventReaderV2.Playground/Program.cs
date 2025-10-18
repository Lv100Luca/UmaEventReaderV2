// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Common.Models.Enums;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Services;
using UmaEventReaderV2.Services.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IUmaEventJsonProvider, StaticUmaEventJsonProvider>();
builder.Services.AddSingleton<IUmaEventRepository, UmaEventMemoryRepository>();
builder.Services.AddSingleton<IUmaRepository, UmaRepository>();
builder.Services.AddSingleton<IRepositoryInitializer, RepositoryInitializer>();

var host = builder.Build();

var eventRepo = host.Services.GetRequiredService<IUmaEventRepository>();
var umaRepo = host.Services.GetRequiredService<IUmaRepository>();
var jsonProvider = host.Services.GetRequiredService<IUmaEventJsonProvider>();

var json = await jsonProvider.GetJsonFileAsync();

var root = JsonSerializer.Deserialize<RootDto>(json);

if (root is null)
    throw new Exception("Could not deserialize json");

await umaRepo.InitializeAsync(root.CharacterArraySchema.Characters);
await eventRepo.InitializeAsync(root.ChoiceArraySchema.EventChoices);

var allEvents = eventRepo.GetAll();

var eventsWithSkillHints = allEvents.Where(e =>
    e.Choices.SelectMany(b => b.Outcomes.SelectMany(c => c.Outcomes)).Any(g => g.Type == OutcomeType.SkillHint));

var eventSkills = new Dictionary<UmaEvent, List<string>>();

foreach (var e in eventsWithSkillHints)
{
    var skillHintOutcome = e.Choices.SelectMany(b => b.Outcomes.SelectMany(c => c.Outcomes))
        .Where(g => g.Type == OutcomeType.SkillHint);

    var skills = skillHintOutcome.Select(GetSkill).Where(s => !string.IsNullOrWhiteSpace(s));

    eventSkills.Add(e, skills.ToList()!);
}

foreach (var (e, skills) in eventSkills)
{
    Console.Out.WriteLine($"Event: '{e.Name}':");

    foreach (var skill in skills)
    {
        Console.Out.WriteLine($"  Skill: '{skill}'");
    }
}

return 0;

string? GetSkill(UmaEventChoiceOutcome umaEventChoiceOutcome)
{
    // split into parts
    var skillHintParts = umaEventChoiceOutcome.Value.Split('+', 2, StringSplitOptions.RemoveEmptyEntries);

    var skillHint = skillHintParts[0].Replace("Skill Hint", "").Replace("(Random)", "").Trim();

    return string.IsNullOrWhiteSpace(skillHint) ? null : skillHint;
}