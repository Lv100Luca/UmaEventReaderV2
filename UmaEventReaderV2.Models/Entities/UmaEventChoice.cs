using UmaEventReaderV2.Models.Enums;

namespace UmaEventReaderV2.Models.Entities;

public class UmaEventChoice
{
    public UmaEventChoiceHeader Header { get; set; }

    public int ChoiceNumber => Header.Number;
    public string ChoiceText => Header.Text;

    public Dictionary<SuccessType, List<UmaEventChoiceOutcome>> Outcomes { get; set; } = [];
}

public class UmaEventChoiceHeader
{
    public string Text { get; init; }
    public int Number { get; init; }
}

/// <summary>
/// helper wrapper for `Random 1` and `Random 2` success types, helpes with grouping
/// </summary>
/// <param name="Type">The success type</param>
/// <param name="Additional">Additional info parsed with the success type</param>
public record UmaEventChoiceSuccessType(SuccessType Type, string Additional = "")
{
    override public string ToString()
    {
        return Type != SuccessType.Random ? $"{Type}" : $"{Type} ({Additional})";
    }
}