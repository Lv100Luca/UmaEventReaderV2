using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaEventRepository
{
    Task InitializeDataAsync();

    UmaEvent? GetById(long id);
    IEnumerable<UmaEvent> GetAll();
    IQueryable<KeyValuePair<long, UmaEvent>> Query();
}