using Microsoft.EntityFrameworkCore;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services.Extensions;

public static class QueryableExtensions {
    public static IQueryable<UmaEventEntity> WhereEventNameContains(
        this IQueryable<UmaEventEntity> query, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return query;

        return query.Where(e => e.EventName.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}