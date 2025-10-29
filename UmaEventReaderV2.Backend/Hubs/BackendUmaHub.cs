using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Backend.Hubs;

public class BackendUmaHub(IUmaRepository umaRepository, IUmaSkillRepository skillRepository) : Hub
{
    public async Task<IEnumerable<Uma>> GetUmas()
    {
        var all = umaRepository.GetAll();

        return all;
    }

    public async Task<IEnumerable<UmaSkill>> GetSkillsAsync()
    {
        return await skillRepository.GetAllAsync();
    }
}