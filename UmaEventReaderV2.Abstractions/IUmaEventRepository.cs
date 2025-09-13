using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaEventRepository
{
    UmaEventEntity? GetById(long id);
    IEnumerable<UmaEventEntity> GetAll();
    IQueryable<UmaEventEntity> Query();
}