using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Abstractions.Repositories;

public interface IUmaRepository
{
    public void Add(Uma uma);

    public bool TryAddSupportUma(Uma uma);

    IEnumerable<Uma> GetAll();

    Uma? GetByFullName(string name);
    IEnumerable<Uma> GetByNames(string[] names);

    public IEnumerable<Uma> FindOrCreate(string[] names);
    public Uma FindOrCreate(string name);
}