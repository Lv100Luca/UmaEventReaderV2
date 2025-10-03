using System.Text.Json;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Services;

public class RepositoryInitializer(
    IUmaEventJsonProvider jsonProvider,
    IUmaRepository umaRepository,
    IUmaEventRepository umaEventRepository
) : IRepositoryInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var json = await jsonProvider.GetJsonFileAsync();

        var root = JsonSerializer.Deserialize<RootDto>(json);

        if (root is null)
            throw new Exception("Could not deserialize json");

        await umaRepository.InitializeAsync(root.CharacterArraySchema.Characters, cancellationToken);
        await umaEventRepository.InitializeAsync(root.ChoiceArraySchema.EventChoices, cancellationToken);
    }
}