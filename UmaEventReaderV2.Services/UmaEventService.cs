using Microsoft.EntityFrameworkCore;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Services.Extensions;

namespace UmaEventReaderV2.Services;

public class UmaEventService(IUmaEventRepository repository) : IUmaEventService
{
    public async Task InitializeDataAsync()
    {
        await repository.InitializeDataAsync();
    }

    public IEnumerable<UmaEventEntity> GetAllWhereNameIsLike(string eventName)
     {
         return repository.Query()
             .WhereEventNameContains(eventName);
     }
}