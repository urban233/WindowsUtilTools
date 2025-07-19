namespace Utils.CommandLineParser;

public class ProgramOptions
{
    [CommandLineOption("w", "install-wsl2", Description = "Installs the WSL2", DefaultValue = false)]
    public bool InstallWsl { get; set; }
    
    public ProcessingMode Mode { get; set; }
}