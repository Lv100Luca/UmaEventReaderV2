using UmaEventReader.Abstractions.Model.Enums;
using UmaEventReaderV2.Infrastructure.dtos;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Models.Enums;

namespace UmaEventReaderV2.Infrastructure;

public static class Mapper
{
    public static UmaEventEntity ToUmaEvent(List<UmaEventChoiceDto> choices)
    {
        if (choices.Count == 0)
            throw new ArgumentException("No choices provided");

        var umaEvent = new UmaEventEntity
        {
            // clean up event name (remove characters)
            // prob best to just get rid of all (.,/!?) entirely
            EventName = choices.First().EventName,
            CharacterName = choices.First().CharacterName,
            Choices = choices.Select(ToUmaEventChoice).ToList()
        };

        // assign FK
        foreach (var choice in umaEvent.Choices)
        {
            choice.UmaEvent = umaEvent;
        }

        return umaEvent;
    }

    private static UmaEventChoiceEntity ToUmaEventChoice(UmaEventChoiceDto dto)
    {
        return new UmaEventChoiceEntity
        {
            ChoiceNumber = int.Parse(dto.ChoiceNumber),
            ChoiceText = dto.ChoiceText,
            SuccessType = Enum.TryParse<SuccessType>(dto.SuccessType, out var s) ? s : SuccessType.None,
            Outcomes = ParseOutcomes(dto.AllOutcomes)
        };
    }

    private static List<UmaEventChoiceOutcomeEntity> ParseOutcomes(string allOutcomes) =>
        allOutcomes
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(GetOutcome)
            .ToList();

    private static UmaEventChoiceOutcomeEntity GetOutcome(string outcome)
    {
        // Condition check
        if (IsCondition(outcome))
            return new UmaEventChoiceOutcomeEntity { Value = outcome, Type = OutcomeType.Condition };

        // Simple stat outcomes like "10 Speed", "30 Energy"
        var parts = outcome.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (Enum.TryParse(parts[1].Replace(" ",""), out OutcomeType type))
            return new UmaEventChoiceOutcomeEntity { Value = parts[0], Type = type };

        // Skill hints
        if (outcome.Contains("Skill Hint", StringComparison.OrdinalIgnoreCase))
            return new UmaEventChoiceOutcomeEntity { Value = outcome, Type = OutcomeType.SkillHint };

        // Fallback
        return new UmaEventChoiceOutcomeEntity { Value = outcome, Type = OutcomeType.Unknown };
    }

    private static bool IsCondition(string outcome)
    {
        outcome = outcome.Replace("(Random)", "").Trim();

        return KnownConditions.Contains(outcome);
    }

    private static bool IsStatIncrease(string outcome)
    {
        var parts = outcome.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        return Enum.TryParse(parts[1], out OutcomeType type);
    }

    private readonly static HashSet<string> KnownConditions = new(StringComparer.OrdinalIgnoreCase)
    {
        // good conditions
        "Practice Perfect ◯",
        "Practice Perfect◎",
        "Shining Brightly",
        "Charming ◯",
        "Fast Learner",
        "Hot Topic",
        // bad conditions
        "Practice Poor",
        "Under The Weather",
        "Migraine",
        "Night Owl",
        "Slow Metabolism",
        "Slacker",
        // extend with other conditions...
    };
}