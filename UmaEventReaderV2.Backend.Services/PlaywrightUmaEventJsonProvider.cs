using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

// todo move this to separate thing and put into pipeline?
public class PlaywrightUmaEventJsonProvider(ILogger<PlaywrightUmaEventJsonProvider> logger) : IUmaEventJsonProvider
{
    public async Task<string> GetJsonFileAsync()
    {
        logger.LogInformation("Starting Playwright");

        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        logger.LogInformation("Initialized Browser");

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var jsonUrl = "";

        page.Request += (_, request) =>
        {
            if (request.Url.Contains("/api/tool_structural_mappings/554.json"))
            {
                jsonUrl = request.Url;
            }
        };

        await page.GotoAsync("https://game8.co/games/Umamusume-Pretty-Derby/archives/539000");

        if (string.IsNullOrEmpty(jsonUrl))
            throw new Exception("JSON request not found");

        var json = await page.EvaluateAsync<string>(@"url => fetch(url).then(r => r.text())", jsonUrl);

        logger.LogInformation("Got JSON File");

        return json;
    }
}