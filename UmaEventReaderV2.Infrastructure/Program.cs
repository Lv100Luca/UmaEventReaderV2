using System.Diagnostics;

namespace UmaEventReaderV2.Infrastructure;

public class Program
{
    public async static Task Main(string[] args)
    {
        // maybe pass args to clear the db or other later

        var clearDb = true;
        var sw = new Stopwatch();
        sw.Start();

        await Console.Out.WriteLineAsync("Initializing DB...");

        await UmaDbInitializer.InitializeAsync(clearDb);

        sw.Stop();
        await Console.Out.WriteLineAsync($"DB initialized in {sw.ElapsedMilliseconds}ms");
    }
}