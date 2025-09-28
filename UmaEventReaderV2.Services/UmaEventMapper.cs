using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Models.Enums;

namespace UmaEventReaderV2.Services;

public static class UmaEventMapper
{
    public static Dictionary<long, UmaEvent> MapFromDtos(IEnumerable<UmaEventChoiceDto> dtos)
    {
        var grouped = dtos.GroupBy(d => (d.EventName, d.CharacterName));
        var eventsDict = new Dictionary<long, UmaEvent>();

        foreach (var group in grouped)
        {
            if (!long.TryParse(group.First().Id, out var eventId))
                throw new InvalidOperationException($"Invalid Event Id: {group.First().Id}");

            var ev = new UmaEvent
            {
                Name = group.Key.EventName,
                CharacterName = group.Key.CharacterName,
                Choices = []
            };

            var choicesGrouped = group.GroupBy(d => (d.ChoiceNumber, d.ChoiceText));

            foreach (var choiceGroup in choicesGrouped)
            {
                if (!int.TryParse(choiceGroup.Key.ChoiceNumber, out var choiceNum))
                    throw new InvalidOperationException($"Invalid Choice Number: {choiceGroup.Key.ChoiceNumber}");

                var choice = new UmaEventChoice
                {
                    Header = new UmaEventChoiceHeader
                    {
                        Number = choiceNum,
                        Text = choiceGroup.Key.ChoiceText
                    },
                    Outcomes = new Dictionary<SuccessType, List<UmaEventChoiceOutcome>>()
                };

                foreach (var dto in choiceGroup)
                {
                    var successType = Enum.TryParse(dto.SuccessType, true, out SuccessType parsed)
                        ? parsed
                        : SuccessType.None;

                    var outcomes = ParseOutcomes(dto.AllOutcomes);

                    if (!choice.Outcomes.ContainsKey(successType))
                        choice.Outcomes[successType] = new List<UmaEventChoiceOutcome>();

                    choice.Outcomes[successType].AddRange(outcomes);
                }

                ev.Choices.Add(choice);
            }

            eventsDict[eventId] = ev;
        }

        Console.Out.WriteLine($"Successfully loaded {eventsDict.Count} events from json");
        return eventsDict;
    }

    private static List<UmaEventChoiceOutcome> ParseOutcomes(string allOutcomes)
    {
        var outcomes = new List<UmaEventChoiceOutcome>();
        if (string.IsNullOrWhiteSpace(allOutcomes)) return outcomes;

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
