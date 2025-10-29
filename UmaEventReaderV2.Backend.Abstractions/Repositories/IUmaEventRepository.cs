using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Abstractions.Repositories;

public interface IUmaEventRepository
{
    // Task InitializeAsync(IEnumerable<UmaEventChoiceDto> dtos, CancellationToken cancellationToken = default);

    void Add(UmaEvent umaEvent);

    IEnumerable<UmaEvent> GetAll();
    IQueryable<UmaEvent> Query();
    IEnumerable<UmaEvent> GetAllWhereNameIsLike(string eventName);
    IEnumerable<UmaEvent> GetAllForCharacterWhereNameIsLike(Uma? uma, string eventName);
}