using UmaEventReaderV2.Common.Models.Enums;

namespace UmaEventReaderV2.Common.Models;

public class Uma
{
    public Guid Id { get; init; }
    public required string FullName { get; init; }

    public required string Character { get; init; }
    public required string Variant { get; init; }

    public bool SupportUma { get; init; }

    public Rarity Rarity { get; init; }

    public string ImageUrl { get; init; } = string.Empty;
}