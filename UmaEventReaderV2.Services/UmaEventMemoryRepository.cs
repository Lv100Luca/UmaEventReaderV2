using System.Text.Json;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services;

public class UmaEventMemoryRepository : IUmaEventRepository
{
    private const string JsonFile = "umaEventData.json";

    private List<UmaEventEntity> events = [];

    public UmaEventMemoryRepository()
    {
        EnsureFileExistsOrThrow(JsonFile);

        Map();
    }

    private void Map()
    {
        var json = File.ReadAllText(JsonFile);
        var root = JsonSerializer.Deserialize<RootDto>(json);

        if (root is null)
            throw new Exception("Could not deserialize json");

        var grouped = root.ChoiceArraySchema.EventChoices
            .GroupBy(c => c.EventName);

        foreach (var group in grouped)
        {
            var umaEvent = Mapper.ToUmaEvent(group.ToList());
            events.Add(umaEvent);
        }
    }

    private static void EnsureFileExistsOrThrow(string umadbJson)
    {
        if (!File.Exists(umadbJson))
            throw new Exception($"File '{umadbJson}' does not exist.");
    }

    public UmaEventEntity? GetById(long id)
    {
        var matches = events.Where(e => e.Id == id);

        if (events.Count > 1)
            throw new Exception("More than one event with same id");

        return matches.FirstOrDefault();
    }

    public IEnumerable<UmaEventEntity> GetAll()
    {
        return events;
    }

    public IQueryable<UmaEventEntity> Query()
    {
        return events.AsQueryable();
    }
}