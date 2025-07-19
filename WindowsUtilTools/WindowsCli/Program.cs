using Utils.CommandLineParser;
using Utils.Wsl2;

namespace WindowsCli;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            var parser = new CommandLineParser<ProgramOptions>("WindowsCli");
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
            
            // Console.WriteLine("Parsed Options:");
            // Console.WriteLine($"WSL2 install: {options.InstallWsl}");
            // Console.WriteLine($"Mode: {options.Mode}");
        }
        catch (CommandLineParseException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine("Use -h or --help for usage information.");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            Environment.Exit(1);
        }

        return -1;
    }
}