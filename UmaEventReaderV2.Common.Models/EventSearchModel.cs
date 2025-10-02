namespace UmaEventReaderV2.Common.Models;

public class EventSearchModel
{
    public string Name { get; set; } = string.Empty;
    public SearchMode SearchMode { get; set; }
}

public enum SearchMode
{
    Contains,
    StartsWith
}