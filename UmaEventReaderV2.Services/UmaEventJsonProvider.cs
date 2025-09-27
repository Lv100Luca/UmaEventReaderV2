using Microsoft.Playwright;

namespace UmaEventReaderV2.Services;

public class UmaEventJsonProvider
{
    private readonly static HttpClient Http = new();

    private static string GetMappingUrl()
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return $"https://game8.co/api/tool_structural_mappings/554.json?updatedAt={unixTimestamp}";
    }

    public async static Task<string> GetMappingJsonAsync()
    {
        var url = GetMappingUrl();

        using var client = new HttpClient();

        // Browser-like headers
        SetHttpClientHeaders(client);

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        throw new Exception("Failed to get mapping json");
    }

    private static void SetHttpClientHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:143.0) Gecko/20100101 Firefox/143.0");

        client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        client.DefaultRequestHeaders.Add("Referer", "https://game8.co/games/Umamusume-Pretty-Derby/archives/539000");

        client.DefaultRequestHeaders.Add("X-CSRF-TOKEN",
            "MTfW7p305Sfm7ufsHC/kF317cdL8hLKvCbA0GKpNKtneGWhPVz23kxSdHhCIhxR3IJ6jt3RipWskEj+PSsDvPA==");

        client.DefaultRequestHeaders.Add("Cookie",
            "gtuid=de114a98-1d43-499a-9018-c4b34b2202d2; _session_id=e650b83e9dcd1ef7e900622c95fe68f3; gtsid=bf3bd09e-d79a-43b4-b6a0-4355f8dbd037");
    }

    public async static Task<string> GetMappingJson()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

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

        await page.WaitForTimeoutAsync(2000);

        if (string.IsNullOrEmpty(jsonUrl))
            throw new Exception("JSON request not found");

        return await page.EvaluateAsync<string>(@"url => fetch(url).then(r => r.text())", jsonUrl);
    }
}