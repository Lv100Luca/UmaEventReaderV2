using System.Text.Json.Serialization;

namespace UmaEventReaderV2.Models.dtos;

public class UmaDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("rarity")] public string Rarity { get; set; } = string.Empty;

    [JsonPropertyName("image_url")] public string ImageUrl { get; set; } = string.Empty;
}