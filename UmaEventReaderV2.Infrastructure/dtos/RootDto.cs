using System.Text.Json.Serialization;

namespace UmaEventReaderV2.Infrastructure.dtos;

public record RootDto(
    [property: JsonPropertyName("toolKey")]
    string ToolKey,
    [property: JsonPropertyName("headingText")]
    string HeadingText,
    [property: JsonPropertyName("choiceArraySchema")]
    ChoiceArraySchemaDto ChoiceArraySchema
);

public record ChoiceArraySchemaDto(
    [property: JsonPropertyName("relationalDamreyDbSchema")]
    RelationalDamreyDbSchemaDto RelationalDamreyDbSchemaDto,
    [property: JsonPropertyName("choices")]
    List<UmaEventChoiceDto> EventChoices
);

public record RelationalDamreyDbSchemaDto(
    [property: JsonPropertyName("category_relation_id")]
    int? CategoryRelationId,
    [property: JsonPropertyName("category_db_id")]
    int CategoryDbId
);