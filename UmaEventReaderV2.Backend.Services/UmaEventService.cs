using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Services.Extensions;

namespace UmaEventReaderV2.Services;

// public class UmaEventService(IUmaEventRepository repository) : IUmaEventService
// {
//     public IEnumerable<UmaEvent> GetAllWhereNameIsLike(string eventName)
//     {
//         return repository.Query()
//             .WhereEventNameContains(eventName);
//     }
//
//     public IEnumerable<UmaEvent> GetAll()
//     {
//         return repository.GetAll();
//     }
// }