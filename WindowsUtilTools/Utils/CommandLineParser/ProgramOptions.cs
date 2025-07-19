namespace Utils.CommandLineParser;

public class ProgramOptions
{
    [CommandLineOption("w", "install-wsl2", Description = "Installs the WSL2", DefaultValue = false)]
    public bool InstallWsl { get; set; }
    
    [CommandLineOption("c", "check-wsl2-install", Description = "Checks if the WSL2 is installed on the given system", DefaultValue = false)]
    public bool CheckWslInstall { get; set; }
    
    public ProcessingMode Mode { get; set; }
}