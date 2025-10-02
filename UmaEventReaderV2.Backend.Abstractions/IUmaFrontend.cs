using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaFrontend
{
    Task ShowEventAsync(UmaEvent umaEvent);
    Task ShowCareerAsync(string careerInfo);
    Task LogAsync(string message);
    string GetSearchQuery();
    void ResetSearchQuery();
    bool IsSearching();
}