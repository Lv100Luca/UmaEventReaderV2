using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Services.Extensions;

public static class QueryableExtensions
{
    public static IEnumerable<UmaEvent> WhereEventNameContains(
        this IQueryable<UmaEvent> query,
        string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return query.AsEnumerable();

        return query
            .AsEnumerable() // switch to in-memory so we can use StringComparison
            .Where(umaEvent => !string.IsNullOrEmpty(umaEvent.Name) &&
                               umaEvent.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}