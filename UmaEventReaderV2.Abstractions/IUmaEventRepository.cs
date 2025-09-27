using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaEventRepository
{
    Task InitializeDataAsync();

    UmaEventEntity? GetById(long id);
    IEnumerable<UmaEventEntity> GetAll();
    IQueryable<KeyValuePair<long, UmaEventEntity>> Query();
}