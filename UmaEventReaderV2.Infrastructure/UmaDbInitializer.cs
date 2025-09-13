using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UmaEventReaderV2.Infrastructure.dtos;

namespace UmaEventReaderV2.Infrastructure;

public class UmaDbInitializer
{
    private const string JsonFile = "umaEventData.json";

    public async static Task InitializeAsync(bool clearDb = false, CancellationToken cancellationToken = default)
    {
        EnsureFileExistsOrThrow(JsonFile, cancellationToken);

        await using var db = new UmaDbContext();

        await EnsureDbExistsOrThrowAsync(db, cancellationToken);

        await EnsureDatabaseHasCorrectSchemaOrThrowAsync(db, cancellationToken);

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

        await EnsureTablesFilledOrThrowAsync(db, cancellationToken);
    }

    private static void EnsureFileExistsOrThrow(string umadbJson, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(umadbJson))
            throw new Exception($"File '{umadbJson}' does not exist.");
    }

    private async static Task EnsureDbExistsOrThrowAsync(UmaDbContext db, CancellationToken cancellationToken = default)
    {
        var canConnect = await db.Database.CanConnectAsync(cancellationToken);

        if (!canConnect)
            throw new Exception("Could not connect to DB");
    }

    private async static Task EnsureDatabaseHasCorrectSchemaOrThrowAsync(UmaDbContext db,
        CancellationToken cancellationToken = default)
    {
        string[] requiredTables = ["Events", "Outcomes", "Choices"];

        foreach (var table in requiredTables)
        {
            var matches = await db.Database.ExecuteSqlAsync(
                $"""
                 SELECT count(*) 
                 FROM information_schema.tables 
                 WHERE table_schema = 'public' 
                   AND table_name = '{table}'
                 """,
                cancellationToken: cancellationToken);

            if (matches == 1)
                throw new Exception($"DB has no table '{table}'. Run migrations.");
        }
    }

    private async static Task EnsureTablesFilledOrThrowAsync(UmaDbContext db, CancellationToken cancellationToken = default)
    {
        if (!await db.Events.AnyAsync(cancellationToken))
            throw new Exception("Table 'Events' has no data.");

        if (!await db.Outcomes.AnyAsync(cancellationToken))
            throw new Exception("Table 'Outcomes' has no data.");

        if (!await db.Choices.AnyAsync(cancellationToken))
            throw new Exception("Table 'Choices' has no data.");
    }
}