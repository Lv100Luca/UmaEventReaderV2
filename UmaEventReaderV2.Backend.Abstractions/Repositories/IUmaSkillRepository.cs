using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Abstractions.Repositories;

public interface IUmaSkillRepository
{
    Task AddAsync(UmaSkill umaSkill);

    Task<IEnumerable<UmaSkill>> GetAllAsync();
}