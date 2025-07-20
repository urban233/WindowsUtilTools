using IronTools.Core.CommandLineParser;
using IronTools.Core.Wsl2.Utils;

namespace IronTools.Cli.Wsl2Ctrl;

class Program
{
    static int Main(string[] args)
    {
        var parser = new CommandLineParser<ProgramOptions>("WSL2Ctrl");
        var options = parser.Parse(args);
        
        if (options.CheckWslInstall)
        {
            if (WslUtils.CheckIfWslIsInstalled())
            {
                return 0;
            }
            return 1;
        }
            
        if (options.InstallWsl)
        {
            WslUtils.InstallWsl2();
            return 0;
        }
        return 1;
    }
}