using System.Text.Json.Serialization;

namespace UmaEventReaderV2.Models.dtos;

public record UmaEventChoiceDto
(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("event_name")]
    string EventName,
    [property: JsonPropertyName("character_name")]
    string CharacterName,
    [property: JsonPropertyName("choice_text")]
    string ChoiceText,
    [property: JsonPropertyName("choice_number")]
    string ChoiceNumber,
    [property: JsonPropertyName("relation")]
    string Relation,
    [property: JsonPropertyName("relation_type")]
    string RelationType,
    [property: JsonPropertyName("success_type")]
    string SuccessType,
    [property: JsonPropertyName("all_outcomes")]
    string AllOutcomes
);