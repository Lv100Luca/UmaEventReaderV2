namespace UmaEventReaderV2.Common.Models;

public class UmaEvent
{
    public string Name { get; init; } = string.Empty;
    public IEnumerable<Uma> Umas { get; init; } = [];

    [Obsolete("Only temporary")]
    public string CharacterName => Umas.First().FullName;

    public List<UmaEventChoice> Choices { get; init; } = [];
}