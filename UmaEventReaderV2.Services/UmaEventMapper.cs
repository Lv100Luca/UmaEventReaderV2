using System;
using System.Collections.Generic;
using System.Linq;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Models.Enums;

namespace UmaEventReaderV2.Services;

public static class UmaEventMapper
{
    public static Dictionary<long, UmaEventEntity> MapFromDtos(IEnumerable<UmaEventChoiceDto> dtos)
    {
        var grouped = dtos.GroupBy(d => (d.EventName, d.CharacterName));
        var eventsDict = new Dictionary<long, UmaEventEntity>();

        foreach (var group in grouped)
        {
            if (!long.TryParse(group.First().Id, out var eventId))
                throw new InvalidOperationException($"Invalid Event Id: {group.First().Id}");

            var eventEntity = new UmaEventEntity
            {
                Id = eventId,
                EventName = group.Key.EventName,
                CharacterName = group.Key.CharacterName,
                Choices = new Dictionary<long, UmaEventChoiceEntity>()
            };

            foreach (var dto in group)
            {
                if (!int.TryParse(dto.ChoiceNumber, out var choiceId))
                    throw new InvalidOperationException($"Invalid Choice Number: {dto.ChoiceNumber}");

                if (!long.TryParse(dto.Id, out var id))
                    throw new InvalidOperationException($"Invalid Choice Id: {dto.Id}");

                var choiceEntity = new UmaEventChoiceEntity
                {
                    Id = id,
                    ChoiceNumber = choiceId,
                    ChoiceText = dto.ChoiceText,
                    SuccessType = Enum.TryParse(dto.SuccessType, out SuccessType s) ? s : SuccessType.None,
                    Outcomes = new Dictionary<long, UmaEventChoiceOutcomeEntity>()
                };

                eventEntity.Choices[id] = choiceEntity;

                // Split the outcomes of this DTO into individual entries
                var newOutcomes = ParseOutcomes(dto.AllOutcomes);

                // Add each outcome with a unique ID
                var maxId = choiceEntity.Outcomes.Count > 0 ? choiceEntity.Outcomes.Keys.Max() : 0;

                foreach (var kv in newOutcomes)
                {
                    choiceEntity.Outcomes[++maxId] = kv.Value;
                }
            }

            eventsDict[eventId] = eventEntity;
        }

        return eventsDict;
    }

    private static Dictionary<long, UmaEventChoiceOutcomeEntity> ParseOutcomes(string allOutcomes)
    {
        var outcomes = new Dictionary<long, UmaEventChoiceOutcomeEntity>();
        if (string.IsNullOrWhiteSpace(allOutcomes)) return outcomes;

        var parts = allOutcomes.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        long idCounter = 0;

        foreach (var part in parts)
        {
            idCounter++;
            outcomes[idCounter] = GetOutcome(part, idCounter);
        }

        return outcomes;
    }

    private static UmaEventChoiceOutcomeEntity GetOutcome(string outcome, long id)
    {
        if (IsCondition(outcome))
            return new UmaEventChoiceOutcomeEntity { Id = id, Value = outcome, Type = OutcomeType.Condition };

        var parts = outcome.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 && Enum.TryParse(parts[1].Replace(" ", ""), true, out OutcomeType type))
            return new UmaEventChoiceOutcomeEntity { Id = id, Value = parts[0], Type = type };

        if (outcome.Contains("Skill Hint", StringComparison.OrdinalIgnoreCase))
            return new UmaEventChoiceOutcomeEntity { Id = id, Value = outcome, Type = OutcomeType.SkillHint };

        return new UmaEventChoiceOutcomeEntity { Id = id, Value = outcome, Type = OutcomeType.Unknown };
    }

    private static bool IsCondition(string outcome)
    {
        outcome = outcome.Replace("(Random)", "").Trim();
        return KnownConditions.Contains(outcome);
    }

    private readonly static HashSet<string> KnownConditions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Practice Perfect ◯",
        "Practice Perfect◎",
        "Shining Brightly",
        "Charming ◯",
        "Fast Learner",
        "Hot Topic",
        "Practice Poor",
        "Under The Weather",
        "Migraine",
        "Night Owl",
        "Slow Metabolism",
        "Slacker",
    };
}
