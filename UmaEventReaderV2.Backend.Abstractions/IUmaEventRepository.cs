using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaEventRepository
{
    Task InitializeAsync(IEnumerable<UmaEventChoiceDto> dtos, CancellationToken cancellationToken = default);

    IEnumerable<UmaEvent> GetAll();
    IQueryable<UmaEvent> Query();
    IEnumerable<UmaEvent> GetAllWhereNameIsLike(string eventName);
}