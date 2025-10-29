using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Common.Models.Enums;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Services.Extensions;
using UmaEventReaderV2.Services.Mapper;

namespace UmaEventReaderV2.Services.Repositories;

public class UmaEventMemoryRepository(ISettingsService settingsService, ILogger<UmaEventMemoryRepository> logger)
    : IUmaEventRepository
{
    private readonly List<UmaEvent> events = [];

    public void Add(UmaEvent umaEvent)
    {
        events.Add(umaEvent);
    }

    public IEnumerable<UmaEvent> GetAll()
    {
        return events;
    }

    public IQueryable<UmaEvent> Query()
    {
        return events.AsQueryable();
    }

    public IEnumerable<UmaEvent> GetAllWhereNameIsLike(string eventName)
    {
        var query = Query().WhereEventNameContains(eventName).ToList();

        if (settingsService.Settings.HighlightedSkills.Count == 0)
            return query;

        {
            var eventsWithSkillHints = query.Where(e =>
                e.Choices.SelectMany(b => b.Outcomes.SelectMany(c => c.Outcomes)).Any(g => g.Type == OutcomeType.SkillHint));

            foreach (var eventsWithSkillHint in eventsWithSkillHints)
            {
                HighlightOutcome(eventsWithSkillHint);
            }
        }

        return query;
    }

    //this should return all event except traineeEvents that arent from the passed in character
    // add service for higher level logic
    public IEnumerable<UmaEvent> GetAllForCharacterWhereNameIsLike(Uma? uma, string eventName)
    {
        var query = Query().WhereEventNameContains(eventName).ToList();

        if (uma is not null)
        {
            query = query.Where(e =>
                !e.IsTraineeEvent ||
                e.Umas.Any(u => u.Id == uma.Id)).ToList();
        }

        if (settingsService.Settings.HighlightedSkills.Count == 0)
            return query;

        {
            var eventsWithSkillHints = query.Where(e =>
                e.Choices.SelectMany(b => b.Outcomes.SelectMany(c => c.Outcomes)).Any(g => g.Type == OutcomeType.SkillHint));

            foreach (var eventsWithSkillHint in eventsWithSkillHints)
            {
                HighlightOutcome(eventsWithSkillHint);
            }
        }

        return query;
    }

    private void HighlightOutcome(UmaEvent umaEvent)
    {
        var choices = umaEvent.Choices;

        foreach (var choice in choices)
        {
            var outcomes = choice.Outcomes.SelectMany(o => o.Outcomes);

            foreach (var outcome in outcomes)
            {
                if (outcome is not SkillOutcome skillOutcome)
                    continue;

                if (settingsService.Settings.HighlightedSkills.All(s => s.Name != skillOutcome.Skill.Name))
                    continue;

                Console.Out.WriteLine("Match");

                choice.IsHighlighted = true;
            }
        }
    }
}