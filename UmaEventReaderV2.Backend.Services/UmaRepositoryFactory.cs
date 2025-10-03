namespace UmaEventReaderV2.Services;

// public class UmaRepositoryFactory(IUmaEventJsonProvider jsonProvider, ILogger<UmaRepository> logger) : IRepositoryFactory<IUmaRepository>
// {
//     public async Task<IUmaRepository> CreateAsync()
//     {
//         var json = await jsonProvider.GetJsonFileAsync();
//
//         var root = JsonSerializer.Deserialize<RootDto>(json);
//
//         if (root is null)
//             throw new Exception("Could not deserialize json");
//
//         var umas = new HashSet<IUma>(
//             root.CharacterArraySchema.Characters.Select(UmaMapper.Map)
//         );
//
//         return new UmaRepository(umas, logger);
//     }
// }