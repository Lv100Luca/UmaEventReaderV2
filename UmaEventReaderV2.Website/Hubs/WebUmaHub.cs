using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Website.Hubs;

public class WebUmaHub() : HubBase("umas")
{
    public async Task<IEnumerable<Uma>> GetUmas()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<IEnumerable<Uma>>("GetUmas");
    }

    public async Task<IEnumerable<UmaSkill>> GetSkillsAsync()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<IEnumerable<UmaSkill>>("GetSkillsAsync");
    }

}