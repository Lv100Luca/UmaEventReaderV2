using Microsoft.EntityFrameworkCore;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Infrastructure;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Services.Extensions;

namespace UmaEventReaderV2.Services;

public class UmaEventRepository(UmaDbContext db) : IUmaEventRepository
{
    public UmaEventEntity? GetById(long id)
    {
        return db.Events.Find(id);
    }

    public IEnumerable<UmaEventEntity> GetAll()
    {
        return db.Events.ToList();
    }

    public IQueryable<UmaEventEntity> Query()
    {
        return db.Events.AsQueryable();
    }
}


// }