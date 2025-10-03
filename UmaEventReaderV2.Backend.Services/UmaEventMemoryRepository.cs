using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Services.Extensions;
using UmaEventReaderV2.Services.Mapper;

namespace UmaEventReaderV2.Services;

public class UmaEventMemoryRepository(IUmaRepository umaRepository, ILogger<UmaEventMemoryRepository> logger)
    : IUmaEventRepository
{
    private readonly List<UmaEvent> events = [];

    // move to initializer class?
    public async Task InitializeAsync(IEnumerable<UmaEventChoiceDto> dtos, CancellationToken cancellationToken = default)
    {
        var eventGroups = dtos.GroupBy(e => e.EventName);

        foreach (var group in eventGroups)
        {
            var characterName = group.First().CharacterName;

            var names = Regex
                .Split(characterName, @",(?![^(]*\))")
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray();

            var umas = FindOrCreate(names);

            var mappedEvent = UmaEventMapperV2.Map(group, umas);

            events.Add(mappedEvent);
        }

        logger.LogSuccess($"Event repository initialized with {events.Count} events.");
    }

    public IEnumerable<UmaEvent> GetAll()
    {
        return events;
    }

    public IQueryable<UmaEvent> Query()
    {
        return events.AsQueryable();
    }

    public IEnumerable<UmaEvent> GetAllWhereNameIsLike(string eventName)
    {
        return Query().WhereEventNameContains(eventName);
    }

    //this should return all event except traineeEvents that arent from the passed in character
    public IEnumerable<UmaEvent> GetAllForCharacterWhereNameIsLike(Uma? uma, string eventName)
    {
        var query = Query().WhereEventNameContains(eventName);

        if (uma is not null)
        {
            query = query.Where(e =>
                !e.IsTraineeEvent ||
                e.Umas.Any(u => u.Id == uma.Id));
        }

        return query;
    }

    private IEnumerable<Uma> FindOrCreate(string[] names)
    {
        foreach (var name in names)
        {
            var foundUma = umaRepository.GetByFullName(name);

            if (foundUma == null)
            {
                foundUma = UmaMapper.Map(name, supportUma: true);

                umaRepository.TryAddSupportUma(foundUma);
            }

            yield return foundUma;
        }
    }
}