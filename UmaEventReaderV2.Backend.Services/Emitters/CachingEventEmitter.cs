using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Services.Emitters;

/// <summary>
/// This emitter caches the result, should the same event be requested to be emitted, wait a set amount until reemit
/// </summary>
public class CachingEventEmitter(ILogger<CachingEventEmitter> logger) : IEventEmitter
{
    private readonly TimeSpan cacheTime = TimeSpan.FromSeconds(3);

    public event Action<EventBatch>? OnEventFound;

    private DateTime lastEmittedTime = DateTime.MinValue;
    private EventBatch? lastEmittedBatch;

    public async Task EmitEventsAsync(IEnumerable<UmaEvent> events)
    {
        var batch = EventBatch.From(events);

        if (batch.Equals(lastEmittedBatch) &&
            DateTime.Now - lastEmittedTime < cacheTime)
            return;

        lastEmittedTime = DateTime.Now;
        lastEmittedBatch = batch;

        logger.LogInformation("emitting event " + events.Count());

        OnEventFound?.Invoke(batch);
    }
}