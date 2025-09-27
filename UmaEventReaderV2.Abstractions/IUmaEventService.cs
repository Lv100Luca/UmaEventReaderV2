using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaEventService
{
    Task InitializeDataAsync();

    IEnumerable<UmaEventEntity> GetAllWhereNameIsLike(string eventName);
}