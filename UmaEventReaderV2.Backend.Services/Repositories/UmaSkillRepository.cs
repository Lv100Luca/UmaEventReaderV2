using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Services.Repositories;

public class UmaSkillRepository : IUmaSkillRepository
{
    private readonly List<UmaSkill> skills = [];

    public void Add(UmaSkill umaSkill)
    {
        skills.Add(umaSkill);
    }

    public async Task<IEnumerable<UmaSkill>> GetAllAsync()
    {
        return skills;
    }
}