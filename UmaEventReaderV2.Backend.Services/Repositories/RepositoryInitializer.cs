using System.Text.Json;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Services.Mapper;

namespace UmaEventReaderV2.Services.Repositories;

public class RepositoryInitializer(
    IUmaEventJsonProvider jsonProvider,
    UmaEventMapperV2 eventMapperV2,
    IUmaRepository umaRepository,
    ILogger<RepositoryInitializer> logger
) : IRepositoryInitializer
{
    public async Task InitializeAsync()
    {
        var json = await jsonProvider.GetJsonFileAsync();

        var root = JsonSerializer.Deserialize<RootDto>(json);

        if (root is null)
            throw new Exception("Could not deserialize json");

        // umas
        foreach (var uma in root.CharacterArraySchema.Characters.Select(UmaMapper.Map))
            umaRepository.Add(uma);

        logger.LogInformation($"UmaRepository initialized with {umaRepository.GetAll().Count()} umas.");

        // events
        eventMapperV2.MapEvents(root.ChoiceArraySchema.EventChoices);
    }
}