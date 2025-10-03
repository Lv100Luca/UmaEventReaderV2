// using System.Text.Json;
// using System.Text.RegularExpressions;
// using Microsoft.Extensions.Logging;
// using UmaEventReaderV2.Abstractions;
// using UmaEventReaderV2.Common;
// using UmaEventReaderV2.Common.Models;
// using UmaEventReaderV2.Models.dtos;
// using UmaEventReaderV2.Services.Mapper;
//
// namespace UmaEventReaderV2.Services;
//
// public class UmaEventRepositoryFactory(IUmaRepository umaRepository, IUmaEventJsonProvider jsonProvider, ILogger<IUmaEventRepository> logger) : IRepositoryFactory<IUmaEventRepository>
// {
//     public async Task<IUmaEventRepository> CreateAsync()
//     {
//         var events = new List<UmaEvent>();
//
//         var json = await jsonProvider.GetJsonFileAsync();
//
//         var root = JsonSerializer.Deserialize<RootDto>(json);
//
//         if (root is null)
//             throw new Exception("Could not deserialize json");
//
//         var dtos = root.ChoiceArraySchema.EventChoices;
//
//         var eventGroups = dtos.GroupBy(e => e.EventName);
//
//         foreach (var group in eventGroups)
//         {
//             var characterName = group.First().CharacterName;
//
//             var names = Regex
//                 .Split(characterName, @",(?![^(]*\))")
//                 .Select(p => p.Trim())
//                 .Where(p => !string.IsNullOrEmpty(p))
//                 .ToArray();
//
//             var umas = FindOrCreate(names);
//
//             var mappedEvent = UmaEventMapperV2.Map(group, umas);
//
//             events.Add(mappedEvent);
//         }
//
//         logger.LogSuccess($"Event repository initialized with {events.Count} events.");
//
//         return new UmaEventMemoryRepository(events, logger);
//     }
//
//
//     private IEnumerable<IUma> FindOrCreate(string[] names)
//     {
//         foreach (var name in names)
//         {
//             var foundUma = umaRepository.GetByFullName(name);
//
//             if (foundUma == null)
//             {
//                 foundUma = UmaMapper.Map(name);
//
//                 umaRepository.TryAddSupportUma(foundUma);
//             }
//
//             yield return foundUma;
//         }
//     }
// }