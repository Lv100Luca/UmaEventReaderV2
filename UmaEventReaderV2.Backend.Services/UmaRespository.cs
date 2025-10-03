using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models.dtos;
using UmaEventReaderV2.Services.Mapper;

namespace UmaEventReaderV2.Services;

public class UmaRepository(ILogger<UmaRepository> logger) : IUmaRepository
{
    private readonly HashSet<Uma> umas = [];

    public async Task InitializeAsync(IEnumerable<UmaDto> dtos,  CancellationToken cancellationToken = default)
    {
        foreach (var dto in dtos)
        {
            var uma = UmaMapper.Map(dto);

            umas.Add(uma);
        }

        logger.LogSuccess($"UmaRepository initialized with {umas.Count} umas.");
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
        return umas.FirstOrDefault(u =>  u.FullName == name);
    }

    public IEnumerable<Uma> GetByNames(string[] names)
    {
        var nameSet = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
        return umas.Where(u => nameSet.Contains(u.FullName));
    }
}