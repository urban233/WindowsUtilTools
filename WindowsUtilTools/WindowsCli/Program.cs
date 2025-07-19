using Utils;
using Utils.CommandLineParser;

namespace WindowsCli;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var parser = new CommandLineParser<ProgramOptions>("WindowsCli");
            var options = parser.Parse(args);

            if (options.InstallWsl)
            {
                // Install WSL
                // CmdElevator.RestartElevated(["wsl", "--install"]);
                CmdElevator.RestartElevated(["wsl", "--list"]);
            }
            
            Console.WriteLine("Parsed Options:");
            Console.WriteLine($"WSL2 install: {options.InstallWsl}");
            Console.WriteLine($"Mode: {options.Mode}");

            // Your application logic here
            Console.WriteLine("Hello World!");
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
    }
}