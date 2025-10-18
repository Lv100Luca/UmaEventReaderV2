using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Abstractions.Repositories;

public interface IUmaRepository
{
    Task InitializeAsync(IEnumerable<UmaDto> dtos, CancellationToken cancellationToken = default);

    public bool TryAddSupportUma(Uma uma);

    IEnumerable<Uma> GetAll();

    Uma? GetByFullName(string name);
    IEnumerable<Uma> GetByNames(string[] names);
}