using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Services.Mapper;

namespace UmaEventReaderV2.Services.Repositories;

public class UmaRepository(ILogger<UmaRepository> logger) : IUmaRepository
{
    private readonly HashSet<Uma> umas = [];

    public void Add(Uma uma)
    {
        umas.Add(uma);
    }

    public bool TryAddSupportUma(Uma uma)
    {
        // if (uma is not SupportUma)
        // return false;

        return umas.Add(uma);
    }

    public IEnumerable<Uma> GetAll()
    {
        return umas;
    }

    public Uma? GetByFullName(string name)
    {
        return umas.FirstOrDefault(u => string.Equals(u.FullName, name, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Uma> GetByNames(string[] names)
    {
        var nameSet = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);

        return umas.Where(u => nameSet.Contains(u.FullName));
    }

    public IEnumerable<Uma> FindOrCreate(string[] names)
    {
        foreach (var name in names)
        {
            var foundUma = GetByFullName(name);

            if (foundUma == null)
            {
                foundUma = UmaMapper.Map(name, supportUma: true);

                // actively decide not to add them here
                // they will be present in the events
                // but arent otherwise relevant

                // umaRepository.TryAddSupportUma(foundUma);
            }

            yield return foundUma;
        }
    }

    public Uma FindOrCreate(string name)
    {
        var foundUma = GetByFullName(name);

        if (foundUma == null)
        {
            foundUma = UmaMapper.Map(name, supportUma: true);

            // actively decide not to add them here
            // they will be present in the events
            // but arent otherwise relevant

            // umaRepository.TryAddSupportUma(foundUma);
        }

        return foundUma;
    }
}