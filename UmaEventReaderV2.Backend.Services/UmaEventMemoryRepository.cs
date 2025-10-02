using System.Text.Json;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.dtos;
using UmaEvent = UmaEventReaderV2.Models.Entities.UmaEvent;

namespace UmaEventReaderV2.Services;

public class UmaEventMemoryRepository(IUmaEventJsonProvider jsonProvider) : IUmaEventRepository
{
    private Dictionary<long, UmaEvent> events = [];

    public async Task InitializeDataAsync()
    {
        var json = await jsonProvider.GetJsonFileAsync();

        var root = JsonSerializer.Deserialize<RootDto>(json);

        if (root is null)
            throw new Exception("Could not deserialize json");

        events = UmaEventMapper.MapFromDtos(root.ChoiceArraySchema.EventChoices);
    }

    public UmaEvent? GetById(long id)
    {
        var found = events.TryGetValue(id, out var match);

        return found ? match : null;
    }

    public IEnumerable<UmaEvent> GetAll()
    {
        return events.Values;
    }

    public IQueryable<KeyValuePair<long, UmaEvent>> Query()
    {
        return events.AsQueryable();
    }
}