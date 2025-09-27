using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class StaticUmaEventJsonProvider : IUmaEventJsonProvider
{
    private const string JsonFile = "umaEventData.json";

    public async Task<string> GetJsonFileAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, JsonFile);

        if (!File.Exists(path))
            throw new FileNotFoundException("Could not find json file", path);

        return  await File.ReadAllTextAsync(path);
    }
}