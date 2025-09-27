namespace UmaEventReaderV2.Abstractions;

public interface IUmaEventJsonProvider
{
    Task<string> GetJsonFileAsync();
}