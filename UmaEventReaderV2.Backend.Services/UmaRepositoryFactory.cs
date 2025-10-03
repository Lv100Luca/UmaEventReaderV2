using System.Text.Json;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Services.Mapper;

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