namespace UmaEventReaderV2.Common.Models;

public class Uma
{
    public string UmaName { get; set; } = string.Empty;
    public string Costume { get; set; } = string.Empty;

    override public string ToString()
    {
        return $"{UmaName} ({Costume})";
    }
}