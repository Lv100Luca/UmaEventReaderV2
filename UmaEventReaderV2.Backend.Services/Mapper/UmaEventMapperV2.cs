using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Common.Models.Enums;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Services.Mapper;

public class UmaEventMapperV2
(
    IUmaRepository umaRepository,
    IUmaEventRepository umaEventRepository,
    IUmaSkillRepository umaSkillRepository,
    ILogger<UmaEventMapperV2> logger)
{
    public void MapEvents(IEnumerable<UmaEventChoiceDto> choices)
    {
        var eventGroups = choices.GroupBy(e => e.EventName);

        foreach (var group in eventGroups)
        {
            var characterName = group.First().CharacterName;

            var names = Regex
                .Split(characterName, @",(?![^(]*\))")
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray();

            var umas = umaRepository.FindOrCreate(names).ToList();

            // fix for error in dataset
            if (umas.Any(u => u.FullName == "Extra Training (Taiki Shuttle)"))
                umas = [umaRepository.FindOrCreate("Taiki Shuttle (Wild Frontier)")];

            var mappedEvent = Map(group, umas);

            umaEventRepository.Add(mappedEvent);
        }

        logger.LogInformation("Event repository initialized with {Count} events.", umaEventRepository.GetAll().Count());
    }

    private UmaEvent Map(IGrouping<string, UmaEventChoiceDto> grouping, IEnumerable<Uma> umas)
    {
        var umaEvent = new UmaEvent
        {
            Name = grouping.Key,
            Choices = [],
            Umas = umas,
        };

        foreach (var dto in grouping)
        {
            if (!int.TryParse(dto.ChoiceNumber, out var choiceNumber))
                throw new InvalidOperationException($"Invalid Choice Number: {dto.ChoiceNumber}");

            var successType = ParseSuccessType(dto.SuccessType);

            // Find existing choice or create a new one
            var choice = umaEvent.Choices
                .FirstOrDefault(c => c.ChoiceNumber == choiceNumber && c.ChoiceText == dto.ChoiceText);

            if (choice == null)
            {
                choice = new UmaEventChoice
                {
                    Header = new UmaEventChoiceHeader
                    {
                        Number = choiceNumber,
                        Text = dto.ChoiceText
                    },
                    Outcomes = new List<UmaEventChoiceOutcomeGroup>()
                };

                umaEvent.Choices.Add(choice);
            }

            // Parse the outcomes from the DTO
            var parsedOutcomes = ParseOutcomes(dto.AllOutcomes);

            // Find existing outcome group for this success type or create a new one
            var outcomeGroup = choice.Outcomes.FirstOrDefault(g => g.SuccessType.Type == successType.Type &&
                                                                   g.SuccessType.Additional == successType.Additional);

            if (outcomeGroup == null)
            {
                outcomeGroup = new UmaEventChoiceOutcomeGroup
                {
                    SuccessType = successType,
                    Outcomes = []
                };

                choice.Outcomes.Add(outcomeGroup);
            }

            outcomeGroup.Outcomes.AddRange(parsedOutcomes);
        }

        return umaEvent;
    }


    private static UmaEventChoiceSuccessType ParseSuccessType(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw == "-")
            return new UmaEventChoiceSuccessType { Type = SuccessType.None };

        var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            return new UmaEventChoiceSuccessType
            {
                Type = SuccessType.Random,
                Additional = parts[1]
            };
        }

        return Enum.TryParse<SuccessType>(raw, true, out var parsed)
            ? new UmaEventChoiceSuccessType { Type = parsed }
            : throw new InvalidOperationException($"Invalid Success Type: {raw}");
    }

    private List<EventChoiceOutcome> ParseOutcomes(string allOutcomes)
    {
        var outcomes = new List<EventChoiceOutcome>();

        if (string.IsNullOrWhiteSpace(allOutcomes))
            return outcomes;

        var parts = allOutcomes.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            var outcome = GetOutcome(part);

            if (outcome is not null)
                outcomes.Add(outcome);
        }

        return outcomes;
    }

    private EventChoiceOutcome? GetOutcome(string outcome)
    {
        if (IsGoodCondition(outcome))
            return new UmaEventChoiceOutcome(outcome, OutcomeType.GoodCondition);

        if (IsBadCondition(outcome))
            return new UmaEventChoiceOutcome(outcome, OutcomeType.BadCondition);

        var parts = outcome.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2 && Enum.TryParse(parts[1].Replace(" ", ""), true, out OutcomeType type))
            return new UmaEventChoiceOutcome(parts[0], type);

        if (outcome.Contains("Skill Hint", StringComparison.OrdinalIgnoreCase))
            return MapSkill(outcome);

        if (outcome.Contains("End of Chain Event", StringComparison.OrdinalIgnoreCase))
            return new UmaEventChoiceOutcome(outcome, OutcomeType.EndOfEventChain);

        return new UmaEventChoiceOutcome(outcome, OutcomeType.Unknown);
    }

    private static bool IsGoodCondition(string outcome)
    {
        outcome = outcome.Replace("(Random)", "").Trim();

        return KnownGoodConditions.Contains(outcome);
    }

    private static bool IsBadCondition(string outcome)
    {
        outcome = outcome.Replace("(Random)", "").Trim();

        return KnownBadConditions.Contains(outcome);
    }

    private readonly static HashSet<string> KnownGoodConditions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Practice Perfect ◯",
        "Practice Perfect◎",
        "Shining Brightly",
        "Charming ◯",
        "Fast Learner",
        "Hot Topic",
    };

    private readonly static HashSet<string> KnownBadConditions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Practice Poor",
        "Under The Weather",
        "Migraine",
        "Night Owl",
        "Slow Metabolism",
        "Slacker",
    };

    private SkillOutcome? MapSkill(string umaEventChoiceOutcome)
    {
        // split into parts
        var skillHintParts = umaEventChoiceOutcome.Split('+', 2, StringSplitOptions.RemoveEmptyEntries);

        var skillHint = skillHintParts[0].Replace("Skill Hint", "").Replace("(Random)", "").Trim();

        var hintCount = 1;

        if (skillHintParts.Length == 2)
        {
            var parts = skillHintParts[1];
            var hints = parts.Split(" ")[0];

            hintCount = int.TryParse(hints, out var count) ? count : 1;
        }

        var skillName = string.IsNullOrWhiteSpace(skillHint) ? null : skillHint;

        if (skillName == null)
            return null;

        Console.Out.WriteLine("Found skillname: " + skillName + "+" + hintCount);

        var skill =  new UmaSkill
        {
            Name = skillName,
        };

        umaSkillRepository.Add(skill);

        return new SkillOutcome(skill, hintCount);
    }
}