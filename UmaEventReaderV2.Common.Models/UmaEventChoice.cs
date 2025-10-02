using UmaEventReaderV2.Common.Models.Enums;

namespace UmaEventReaderV2.Common.Models;

public class UmaEventChoice
{
    public UmaEventChoiceHeader Header { get; set; }

    public int ChoiceNumber => Header.Number;
    public string ChoiceText => Header.Text;

    public List<UmaEventChoiceOutcomeGroup> Outcomes { get; set; } = [];
}

public class UmaEventChoiceHeader
{
    public string Text { get; init; }
    public int Number { get; init; }
}

/// <summary>
/// helper wrapper for `Random 1` and `Random 2` success types, helpes with grouping
/// </summary>
public class UmaEventChoiceSuccessType
{
    public SuccessType Type { get; init; }

    public string Additional { get; init; } = "";

    override public string ToString()
    {
        return Type != SuccessType.Random ? $"{Type}" : $"{Type} ({Additional})";
    }
}

public class UmaEventChoiceOutcomeGroup
{
    public UmaEventChoiceSuccessType SuccessType { get; set; } = default!;
    public List<UmaEventChoiceOutcome> Outcomes { get; set; } = new();
}