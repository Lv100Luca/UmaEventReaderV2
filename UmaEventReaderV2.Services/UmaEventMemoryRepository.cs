using System.Text.Json;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services;

public class UmaEventMemoryRepository : IUmaEventRepository
{
    // private const string JsonFile = "umaEventData.json";

    private Dictionary<long, UmaEventEntity> events = [];

    public async Task InitializeDataAsync()
    {
        // var path = Path.Combine(AppContext.BaseDirectory, JsonFile);
        //
        // EnsureFileExistsOrThrow(path);
        // var json = File.ReadAllText(path);

        var json = await UmaEventJsonProvider.GetMappingJson();

        var root = JsonSerializer.Deserialize<RootDto>(json);

        if (root is null)
            throw new Exception("Could not deserialize json");

        events = UmaEventMapper.MapFromDtos(root.ChoiceArraySchema.EventChoices);
    }

    private static void EnsureFileExistsOrThrow(string umadbJson)
    {
        if (!File.Exists(umadbJson))
            throw new Exception($"File '{umadbJson}' does not exist.");
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