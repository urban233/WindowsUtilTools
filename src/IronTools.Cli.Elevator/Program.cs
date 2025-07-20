using IronTools.Core.Cmd.Utils;
using IronTools.Core.CommandLineParser;

namespace IronTools.Cli.Elevator;

class Program
{
    static void Main(string[] args)
    {
        var parser = new CommandLineParser<ProgramOptions>("WSL2Ctrl");
        var options = parser.Parse(args);

        CmdUtils.RestartElevatedCmd([options.File]);
    }
}