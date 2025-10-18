using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Common.Models.Enums;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Services;

public class UmaEventMapper(IUmaRepository umaRepository)
{
    public Dictionary<long, UmaEvent> MapFromDtos(IEnumerable<UmaEventChoiceDto> dtos)
    {
        var grouped = dtos.GroupBy(d => (d.EventName, d.CharacterName));
        var eventsDict = new Dictionary<long, UmaEvent>();

        foreach (var group in grouped)
        {
            if (!long.TryParse(group.First().Id, out var eventId))
                throw new InvalidOperationException($"Invalid Event Id: {group.First().Id}");

            var umaEvent = new UmaEvent
            {
                Name = group.Key.EventName,
                Umas = [],
                // CharacterName = group.Key.CharacterName,
                Choices = []
            };

            foreach (var dto in group)
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
                        Outcomes = []
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
                        Outcomes = new List<UmaEventChoiceOutcome>()
                    };

                    choice.Outcomes.Add(outcomeGroup);
                }

                outcomeGroup.Outcomes.AddRange(parsedOutcomes);
            }

            eventsDict[eventId] = umaEvent;
        }

        return eventsDict;
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

    private static List<UmaEventChoiceOutcome> ParseOutcomes(string allOutcomes)
    {
        var outcomes = new List<UmaEventChoiceOutcome>();

        if (string.IsNullOrWhiteSpace(allOutcomes))
            return outcomes;

        var parts = allOutcomes.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            outcomes.Add(GetOutcome(part));
        }

        return outcomes;
    }

    private static UmaEventChoiceOutcome GetOutcome(string outcome)
    {
        if (IsGoodCondition(outcome))
            return new UmaEventChoiceOutcome { Value = outcome, Type = OutcomeType.GoodCondition };

        if (IsBadCondition(outcome))
            return new UmaEventChoiceOutcome { Value = outcome, Type = OutcomeType.BadCondition };

        var parts = outcome.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2 && Enum.TryParse(parts[1].Replace(" ", ""), true, out OutcomeType type))
            return new UmaEventChoiceOutcome { Value = parts[0], Type = type };

        if (outcome.Contains("Skill Hint", StringComparison.OrdinalIgnoreCase))
            return new UmaEventChoiceOutcome { Value = outcome, Type = OutcomeType.SkillHint };

        if (outcome.Contains("End of Chain Event", StringComparison.OrdinalIgnoreCase))
            return new UmaEventChoiceOutcome { Value = outcome, Type = OutcomeType.EndOfEventChain };

        return new UmaEventChoiceOutcome { Value = outcome, Type = OutcomeType.Unknown };
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
}