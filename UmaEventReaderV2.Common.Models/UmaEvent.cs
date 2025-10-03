namespace UmaEventReaderV2.Common.Models;

public class UmaEvent
{
    public string Name { get; init; } = string.Empty;
    public IEnumerable<Uma> Umas { get; init; } = [];

    public List<UmaEventChoice> Choices { get; init; } = [];

    public bool IsTraineeEvent => !Umas.Any(u => u.SupportUma);
}