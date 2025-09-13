using Microsoft.EntityFrameworkCore;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Infrastructure;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Services.Extensions;

namespace UmaEventReaderV2.Services;

public class UmaEventRepository(UmaDbContext db) : IUmaEventRepository
{
    public IEnumerable<UmaEventEntity> GetAll()
    {
        return db.Events.ToList();
    }

    public IEnumerable<UmaEventEntity> GetAllByName(string eventName)
    {
        return db.Events
            .AsNoTracking() // loads the db entries as 'read only' -> changes wont be reflected in the db
            .WhereEventNameContains(eventName)
            .Include(e => e.Choices)
            .ThenInclude(c => c.Outcomes);
    }

    public IEnumerable<UmaEventEntity> GetAllByChoiceText(string choiceText)
    {
        return db.Events
            .AsNoTracking()
            .Where(e => e.Choices.Any(c => EF.Functions.ILike(c.ChoiceText, $"%{choiceText}%")))
            .Include(e => e.Choices)
            .ThenInclude(c => c.Outcomes);
    }
}