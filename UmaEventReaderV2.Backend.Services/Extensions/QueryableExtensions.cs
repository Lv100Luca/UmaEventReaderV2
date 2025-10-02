using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Services.Extensions;

public static class QueryableExtensions
{
    public static IEnumerable<UmaEvent> WhereEventNameContains(
        this IQueryable<KeyValuePair<long, UmaEvent>> query,
        string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return query.Select(kv => kv.Value).AsEnumerable();

        return query
            .AsEnumerable() // switch to in-memory so we can use StringComparison
            .Where(kv => !string.IsNullOrEmpty(kv.Value.Name) &&
                         kv.Value.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Value);
    }
}