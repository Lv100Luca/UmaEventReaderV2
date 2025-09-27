using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services.Extensions;

public static class QueryableExtensions
{
    public static IEnumerable<UmaEventEntity> WhereEventNameContains(
        this IQueryable<KeyValuePair<long, UmaEventEntity>> query, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return query.Select(kv => kv.Value).AsEnumerable();

        return query
            .AsEnumerable() // switch to in-memory so we can use StringComparison
            .Where(kv => !string.IsNullOrEmpty(kv.Value.EventName) &&
                         kv.Value.EventName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Value);
    }
}
