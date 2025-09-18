using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaFrontend
{
    Task ShowEventAsync(UmaEventEntity umaEvent);
    Task ShowCareerAsync(string careerInfo);
    Task LogAsync(string message);
    string GetSearchQuery();
    void ResetSearchQuery();
    bool IsSearching();
}