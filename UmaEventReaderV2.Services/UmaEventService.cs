using Microsoft.EntityFrameworkCore;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Services.Extensions;

namespace UmaEventReaderV2.Services;

public class UmaEventService(IUmaEventRepository repository) : IUmaEventService
{
     public IEnumerable<UmaEventEntity> GetAllWhereNameIsLike(string eventName)
     {
         return repository.Query()
             .AsNoTracking()
             .WhereEventNameContains(eventName);
     }

     public IEnumerable<UmaEventEntity> GetAllWhereChoiceTextIsLike(string choiceText)
     {
         return repository.Query()
             .AsNoTracking()
             .Where(e => e.Choices.Any(c => c.ChoiceText.Contains(choiceText, StringComparison.OrdinalIgnoreCase)));
     }
}