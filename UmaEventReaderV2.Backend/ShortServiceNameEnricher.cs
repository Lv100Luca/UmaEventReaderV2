using Serilog.Core;
using Serilog.Events;

namespace UmaEventReaderV2.Web;

public class ShortSourceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue("SourceContext", out var prop))
        {
            var fullName = prop.ToString().Trim('"');
            var shortName = fullName.Split('.').Last(); // take last segment
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ServiceName", shortName));
        }
    }
}