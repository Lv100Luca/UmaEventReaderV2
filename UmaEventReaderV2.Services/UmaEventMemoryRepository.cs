using System.Text.Json;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services;

public class UmaEventMemoryRepository(IUmaEventJsonProvider jsonProvider) : IUmaEventRepository
{

    private Dictionary<long, UmaEventEntity> events = [];

    public async Task InitializeDataAsync()
    {
        var json = await jsonProvider.GetJsonFileAsync();

        var root = JsonSerializer.Deserialize<RootDto>(json);

        if (root is null)
            throw new Exception("Could not deserialize json");

        events = UmaEventMapper.MapFromDtos(root.ChoiceArraySchema.EventChoices);
    }



    public UmaEventEntity? GetById(long id)
    {
        var found = events.TryGetValue(id, out var match);

        return found ? match : null;
    }

    public IEnumerable<UmaEventEntity> GetAll()
    {
        return events.Values;
    }

    public IQueryable<KeyValuePair<long, UmaEventEntity>> Query()
    {
        return events.AsQueryable();
    }
}