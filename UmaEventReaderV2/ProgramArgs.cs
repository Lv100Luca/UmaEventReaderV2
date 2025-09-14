using CommandLine;

namespace UmaEventReaderV2;

public class ProgramArgs
{
    [Option("select-area", Default = false, HelpText = "Use UI selection for screenshot area instead of static.")]
    public bool SelectArea { get; set; }
}