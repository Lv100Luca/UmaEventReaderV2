using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Abstractions.Repositories;

public interface IUmaSkillRepository
{
    void Add(UmaSkill umaSkill);

    Task<IEnumerable<UmaSkill>> GetAllAsync();
}