using IronTools.Core.CommandLineParser;

namespace IronTools.Cli.Elevator;

public class ProgramOptions
{
    [CommandLineOption("f", "file", Description = "File to run with elevated privileges", Required = true)]
    public String File { get; set; }
    
    public ProcessingMode Mode { get; set; }
}