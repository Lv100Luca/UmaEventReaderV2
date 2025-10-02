namespace UmaEventReaderV2.Common.Models;

public class UmaEvent
{
    public string CharacterName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public List<UmaEventChoice> Choices { get; init; } = [];
}