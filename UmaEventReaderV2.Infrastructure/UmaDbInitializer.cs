using System.Text.Json;
using UmaEventReaderV2.Infrastructure.dtos;

namespace UmaEventReaderV2.Infrastructure;

public class UmaDbInitializer
{
    private const string JsonFile = "umaEventData.json";

    public async static Task InitializeAsync(bool clearDb = false, CancellationToken cancellationToken = default)
    {
        await using var db = new UmaContext();

        if (clearDb)
        {
            await db.Database.EnsureDeletedAsync(cancellationToken);
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        var json = await File.ReadAllTextAsync(JsonFile, cancellationToken);
        var root = JsonSerializer.Deserialize<RootDto>(json);

        if (root == null)
            return;

        // Group by EventName
        var grouped = root.ChoiceArraySchema.EventChoices
            .GroupBy(c => c.EventName);

        foreach (var group in grouped)
        {
            var umaEvent = Mapper.ToUmaEvent(group.ToList());
            db.Events.Add(umaEvent);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}